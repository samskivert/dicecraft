namespace dicecraft.React {

using System;
using System.Collections.Generic;
using Remover = System.Action;

public delegate void OnAdded<K, V> (K key, V value, V oldValue);
public delegate void OnRemoved<K, V> (K key, V oldValue);

/// <summary>
/// Interface for reactive maps.
/// </summary>
public interface IMap<TKey, TValue> :
  IReadableSource<IMap<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue>
{
  /// <summary>An event emitted when values are added to (or updated in) this map.</summary>
  event OnAdded<TKey, TValue> Added;

  /// <summary>An event emitted when values are removed from this map.</summary>
  event OnRemoved<TKey, TValue> Removed;

  /// <summary>A reactive view of the keys of this map.</summary>
  /// The value will emit a change when mappings are added or removed. Reactive views are not
  /// provided for `Values` or `Entries` because those change every time anything in the map
  /// changes. Simply call `Map(m => m.Values)` for example.
  IReadableSource<IEnumerable<TKey>> KeysValue { get; }

  /// <summary>The number of entries in this map as a reactive value.</summary>
  IValue<int> CountValue { get; }
}

public static class IMapExtensions {

  /// <summary>Connects `onAdd` to the `Added` event.</summary>
  /// <returns>A remover thunk that can be called to remove the connection.</returns>
  public static Remover OnAdd<TKey, TValue> (
      this IMap<TKey, TValue> map, OnAdded<TKey, TValue> onAdd) {
    map.Added += onAdd;
    return () => map.Added -= onAdd;
  }

  /// <summary>Connects `onEntry` to the `Added` event and immediately runs it on all existing map
  /// entries.</summary>
  /// Note that the call to `onEntry` for the pre-existing entries will provide the default for
  /// `TValue` as the "old" value.
  /// <returns>A remover thunk that can be called to remove the connection.</returns>
  public static Remover OnEntries<TKey, TValue> (
      this IMap<TKey, TValue> map, OnAdded<TKey, TValue> onEntry) {
    map.Added += onEntry;
    foreach (var entry in map) onEntry(entry.Key, entry.Value, default);
    return () => map.Added -= onEntry;
  }

  /// <summary>Connects `onRemove` to the `Removed` event.</summary>
  /// <returns>A remover thunk that can be called to remove the connection.</returns>
  public static Remover OnRemove<TKey, TValue> (
      this IMap<TKey, TValue> map, OnRemoved<TKey, TValue> onRemove) {
    map.Removed += onRemove;
    return () => map.Removed -= onRemove;
  }

  /// <summary>Returns a `Value` that reflects the value of this map at `key`.</summary>
  /// When mapping changes, the value will emit a change. While no mapping exists for key, the value
  /// will contain the default for `TValue`.
  public static IValue<TValue> GetValue<TKey, TValue> (this IMap<TKey, TValue> map, TKey key) =>
    map.ProjectValue(key, v => v);

  /// <summary>Returns a `Value` that reflects a projection (via `fn`) of the value of this map at
  /// `key`.</summary>
  /// When mapping changes, `fn` will be applied to the new value to obtain the projected value, and
  /// if it differs from the previously projected value, a change will be emitted. The projection
  /// function will only ever be called with an actual value; while no mapping exists for key, the
  /// value will contain the default for `TValue`.
  public static IValue<W> ProjectValue<TKey, TValue, W> (
    this IMap<TKey, TValue> map, TKey key, Func<TValue, W> fn
  ) {
    return Values.Derive(disp => {
      OnAdded<TKey, TValue> onAdd = (akey, value, oldValue) => {
        if (Values.Eq(key, akey)) {
          W nvalue = fn(value), ovalue = fn(oldValue);
          if (!Values.Eq(nvalue, ovalue)) disp(nvalue, ovalue);
        }
      };
      map.Added += onAdd;
      OnRemoved<TKey, TValue> onRemove = (rkey, oldValue) => {
        if (Values.Eq(key, rkey)) disp(default, fn(oldValue));
      };
      map.Removed += onRemove;
      return () => {
        map.Added -= onAdd;
        map.Removed -= onRemove;
      };
    }, () => {
      TValue value;
      return map.TryGetValue(key, out value) ? fn(value) : default;
    });
  }

  /// <summary>A reactive view of the keys of this map.</summary>
  /// The value will emit a change when mappings are added or removed. Reactive views are not
  /// provided for `Values` or `Entries` because those change every time anything in the map
  /// changes. Simply call `Map(m => m.Values)` for example.
  public static IReadableSource<IEnumerable<TKey>> GetKeysValue<TKey, TValue> (
    this IMap<TKey, TValue> map
  ) {
    return Sources.DeriveReadable<IEnumerable<TKey>>(
      disp => map.OnEmit(_ => disp(map.Keys)), () => map.Keys);
  }

  /// <summary>The number of entries in this map as a reactive value.</summary>
  public static IValue<int> GetCountValue<TKey, TValue> (this IMap<TKey, TValue> map) {
    return Values.Derive(disp => {
      var oldCount = map.Count;
      OnAdded<TKey, TValue> onAdd = (key, value, oldValue) => {
        var count = map.Count;
        if (count != oldCount) {
          disp(count, oldCount);
          oldCount = count;
        }
      };
      OnRemoved<TKey, TValue> onRemove = (key, oldValue) => {
        var count = map.Count;
        disp(count, count+1);
        oldCount = count;
      };
      map.Added += onAdd;
      map.Removed += onRemove;
      return () => {
        map.Added -= onAdd;
        map.Removed -= onRemove;
      };
    }, () => map.Count);
  }
}

}
