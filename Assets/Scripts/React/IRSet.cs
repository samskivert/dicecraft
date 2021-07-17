namespace dicecraft.React {

using System;
using System.Collections.Generic;
using Remover = System.Action;

public delegate void OnAdded<E> (E entry);
public delegate void OnRemoved<E> (E entry);

/// <summary>A read-only set interface.</summary>
/// C# unfortunately does not provide one, so this extracts the non-mutating methods from ISet.
public interface IReadOnlySet<TEntry> : IReadOnlyCollection<TEntry> {

  /// Returns whether this set contains `entry`.
  bool Contains (TEntry entry);

  /// <summary>Determines whether this set is a subset of `other`.</summary>
  bool IsSubsetOf (IEnumerable<TEntry> other);

  /// <summary>Determines whether this set is a superset of `other`.</summary>
  bool IsSupersetOf (IEnumerable<TEntry> other);

  /// <summary>Determines whether this set is a proper (strict) subset of `other`.</summary>
  bool IsProperSubsetOf (IEnumerable<TEntry> other);

  /// <summary>Determines whether this set is a proper (strict) superset of `other`.</summary>
  bool IsProperSupersetOf (IEnumerable<TEntry> other);

  /// <summary>Determines whether this set set overlaps `other`.</summary>
  bool Overlaps (IEnumerable<TEntry> other);

  /// <summary>Determines whether this set and `other` contain the same elements.</summary>
  bool SetEquals (IEnumerable<TEntry> other);
}

/// <summary>Interface for reactive sets.</summary>
public interface IRSet<TEntry> : IReadableSource<IRSet<TEntry>>, IReadOnlySet<TEntry> {

  /// <summary>An event emitted when entries are added to the set.</summary>
  event OnAdded<TEntry> Added;

  /// <summary>An event emitted when entries are removed from the set.</summary>
  event OnRemoved<TEntry> Removed;

  /// <summary>The number of entries in this set as a reactive value.</summary>
  IValue<int> CountValue { get; }
}

public static class IRSetExtensions {

  /// <summary>Connects `onAdd` to the `Added` event.</summary>
  /// <returns>A remover thunk that can be called to remove the connection.</returns>
  public static Remover OnAdd<E> (this IRSet<E> set, OnAdded<E> onAdd) {
    set.Added += onAdd;
    return () => set.Added -= onAdd;
  }

  /// <summary>Connects `onEntry` to the `Added` event and immediately runs it on all existing set
  /// entries.</summary>
  /// <returns>A remover thunk that can be called to remove the connection.</returns>
  public static Remover OnEntries<E> (this IRSet<E> set, OnAdded<E> onEntry) {
    set.Added += onEntry;
    foreach (var entry in set) onEntry(entry);
    return () => set.Added -= onEntry;
  }

  /// <summary>Connects `onRemove` to the `Removed` event.</summary>
  /// <returns>A remover thunk that can be called to remove the connection.</returns>
  public static Remover OnRemove<E> (this IRSet<E> set, OnRemoved<E> onRemove) {
    set.Removed += onRemove;
    return () => set.Removed -= onRemove;
  }

  /// <summary>Returns a `Value` that reflects whether the set contains `entry`.</summary>
  public static IValue<bool> ContainsValue<E> (this IRSet<E> set, E entry) {
    return Values.Derive(disp => {
      OnAdded<E> onAdd = (added) => {
        if (Values.Eq(entry, added)) disp(true, false);
      };
      set.Added += onAdd;
      OnRemoved<E> onRemove = (removed) => {
        if (Values.Eq(entry, removed)) disp(false, true);
      };
      set.Removed += onRemove;
      return () => {
        set.Added -= onAdd;
        set.Removed -= onRemove;
      };
    }, () => set.Contains(entry));
  }

  /// <summary>The number of entries in this set as a reactive value.</summary>
  public static IValue<int> GetCountValue<E> (this IRSet<E> set) {
    return Values.Derive<int>(disp => {
      OnAdded<E> onAdd = (entry) => {
        var count = set.Count;
        disp(count, count-1);
      };
      OnRemoved<E> onRemove = (entry) => {
        var count = set.Count;
        disp(count, count+1);
      };
      set.Added += onAdd;
      set.Removed += onRemove;
      return () => {
        set.Added -= onAdd;
        set.Removed -= onRemove;
      };
    }, () => set.Count);
  }
}
}
