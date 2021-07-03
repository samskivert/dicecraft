namespace dicecraft {

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
}

}
