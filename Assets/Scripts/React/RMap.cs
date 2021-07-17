namespace dicecraft.React {

using System;
using System.Collections;
using System.Collections.Generic;
using Remover = System.Action;

/// <summary>
/// A reactive map: emits change events when entries are added or removed. A client can choose to
/// observe fine-grained map changes via `Added` and `Removed`, or treat the map as a `Source` and
/// reprocess the entire map any time it changes.
/// </summary>
public abstract class RMap<TKey, TValue> : IReadableSource<IMap<TKey, TValue>>, IMap<TKey, TValue> {

  protected abstract IReadOnlyDictionary<TKey, TValue> _contents { get; }

  // from IMap
  public event OnAdded<TKey, TValue> Added;
  public event OnRemoved<TKey, TValue> Removed;

  /// <summary>The number of elements in the map.</summary>
  public int Count { get => _contents.Count; }

  /// <summary>Gets the element that has the specified key in the map.</summary>
  public TValue this[TKey key] { get => _contents[key]; }

  /// <summary>Gets an enumerable collection that contains the keys in the map.</summary>
  public IEnumerable<TKey> Keys { get => _contents.Keys; }

  /// <summary>Gets an enumerable collection that contains the values in the map.</summary>
  public IEnumerable<TValue> Values { get => _contents.Values; }

  /// <summary>Returns whether the map contains an element that has the specified key.</summary>
  public bool ContainsKey (TKey key) => _contents.ContainsKey(key);

  /// <summary>Returns an enumerator that iterates through the map.</summary>
  System.Collections.IEnumerator IEnumerable.GetEnumerator () => _contents.GetEnumerator();

  /// <summary>Returns an enumerator that iterates through the map.</summary>
  public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator () => _contents.GetEnumerator();

  /// <summary>Retrieves the value that is associated with the specified key.</summary>
  public bool TryGetValue (TKey key, out TValue value) => _contents.TryGetValue(key, out value);

  // TODO: check whether other is instance of Map?
  public override bool Equals (Object other) => _contents.Equals(other);

  public override int GetHashCode () => _contents.GetHashCode();

  // from IMap
  public IReadableSource<IEnumerable<TKey>> KeysValue => this.GetKeysValue();
  public IValue<int> CountValue => this.GetCountValue();

  // from ISource
  public Remover OnEmit (Action<IMap<TKey, TValue>> fn) {
    OnAdded<TKey, TValue> onAdd = (key, value, oldValue) => fn(this);
    OnRemoved<TKey, TValue> onRemove = (key, oldValue) => fn(this);
    Added += onAdd;
    Removed += onRemove;
    return () => {
      Added -= onAdd;
      Removed -= onRemove;
    };
  }

  // from ISource
  public Remover OnValue (Action<IMap<TKey, TValue>> fn) {
    var remover = OnEmit(fn);
    fn(this);
    return remover;
  }

  // from IReadableSource
  public IMap<TKey, TValue> current => this;

  protected void DispatchAdd (TKey key, TValue value, TValue oldValue) {
    if (Added != null) Added(key, value, oldValue);
  }
  protected void DispatchRemove (TKey key, TValue oldValue) {
    if (Removed != null) Removed(key, oldValue);
  }
}

/// <summary>A mutable `RMap` that provides an API for adding and removing entries.</summary>
public abstract class MutableMap<TKey, TValue> : RMap<TKey, TValue>, IDictionary<TKey, TValue> {
  protected abstract IDictionary<TKey, TValue> _ucontents { get; }

  // note: these underlying collections reject mutations so we don't need to wrap
  public new ICollection<TKey> Keys { get => _ucontents.Keys; }
  public new ICollection<TValue> Values { get => _ucontents.Values; }

  public bool IsReadOnly { get => false; }

  public bool Contains (KeyValuePair<TKey, TValue> entry) => _ucontents.Contains(entry);

  public void CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
    _ucontents.CopyTo(array, arrayIndex);

  public new TValue this[TKey key] { get => _ucontents[key]; set {
    TValue oldValue;
    _ucontents.TryGetValue(key, out oldValue);
    _ucontents[key] = value;
    DispatchAdd(key, value, oldValue);
  }}

  public void Add (TKey key, TValue value) {
    TValue oldValue;
    _ucontents.TryGetValue(key, out oldValue);
    _ucontents.Add(key, value);
    DispatchAdd(key, value, oldValue);
  }

  public void Add (KeyValuePair<TKey, TValue> entry) => Add(entry.Key, entry.Value);

  public bool Remove (TKey key) => Remove(key, out var _);

  /// <summary>Attempts to remove `key` from this map, placing its old value in `value`.</summary>
  /// <returns>`true` if the mapping was in the map, was removed and its old value was assigned to
  /// `value`, `false` if the mapping was not in the map.</returns>
  public bool Remove (TKey key, out TValue value) {
    if (!_ucontents.TryGetValue(key, out value)) return false;
    _ucontents.Remove(key);
    DispatchRemove(key, value);
    return true;
  }

  public bool Remove (KeyValuePair<TKey, TValue> entry) {
    var key = entry.Key;
    TValue oldValue;
    if (!_ucontents.TryGetValue(key, out oldValue)) return false;
    if (!dicecraft.React.Values.Eq(entry.Value, oldValue)) return false;
    _ucontents.Remove(key);
    DispatchRemove(key, oldValue);
    return true;
  }

  /// <summary>Updates `key` by applying `updateFn` to the current mapping for `key`.</summary>
  /// If there is no current mapping for `key`, the default for `TValue` will be supplied.
  public void Update (TKey key, Func<TValue, TValue> updateFn) {
    TValue oldValue;
    TryGetValue(key, out oldValue);
    var newValue = updateFn(oldValue);
    _ucontents[key] = newValue;
    DispatchAdd(key, newValue, oldValue);
  }

  public void Clear () {
    // we have to remove individually so we can dispatch remove notices
    var keys = new List<TKey>(Keys);
    foreach (var key in keys) this.Remove(key);
  }

  // TODO: GetMutable, ProjectMutable
}

/// <summary>A mutable map that stores data in local memory.</summary>
public class LocalMutableMap<TKey, TValue> : MutableMap<TKey, TValue> {
  // this double dictionary blah blah is because IDictionary does not extend IReadOnlyDictionary,
  // instead concrete implementations just implement both interfaces...
  private readonly IReadOnlyDictionary<TKey, TValue> _data;
  private readonly IDictionary<TKey, TValue> _udata;

  protected override IReadOnlyDictionary<TKey, TValue> _contents => _data;
  protected override IDictionary<TKey, TValue> _ucontents => _udata;

  public LocalMutableMap (IReadOnlyDictionary<TKey, TValue> data, IDictionary<TKey, TValue> udata) {
    _data = data;
    _udata = udata;
  }
}

/// <summary>Local reactive map utility functions.</summary>
public static class RMaps {

  /// <summary>Returns a mutable map that stores data in local memory.</summary>
  public static MutableMap<K, V> LocalMutable<K, V> () {
    var data = new Dictionary<K, V>();
    return new LocalMutableMap<K, V>(data, data);
  }

  /// <summary>Returns a mutable map that stores data in local memory using a sorted backing
  /// map.</summary>
  public static MutableMap<K, V> LocalSortedMutable<K, V> () {
    var data = new SortedDictionary<K, V>();
    return new LocalMutableMap<K, V>(data, data);
  }
}
}
