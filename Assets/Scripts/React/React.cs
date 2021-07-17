namespace dicecraft.React {

using System;
using Remover = System.Action;

/// <summary>
/// Subscribes an `Action` to a source, returning a Remover that can be used to unsubscribe.
/// </summary>
public delegate Remover ConnectValue<T> (Action<T> fn);

/// <summary>
/// A callback that consumes a change in value from a reactive source.
/// </summary>
public delegate void OnChange<in T> (T value, T oldValue);

/// <summary>
/// Subscribes a ChangeFn to a source, returning a Remover that can be used to unsubscribe.
/// </summary>
public delegate Remover ConnectChange<T> (OnChange<T> fn);

/// <summary>
/// Static declarations of shared objects in the React namespace.
/// </summary>
public static class React
{
  /// <summary> A no-op Remover (which is an alias for System.Action). </summary>
  public static readonly Remover NoopRemover = () => {};

  /// <summary>Maintains a single reactive observation.</summary>
  public class Observer : IDisposable {
    private Remover _clear = NoopRemover;

    /// <summary>Clears out any previous observation and notes the supplied remover for a new
    /// observation.</summary>
    public void Observe (Remover remover)  {
      _clear();
      _clear = remover;
    }

    /// <summary>Disposes any existing observation.</summary>
    public void Dispose () {
      _clear();
      _clear = NoopRemover;
    }
  }
}

}
