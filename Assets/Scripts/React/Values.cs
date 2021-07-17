namespace dicecraft.React {

using System;
using System.Linq;
using System.Collections.Generic;
using Remover = System.Action;

/// <summary>A reactive source of values.</summary>
/// Sources emit values over time and allow listeners to be registered to hear about those values.
public interface IValue<out T> : IReadableSource<T> {

  /// <summary>Registers `fn` to be called when this value changes.</summary>
  /// The new and old values will be provided to the listener.
  /// <returns>A remover thunk (invoke with no args to unregister `fn`).</returns>
  Remover OnChange (OnChange<T> fn);
}

/// <summary>A reactive value which can be mutated by external callers.</summary>
public interface IMutable<T> : IValue<T> {

  /// <summary>Updates this mutable value to `newValue`.</summary>
  /// If `newValue` differs from the current value, listeners will be notified of the change.
  void Update (T newValue);

  /// <summary>Updates this mutable value to `newValue`.</summary>
  /// The change is applied and propagated whether or not `newValue` is equal to the current value.
  void ForceUpdate (T newValue);
}

/// <summary>Helper and extension methods for `Value`.</summary>
public static class Values {

  /// <summary>A readable value derived from some external source of values.</summary>
  public class Derived<T> : IValue<T> {
    private readonly ConnectChange<T> _connect;
    private readonly Func<T> _current;
    private event OnChange<T> _onChange;
    private Remover _disconnect = React.NoopRemover;

    public Derived (ConnectChange<T> connect, Func<T> current) {
      _connect = connect;
      _current = current;
    }

    /// from IReadableSource
    public T current => _current();

    /// from ISource
    public Remover OnEmit (Action<T> fn) => OnChange((v, ov) => fn(v));

    /// from ISource
    public Remover OnValue (Action<T> fn) {
      fn(_current());
      return OnEmit(fn);
    }

    /// from IValue
    public Remover OnChange (OnChange<T> fn) {
      var needConnect = _onChange == null;
      _onChange += fn;
      if (needConnect) _disconnect = _connect((nv, ov) => _onChange(nv, ov));
      return () => {
        _onChange -= fn;
        if (_onChange == null) {
          _disconnect();
          _disconnect = React.NoopRemover;
        }
      };
    }
  }

  /// <summary>
  /// Creates a value derived from an external source. The `current` function should return the
  /// current value. The `connect` function should subscribe to the underlying source, and call
  /// `dispatch` when any future values that arrive. It should return a remover thunk that can be
  /// used to clear the subscription. The remover thunk will be called when the last listener to
  /// the stream is removed and the stream goes dormant. If a new listener subsequently arrives,
  /// `connect` will be called anew to resume wakefulness.
  /// </summary>
  /// <param name="connect">
  /// Called when the value receives its first listener after being in a dormant state.
  /// </param>
  public static IValue<T> Derive<T> (ConnectChange<T> connect, Func<T> current) {
    return new Derived<T>(connect, current);
  }

  /// <summary>Creates a constant value which always contains `value`.</summary>
  public static IValue<T> Constant<T> (T value) {
    return Derive(lner => React.NoopRemover, () => value);
  }

  /// <summary>Compares `one` and `two` for equality.</summary>
  /// We could have the reactive classes accept an optional IEqualityComparer<T>
  /// (which they recommend you implement by extending EqualityComparer<T>..).
  public static bool Eq<T> (T one, T two) {
    return EqualityComparer<T>.Default.Equals(one, two);
    // return (one == null) ? (two == null) : one.Equals(two);
  }

  /// <summary>Returns a value which "switches" between successive underlying values.</summary>
  /// The switched value will always reflect the contents and events of the "latest" value from
  /// `values`. When `values` changes, the switched value will only emit a change if the current
  /// (inner) value of the old (outer) value differs from the current (inner) value of the new
  /// (outer) value.
  public static IValue<T> Switch<T> (IValue<IValue<T>> values) {
    return Derive(disp => {
      var disconnect = values.current.OnChange(disp);
      var unlisten = values.OnChange((v, ov) => {
        disconnect();
        disconnect = v.OnChange(disp);
        T previous = ov.current, current = v.current;
        if (!Eq(current, previous)) {
          disp(current, previous);
        }
      });
      return () => { disconnect(); unlisten(); };
    }, () => values.current.current);
  }

  /// <summary>Joins a collection of `values` into a single value which aggregates the latest value
  /// of each underlying value.</summary>
  /// The supplied enumerable will be turned into an ordered list and the iteration order during
  /// that conversion will dictate the order of the joined value.
  public static IValue<IReadOnlyList<A>> Join<A> (IEnumerable<IValue<A>> values) {
    var vals = values.ToList();
    A[] curr = new A[vals.Count], prev = new A[vals.Count];
    IReadOnlyList<A> currView = curr, prevView = prev;

    var connected = false;
    Func<IReadOnlyList<A>> current = () => {
      if (!connected) for (var ii = 0; ii < vals.Count; ++ii) curr[ii] = vals[ii].current;
      return currView;
    };

    return Derive(disp => {
      current(); // refresh our current snapshot
      connected = true;
      var removers = vals.Select((v, ii) => v.OnChange((val, oldVal) => {
        curr[ii] = val;
        prev[ii] = oldVal;
        disp(currView, prevView);
      })).ToList();
      return () => { connected = false; removers.ForEach(r => r()); };
    }, current);
  }

  /// <summary>Joins two values into a single "tuple" value.</summary>
  /// When either of the underlying values changes, this value will change and the changed element
  /// will be reflected in its new value.
  public static IValue<(A, B)> Join<A, B> (IValue<A> a, IValue<B> b) {
    Func<(A, B)> current = () => (a.current, b.current);
    return Derive(disp => {
      var prev = current();
      var curr = current();
      return a.OnChange((val, oval) => {
        prev.Item1 = oval;
        curr.Item1 = val;
        disp(curr, prev);
      }) +
      b.OnChange((val, oval) => {
        prev.Item2 = oval;
        curr.Item2 = val;
        disp(curr, prev);
      });
    }, current);
  }

  /// <summary>Joins three values into a single "tuple" value.</summary>
  /// When any of the underlying values change, this value will change and the changed element
  /// will be reflected in its new value.
  public static IValue<(A, B, C)> Join<A, B, C> (IValue<A> a, IValue<B> b, IValue<C> c) {
    Func<(A, B, C)> current = () => (a.current, b.current, c.current);
    return Derive(disp => {
      var prev = current();
      var curr = current();
      return a.OnChange((val, oval) => {
        prev.Item1 = oval;
        curr.Item1 = val;
        disp(curr, prev);
      }) +
      b.OnChange((val, oval) => {
        prev.Item2 = oval;
        curr.Item2 = val;
        disp(curr, prev);
      }) +
      c.OnChange((val, oval) => {
        prev.Item3 = oval;
        curr.Item3 = val;
        disp(curr, prev);
      });
    }, current);
  }

  /// <summary>Joins four values into a single "tuple" value.</summary>
  /// When any of the underlying values change, this value will change and the changed element
  /// will be reflected in its new value.
  public static IValue<(A, B, C, D)> Join<A, B, C, D> (
      IValue<A> a, IValue<B> b, IValue<C> c, IValue<D> d) {
    Func<(A, B, C, D)> current = () => (a.current, b.current, c.current, d.current);
    return Derive(disp => {
      var prev = current();
      var curr = current();
      return a.OnChange((val, oval) => {
        prev.Item1 = oval;
        curr.Item1 = val;
        disp(curr, prev);
      }) +
      b.OnChange((val, oval) => {
        prev.Item2 = oval;
        curr.Item2 = val;
        disp(curr, prev);
      }) +
      c.OnChange((val, oval) => {
        prev.Item3 = oval;
        curr.Item3 = val;
        disp(curr, prev);
      }) +
      d.OnChange((val, oval) => {
        prev.Item4 = oval;
        curr.Item4 = val;
        disp(curr, prev);
      });
    }, current);
  }

  /// <summary>Returns a value which maps the contents of `source` via `fn`.</summary>
  /// Whenever `source` emits a value, the returned value will emit `fn(value)`. When the value is
  /// connected, a cached copy of the transformed latest value is maintained and used to statisfy
  /// `IValue.current` calls. When the value is not connected, every call to `IValue.current` must
  /// transform the underlying current value. This is generally only relevant if `fn` is expensive.
  public static IValue<U> Map<T, U> (this IValue<T> source, Func<T, U> fn) {
    var connected = false;
    U latest = default;
    return Derive<U>(listener => {
      connected = true;
      latest = fn(source.current);
      var unlisten = source.OnChange((newValue, oldValue) => {
        var current = fn(newValue);
        var previous = latest;
        if (!Eq(current, previous)) {
          latest = current;
          listener(current, previous);
        }
      });
      return () => { connected = false ; unlisten(); };
    }, () => connected ? latest : fn(source.current));
  }

  public static IValue<R> SwitchMap<T,R> (this IValue<T> value, Func<T, IValue<R>> fn) =>
    Switch(value.Map(fn));

  /// <summary>Returns a value which retains the last non-null value from `source`.</summary>
  /// Note that if `source` starts with a `null` value the retaining value will contain `null`
  /// until `source` reports some non-null value. From then on the returned value will never
  /// contain null.
  public static IValue<T> RetainNonNull<T> (this IValue<T> source) where T : class {
    T retained = source.current;
    return Derive<T>(listener => {
      var current = source.current;
      if (current != null) retained = current;
      return source.OnChange((newValue, oldValue) => {
        if (newValue == null) return;
        if (!Eq(newValue, retained)) {
          var previous = retained;
          listener(retained = newValue, previous);
        }
      });
    }, () => source.current ?? retained);
  }

  /// <summary>Returns an `IStream` that emits a value whenever this value changes.</summary>
  public static IStream<T> ToStream<T> (this IValue<T> value) {
    return Streams.Derive<T>(disp => value.OnChange((v, ov) => disp(v)));
  }

  /// <summary>A mutable value which stores its contents in local memory.</summary>
  public class LocalMutable<T> : IMutable<T> {
    private event OnChange<T> _onChange;
    private T _current;
    public LocalMutable (T start) {
      _current = start;
    }

    /// from ISource
    public Remover OnEmit (Action<T> fn) => OnChange((v, ov) => fn(v));
    /// from ISource
    public Remover OnValue (Action<T> fn) {
      fn(_current);
      return OnEmit(fn);
    }
    /// from IReadableSource
    public T current => _current;
    /// from IValue
    public Remover OnChange (OnChange<T> fn) {
      _onChange += fn;
      return () => _onChange -= fn;
    }
    /// from IMutable
    public void Update (T value) {
      var prev = _current;
      if (!Eq(prev, value)) {
        _current = value;
        if (_onChange != null) _onChange(value, prev);
      }
    }
    /// from IMutable
    public void ForceUpdate (T value) {
      var prev = _current;
      _current = value;
      if (_onChange != null) _onChange(value, prev);
    }

    public override string ToString () => $"Mutable({_current})";
  }

  /// <summary>Creates a mutable value which stores data in local memory.</summary>
  public static IMutable<T> Mutable<T> (T start) => new LocalMutable<T>(start);

  /// <summary>Updates `mutable` to `newValue` iff `pred` is true for the current value.</summary>
  public static void UpdateIf<T> (this IMutable<T> mutable, Predicate<T> pred, T newValue) {
    if (pred(mutable.current)) mutable.Update(newValue);
  }

  /// <summary>Updates `mutable` with `fn` applied to its current value.</summary>
  public static void UpdateVia<T> (this IMutable<T> mutable, Func<T,T> fn) =>
    mutable.Update(fn(mutable.current));
}
}
