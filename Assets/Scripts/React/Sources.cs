namespace dicecraft.React {

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Remover = System.Action;

/// <summary>A reactive source of values.</summary>
/// Sources emit values over time and allow listeners to be registered to hear about those values.
public interface ISource<out T> {

  /// <summary>Registers `fn` to be called with values emitted by this source.</summary>
  /// If the source has a current value, `fn` will _not_ be called with the current value.
  Remover OnEmit (Action<T> fn);

  /// <summary>Registers `fn` to be called with values emitted by this source.</summary>
  /// If the source has a current value, `fn` will be called immediately with the current value.
  Remover OnValue (Action<T> fn);
}

/// <summary>A source whose current value can be read immediately.</summary>
public interface IReadableSource<out T> : ISource<T> {

  /// <value>The current value of the source.</value>
  T current { get; }
}

/// <summary>
/// Utility methods for `ISource` and `IReadableSource`. Those interfaces provide just the basic
/// mechanisms for listening for changes and all additional functionality is provided via
/// extension methods.
/// </summary>
public static class Sources {

  /// <summary>A readable source derived from some external source of values.</summary>
  public class DerivedReadable<T> : IReadableSource<T> {
    private readonly ConnectValue<T> _connect;
    private readonly Func<T> _current;
    private event Action<T> _onEmit;
    private Remover _disconnect;

    public DerivedReadable (ConnectValue<T> connect, Func<T> current) {
      _connect = connect;
      _current = current;
    }

    /// from IReadableSource
    public T current => _current();

    /// from ISource
    public Remover OnEmit (Action<T> fn) {
      var needConnect = _onEmit == null;
      _onEmit += fn;
      if (needConnect) _disconnect = _connect(v => _onEmit(v));
      return () => {
        _onEmit -= fn;
        if (_onEmit == null) {
          _disconnect();
          _disconnect = null;
        }
      };
    }

    /// from ISource
    public Remover OnValue (Action<T> fn) {
      fn(_current());
      return OnEmit(fn);
    }
  }

  /// <summary>Creates a readable source based on some external source of values.</summary>
  /// <param name="connect">When called, subscribes to the underlying source, and call `dispatch`
  /// with any future values that arrive. It should return a remover thunk that can be used to clear
  /// the subscription. The remover thunk will be called when the last listener to the source is
  /// removed and the source goes dormant. If a new listener subsequently arrives, `connect` will
  /// be called anew to resume wakefulness.</param>
  /// <param name="current">Should return the current value of the readable source at any
  /// time.</param>
  public static IReadableSource<T> DeriveReadable<T> (ConnectValue<T> connect, Func<T> current) {
    return new DerivedReadable<T>(connect, current);
  }

  /// <summary>Registers `fn` to be called the first time `source` contains or emits a
  /// value.</summary>
  /// If the source contains a current value, `fn` will be called before this call returns. `fn`
  /// will be called zero or one times.
  /// <returns>A remover thunk (invoke with no args to unregister `fn`).</returns>
  public static Remover Once<T> (this ISource<T> source, Action<T> fn) {
    Remover remover = null;
    var fired = false;
    remover = source.OnValue(t => {
      if (remover != null) remover();
      fn(t);
      fired = true;
    });
    if (!fired) return remover;
    remover();
    return React.NoopRemover;
  }

  public static Remover Once<T> (this IReadableSource<T> src, Action<T> fn) {
    fn(src.current);
    return React.NoopRemover;
  }

  /// <summary>Registers `fn` to be called the next time `source` emits a value.</summary>
  /// If source contains a current value `fn` will _not_ be called with the current value.
  /// <returns>A remover thunk (invoke with no args to unregister `fn`).</returns>
  public static Remover Next<T> (this ISource<T> source, Action<T> fn) {
    Remover remover = null;
    return remover = source.OnEmit(t => { remover(); fn(t); });
  }

  /// <summary>Returns a task that will complete the next time the source emits a value.</summary>
  public static Task<T> NextAsync<T> (this ISource<T> source, CancellationToken cancelToken) {
    var taskCompletionSource = new TaskCompletionSource<T>();
    Remover remover = null;
    CancellationTokenRegistration registration = default;
    remover = source.OnEmit(value => {
      remover();
      registration.Dispose();
      taskCompletionSource.TrySetResult(value);
    });
    registration = cancelToken.Register(() => {
      remover();
      registration.Dispose();
      taskCompletionSource.TrySetCanceled();
    }, true);
    return taskCompletionSource.Task;
  }

  /// <summary>Registers `fn` to be called when `source` contains or emits values which satisfy
  /// `pred`.</summary>
  /// <returns>A remover thunk (invoke with no args to unregister `fn`).</returns>
  public static Remover When<T> (this ISource<T> source, Predicate<T> pred, Action<T> fn) {
    return source.OnValue(v => {
      if (pred(v)) fn(v);
    });
  }

  /// <summary>Registers `fn` to be called when `source` contains or emits values which are not
  /// null.</summary>
  /// <returns>A remover thunk (invoke with no args to unregister `fn`).</returns>
  public static Remover WhenNotNull<T> (this ISource<T> source, Action<T> fn) where T : class {
    return source.OnValue(v => {
      if (v != null) fn(v);
    });
  }

  /// <summary>Registers `fn` to be called the first time `source` contains or emits a value which
  /// satisfies `pred`.</summary>
  /// `fn` will be called zero or one times.
  /// <returns>A remover thunk (invoke with no args to unregister `fn`).</returns>
  public static Remover WhenOnce<T> (this ISource<T> source, Predicate<T> pred, Action<T> fn) {
    Remover remover = null;
    var fired = false;
    remover = source.OnValue(t => {
      if (pred(t)) {
        if (remover != null) remover();
        fn(t);
        fired = true;
      }
    });
    if (!fired) return remover;
    remover();
    return React.NoopRemover;
  }

  /// <summary>Returns a task that will complete the first time the source contains or emits
  /// a value which satifies `pred`.</summary>
  /// <returns>A Task that will complete with that value.</returns>
  public static Task<T> WhenOnceAsync<T> (
      this ISource<T> source, Predicate<T> pred, CancellationToken cancelToken) {
    // If we enter this method and the token is already canceled then we are canceled, even
    // if the source already contains a satisfying value.
    if (cancelToken.IsCancellationRequested) return Task.FromCanceled<T>(cancelToken);
    var taskSource = new TaskCompletionSource<T>();
    CancellationTokenRegistration registration = default;
    Remover remover = null;
    var fired = false;
    remover = source.OnValue(val => {
      if (pred(val)) {
        if (remover != null) {
          remover();
          registration.Dispose();
        }
        taskSource.SetResult(val);
        fired = true;
      }
    });
    if (fired) remover();
    else registration = cancelToken.Register(() => {
      remover();
      registration.Dispose();
      taskSource.SetCanceled();
    }, true);
    return taskSource.Task;
  }

  /// <summary>Returns a source which transforms the values emitted by `source` via `fn`.</summary>
  /// Whenever `source` emits a `value`, the returned source will emit `fn(value)`.
  public static ISource<U> Map<T, U> (this ISource<T> source, Func<T, U> fn) {
    return Streams.Derive<U>(listener => source.OnEmit(value => listener(fn(value))));
  }

  /// <summary>Returns a readable source which transforms the values emitted by `source` via
  /// `fn`.</summary>
  /// Whenever `source` emits a `value`, the returned source will emit `fn(value)`. The current
  /// value is translated at the time that `current` is called.
  public static IReadableSource<U> Map<T, U> (this IReadableSource<T> source, Func<T, U> fn) {
    return DeriveReadable<U>(listener => source.OnEmit(value => listener(fn(value))),
                             () => fn(source.current));
  }

  /// <summary>Folds over `source` creating a value that starts with `start` and changes to `fn`
  /// applied to the value emitted by `source` whenever it is emitted.</summary>
  ///
  /// *Note:* the fold value is only "live" while it has listeners. When it has no listeners, it
  /// will not listen to the underlying source and will not observe events it emits. Thus it is
  /// advisable to only ever create a value using this method and immediately listen to it. If you
  /// will be listening and unlistening to the value, you are better off recreating it each time so
  /// that it's more apparent to readers of the code that the value will contain `start` until a new
  /// value arrives.
  public static IValue<Z> Fold<T, Z> (this ISource<T> source, Z start, Func<Z, T, Z>  fn) {
    var current = start;
    return Values.Derive(disp => source.OnValue(v => {
      Z oldValue = current, newValue = fn(oldValue, v);
      if (!Values.Eq(oldValue, newValue)) {
        current = newValue;
        disp(newValue, oldValue);
      }
    }), () => current);
  }

  /// <summary>Joins a collection of `sources` into a single source which aggregates the latest
  /// value of each underlying source.</summary>
  /// The supplied enumerable will be turned into an ordered list and the iteration order during
  /// that conversion will dictate the order of the joined value.
  public static IReadableSource<IReadOnlyList<A>> Join<A> (IEnumerable<IReadableSource<A>> sources) {
    var srcs = sources.ToList();
    A[] curr = new A[srcs.Count];
    IReadOnlyList<A> currView = curr;

    var connected = false;
    Func<IReadOnlyList<A>> current = () => {
      if (!connected) for (var ii = 0; ii < srcs.Count; ++ii) curr[ii] = srcs[ii].current;
      return currView;
    };

    return DeriveReadable(disp => {
      current(); // refresh our current snapshot
      connected = true;
      var removers = srcs.Select((v, ii) => v.OnEmit(val => {
        curr[ii] = val;
        disp(currView);
      })).ToList();
      return () => { connected = false; removers.ForEach(r => r()); };
    }, current);
  }

  /// <summary>Joins two sources into a single "tuple" source.</summary>
  /// When either of the underlying sources changes, this source will change and the changed
  /// element will be reflected in its new value.
  public static IReadableSource<(A, B)> Join<A, B> (IReadableSource<A> a, IReadableSource<B> b) {
    Func<(A, B)> current = () => (a.current, b.current);
    return DeriveReadable(disp => {
      var curr = current();
      var remove = a.OnEmit(val => {
        curr.Item1 = val;
        disp(curr);
      });
      remove += b.OnEmit(val => {
        curr.Item2 = val;
        disp(curr);
      });
      return remove;
    }, current);
  }

  /// <summary>Joins three sources into a single "triple" source.</summary>
  /// When either of the underlying sources changes, this source will change and the changed
  /// element will be reflected in its new value.
  public static IReadableSource<(A, B, C)> Join<A, B, C> (
    IReadableSource<A> a, IReadableSource<B> b, IReadableSource<C> c
  ) {
    Func<(A, B, C)> current = () => (a.current, b.current, c.current);
    return DeriveReadable(disp => {
      var curr = current();
      var remove = a.OnEmit(val => {
        curr.Item1 = val;
        disp(curr);
      });
      remove += b.OnEmit(val => {
        curr.Item2 = val;
        disp(curr);
      });
      remove += c.OnEmit(val => {
        curr.Item3 = val;
        disp(curr);
      });
      return remove;
    }, current);
  }

  /// <summary>Returns a source which "switches" between successive underlying sources.</summary>
  /// The switched source will always emit events from the "latest" source emitted from `sources`.
  /// If `sources` has a current value, this will immediately start listening to it, otherwise the
  /// first events will come from the first source emitted by `sources`.
  public static ISource<T> Switch<T> (ISource<ISource<T>> sources) {
    return Streams.Derive<T>(disp => {
      var disconnect = React.NoopRemover;
      var unlisten = sources.OnValue(source => {
        disconnect();
        disconnect = source.OnEmit(disp);
      });
      return () => { disconnect(); unlisten(); };
    });
  }

  /// <summary>Returns a source which "switches" between successive underlying sources.</summary>
  /// The switched source will always emit events from the "latest" source emitted by `sources`.
  public static IReadableSource<T> Switch<T> (IReadableSource<IReadableSource<T>> sources) {
    return DeriveReadable(disp => {
      var disconnect = React.NoopRemover;
      var unlisten = sources.OnValue(source => {
        disconnect();
        disconnect = source.OnValue(disp);
      });
      return () => { disconnect(); unlisten(); };
    }, () => sources.current.current);
  }

  /// <summary>Converts `source` into a value via a `Fold` that does not transform the values
  /// emitted by the source in any way.</summary>
  public static IValue<T> ToValue<T> (this ISource<T> source, T start) =>
    source.Fold(start, (ov, nv) => nv);
}
}
