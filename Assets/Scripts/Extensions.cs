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
}

}
