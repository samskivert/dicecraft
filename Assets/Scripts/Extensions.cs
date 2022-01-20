namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using TMPro;

using React;

public static class Extensions {

  /// <summary>Destroys all (or all after `fromIndex`) children of `obj`.</summary>
  /// <param name="fromIndex">The index at which to start destroying children.</param>
  public static void DestroyChildren (this GameObject obj, int fromIndex = 0) =>
    DestroyChildren(obj.transform, fromIndex);

  /// <summary>Destroys all (or all after `fromIndex`) children of `transform`.</summary>
  /// <param name="fromIndex">The index at which to start destroying children.</param>
  public static void DestroyChildren (this Transform transform, int fromIndex = 0) {
    for (var ii = fromIndex; ii < transform.childCount; ii++) GameObject.Destroy(
      transform.GetChild(ii).gameObject);
  }

  public static Coroutine RunIn (this MonoBehaviour comp, int frames, Action action) =>
    comp.StartCoroutine(WaitFrames(frames, action));

  public static IEnumerator<object> WaitFrames (int frames, Action action) {
    for (int ii = 0; ii < frames; ii++) yield return null;
    action();
  }

  public static Coroutine RunAfter (this MonoBehaviour comp, float seconds, Action action) =>
    comp.StartCoroutine(WaitSeconds(seconds, action));

  public static IEnumerator<object> WaitSeconds (float seconds, Action action) {
    yield return new WaitForSeconds(seconds);
    action();
  }

  public static T Pick<T> (this System.Random random, IList<T> list, T ifEmpty = default) {
    return list.Count == 0 ? ifEmpty : list[random.Next(list.Count)];
  }

  /// <summary>Pick from an enumerable of unknown length.</summary>
  public static T PickUnknown<T> (
    this IEnumerable<T> enumerable, System.Random rand, T ifEmpty = default
  ) {
    using (var itr = enumerable.GetEnumerator()) {
      if (!itr.MoveNext()) return ifEmpty;
      T result = itr.Current;
      for (int count = 2; itr.MoveNext(); ++count) {
        if (0 == rand.Next(count)) result = itr.Current;
      }
      return result;
    }
  }

  /// <summary>Shuffles `list` in place.</summary>
  public static List<T> Shuffle<T> (this System.Random random, List<T> list) {
    for (var ii = list.Count - 1; ii > 0; ii--) {
      var jj = random.Next(ii + 1);
      if (ii != jj) {
        var tmp = list[ii];
        list[ii] = list[jj];
        list[jj] = tmp;
      }
    }
    return list;
  }

  /// <summary>Configures `dropdown` to pick from a list of values.</summary>
  public static IMutable<E> PickValues<E> (
    this TMP_Dropdown dropdown, IList<E> values, int start, Func<E, string> toString
  ) {
    dropdown.ClearOptions();
    dropdown.AddOptions(values.Select(vv => toString(vv)).ToList());
    dropdown.value = start;
    var option = Values.Mutable(values[start]);
    dropdown.onValueChanged.RemoveAllListeners();
    dropdown.onValueChanged.AddListener(value => option.Update(values[value]));
    return option;
  }

  /// <summary>Configures `dropdown` to pick from a list of values.</summary>
  public static IMutable<E> PickValues<E> (this TMP_Dropdown dropdown, IList<E> values, int start) =>
    PickValues(dropdown, values, start, vv => vv.ToString());

  /// <summary>Configures `dropdown` to pick an enum value.</summary>
  public static IMutable<E> PickEnum<E> (this TMP_Dropdown dropdown, E start) where E : Enum {
    var values = (E[])Enum.GetValues(typeof(E));
    return PickValues(dropdown, values, Array.IndexOf(values, start));
  }

}

}
