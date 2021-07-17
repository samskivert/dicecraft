namespace dicecraft.React {

using System;
using Remover = System.Action;

/// <summary>A stream which can have values emitted on it by external callers.</summary>
public class Emitter<T> : IStream<T> {
  private event Action<T> _onValue;

  /// <summary>Returns whether this emitter has any listeners.</summary>
  public bool active => _onValue != null;

  // from IStream
  public Remover OnEmit (Action<T> fn) {
    _onValue += fn;
    return () => _onValue -= fn;
  }

  // from IStream
  public Remover OnValue (Action<T> fn) => OnEmit(fn);

  /// <summary>Emits `value` on this stream.</summary>
  /// Any current listeners will be notified of the value.
  public void Emit (T value) {
    if (_onValue != null) _onValue(value);
  }
}
}
