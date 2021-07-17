namespace dicecraft.React {

using System;
using System.Collections;
using System.Collections.Generic;
using Remover = System.Action;

/// <summary>
/// A reactive set: emits change events when entries are added or removed. A client can choose to
/// observe fine-grained set changes via `Added` and `Removed`, or treat the set as a `Source` and
/// reprocess the entire set any time it changes.
/// </summary>
public abstract class RSet<TEntry> : IRSet<TEntry> {

  // note: there is no standard IReadOnlySet interface, so we require a mutable ISet
  // here, but the RSet methods must never mutate _contents
  protected abstract ISet<TEntry> _contents { get; }

  // from IReadOnlyCollection
  public int Count { get => _contents.Count; }

  // from ISet
  public bool Contains (TEntry entry) => _contents.Contains(entry);
  public bool IsSubsetOf (IEnumerable<TEntry> other) => _contents.IsSubsetOf(other);
  public bool IsSupersetOf (IEnumerable<TEntry> other) => _contents.IsSupersetOf(other);
  public bool IsProperSubsetOf (IEnumerable<TEntry> other) => _contents.IsProperSubsetOf(other);
  public bool IsProperSupersetOf (IEnumerable<TEntry> other) => _contents.IsProperSupersetOf(other);
  public bool Overlaps (IEnumerable<TEntry> other) => _contents.Overlaps(other);
  public bool SetEquals (IEnumerable<TEntry> other) => _contents.SetEquals(other);

  /// <summary>Returns an enumerator that iterates through the set.</summary>
  System.Collections.IEnumerator IEnumerable.GetEnumerator () => _contents.GetEnumerator();

  /// <summary>Returns an enumerator that iterates through the set.</summary>
  public IEnumerator<TEntry> GetEnumerator () => _contents.GetEnumerator();

  public override bool Equals (Object other) => _contents.Equals(other);

  public override int GetHashCode () => _contents.GetHashCode();

  // from IRSet
  public event OnAdded<TEntry> Added;
  public event OnRemoved<TEntry> Removed;
  public IValue<int> CountValue => this.GetCountValue();

  // from IReadableSource
  public IRSet<TEntry> current => this;

  public Remover OnEmit (Action<IRSet<TEntry>> fn) {
    OnAdded<TEntry> onAdd = (entry) => fn(this);
    OnRemoved<TEntry> onRemove = (entry) => fn(this);
    Added += onAdd;
    Removed += onRemove;
    return () => {
      Added -= onAdd;
      Removed -= onRemove;
    };
  }

  public Remover OnValue (Action<IRSet<TEntry>> fn) {
    var remover = OnEmit(fn);
    fn(this);
    return remover;
  }

  protected void DispatchAdd (TEntry entry) {
    if (Added != null) Added(entry);
  }
  protected void DispatchRemove (TEntry entry) {
    if (Removed != null) Removed(entry);
  }
}

/// <summary>A mutable `RSet` that provides an API for adding and removing entries.</summary>
public abstract class MutableSet<TEntry> : RSet<TEntry>, ISet<TEntry> {

  public bool IsReadOnly { get => false; }

  void ICollection<TEntry>.Add (TEntry entry) => Add(entry);

  public bool Add (TEntry entry) {
    if (!_contents.Add(entry)) return false;
    DispatchAdd(entry);
    return true;
  }

  public bool Remove (TEntry entry) {
    if (!_contents.Remove(entry)) return false;
    DispatchRemove(entry);
    return true;
  }

  public void ExceptWith (IEnumerable<TEntry> other) {
    foreach (var entry in other) Remove(entry);
  }

  public void SymmetricExceptWith (IEnumerable<TEntry> other) {
    var toAdd = new List<TEntry>();
    var toRemove = new List<TEntry>();
    foreach (var entry in other) {
      if (Contains(entry)) toRemove.Add(entry);
      else toAdd.Add(entry);
    }
    ExceptWith(toRemove);
    UnionWith(toAdd);
  }

  public void IntersectWith (IEnumerable<TEntry> other) {
    var otherSet = new HashSet<TEntry>(other);
    var toRemove = new List<TEntry>();
    foreach (var entry in this) if (!otherSet.Contains(entry)) toRemove.Add(entry);
    ExceptWith(toRemove);
  }

  public void UnionWith (IEnumerable<TEntry> other) {
    foreach (var entry in other) Add(entry);
  }

  public void CopyTo (TEntry[] array, int arrayIndex) => _contents.CopyTo(array, arrayIndex);

  public void Clear () {
    // we have to remove individually so we can dispatch remove notices
    var entries = new List<TEntry>(this);
    foreach (var entry in entries) this.Remove(entry);
  }
}

/// <summary>A mutable set that stores data in local memory.</summary>
public class LocalMutableSet<TEntry> : MutableSet<TEntry> {
  private readonly HashSet<TEntry> _data = new HashSet<TEntry>();
  protected override ISet<TEntry> _contents => _data;
}

/// <summary>Local reactive set utility functions.</summary>
public static class RSets {

  /// <summary>Returns a mutable set that stores data in local memory.</summary>
  public static MutableSet<E> LocalMutable<E> () => new LocalMutableSet<E>();
}
}
