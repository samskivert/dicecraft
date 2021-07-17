namespace dicecraft.React {

using System;
using System.Linq;
using Remover = System.Action;

/// <summary>A reactive source that emits a stream of values.</summary>
///
/// A stream does not have a current value, it emits values which are distributed to any registered
/// listeners and then forgotten. Streams do not add any methods to `ISource` but only exist as a
/// type to communicate expected behavior in other APIs. If something return an `IStream`, you know
/// it never has a current value, whereas if something returns an `ISource` it may or may not have a
/// current value.
public interface IStream<T> : ISource<T> {
}

/// <summary>Helper and extension methods for `IStream`.</summary>
public static class Streams {

  /// <summary>A stream derived from some external source of values. See `Derive`.</summary>
  public class Derived<T> : IStream<T> {
    private readonly ConnectValue<T> _connect;
    public Derived (ConnectValue<T> connect) {
      _connect = connect;
    }
    /// from ISource
    public Remover OnEmit (Action<T> fn) => _connect(fn);
    /// from ISource
    public Remover OnValue (Action<T> fn) => _connect(fn);
  }

  /// <summary>Creates a stream based on some external source of values.</summary>
  /// The `connect` function should subscribe to the underlying source, and call `dispatch` with any
  /// future values that arrive. It should return a remover thunk that can be used to clear the
  /// subscription. The remover thunk will be called when the last listener to the subject is
  /// removed and the subject goes dormant. If a new listener subsequently arrives, `connect` will
  /// be called anew to resume wakefulness.
  public static IStream<T> Derive<T> (ConnectValue<T> connect) {
    return new Derived<T>(connect);
  }

  /// <summary>Adapts a delegate to an `IStream`.</summary>
  public static IStream<T> FromDelegate<T> (Action<T> source) {
    return Derive<T>(listener => {
      source += listener;
      return () => source -= listener;
    });
  }

  /// <summary>Adapts an event handler to an `IStream`.</summary>
  public static IStream<T> FromDelegate<T> (EventHandler<T> source) {
    return Derive<T>(listener => {
      EventHandler<T> handler = (src, val) => listener(val);
      source += handler;
      return () => source -= handler;
    });
  }

  /// <summary>Merges `sources` streams into a single stream that emits a value whenever any of the
  /// underlying streams emit a value.</summary>
  public static IStream<T> Merge<T> (params IStream<T>[] streams) {
    return Derive<T>(listener => {
      Remover rems = null;
      foreach (var s in streams) rems += s.OnEmit(listener);
      return rems ?? React.NoopRemover;
    });
  }

  /// <summary>Returns a stream which filters the values of `stream` via `pred`.</summary>
  /// Only the values emitted by stream that satisfy `pred` (cause it to return `true`) will be
  /// emitted by the returned stream.
  public static IStream<T> Filter<T> (this IStream<T> stream, Predicate<T> pred) {
    return Derive<T>(listener => stream.OnEmit(value => {
      if (pred(value)) listener(value);
    }));
  }

  /// <summary>Returns a stream which filters the values of `stream` to instances of a
  /// specific type.</summary>
  public static IStream<D> Filter<T, D> (this ISource<T> source) where D : T {
    return Derive<D>(disp => source.OnEmit(value => {
      if (value is D dvalue) disp(dvalue);
    }));
  }
}
}
