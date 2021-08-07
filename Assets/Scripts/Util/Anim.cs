namespace dicecraft.Util {

using UnityEngine;
using System;

/// <summary>An animation is a function that takes a delta time and returns how much time remains
/// until the animation is finished.</summary>
/// When the animation returns 0 or less than 0, it is considered complete and is removed by the
/// animator.
public delegate float AnimFn (float dt);

/// <summary>Helper methods for creating common animations.</summary>
public static class Anim {

  /// <summary>Returns an animation that delays `time` seconds.</summary>
  public static AnimFn Delay (float time) => dt => time -= dt;

  /// <summary>Return an animation that can be used once in a Serial() to wait a frame.</summary>
  public static AnimFn DelayFrame () {
    var seenaframe = false;
    return dt => {
      if (seenaframe) return 0f;
      seenaframe = true;
      return 1f;
    };
  }

  /// <summary>Returns an animation that invokes `act`.</summary>
  public static AnimFn Action (Action act) {
    return dt => {
      act();
      return -dt;
    };
  }

  /// <summary>Returns an animation that invokes `act` after `time` seconds.</summary>
  public static AnimFn DelayedAction (float time, Action act) {
    return Serial(Delay(time), Action(act));
  }

  /// <summary>Returns an animation that runs `anims` in parallel.</summary>
  /// When all of the animations are finished the aggregate animation reports itself as finished.
  public static AnimFn Parallel (params AnimFn[] anims) {
    var done = new bool[anims.Length];
    return dt => {
      var remain = -dt;
      for (int ii = 0, ll = anims.Length; ii < ll; ii += 1) {
        if (done[ii]) continue;
        var aremain = anims[ii](dt);
        if (aremain <= 0) done[ii] = true;
        if (aremain > remain) remain = aremain;
      }
      return remain;
    };
  }

  /// <summary>Returns an animation that runs `anims` in series.</summary>
  /// When the last animation is finished, the aggregate animation reports itself as finished.
  public static AnimFn Serial (params AnimFn[] anims) {
    var curidx = 0;
    return dt => {
      var remain = anims[curidx](dt);
      while (remain <= 0) {
        curidx += 1;
        // apply the leftover delta to the next animation
        if (curidx < anims.Length) remain = anims[curidx](-remain);
        else break;
      }
      return remain;
    };
  }

  public static AnimFn TweenSingle (
      Action<float> setter, float start, float end, float time, Interp interp) {
    var elapsed = 0f;
    var range = end - start;
    return dt => {
      elapsed += dt;
      setter(start + range * interp(Math.Min(1f, elapsed / time)));
      return time - elapsed;
    };
  }

  public static AnimFn CreateLate (Func<AnimFn> creator) {
    AnimFn created = null;
    return dt => {
      if (created == null) created = creator();
      return created(dt);
    };
  }

  public static AnimFn TweenLocalTo (
      Transform xform, Vector3 dest, Quaternion destRot, float time, Interp interp) {
    return TweenLocal(xform, xform.localPosition, dest, xform.localRotation, destRot, time, interp);
  }

  public static AnimFn TweenLocal (
      Transform xform, Vector3 start, Vector3 dest,
      Quaternion startRot, Quaternion destRot, float time, Interp interp) {
    var elapsed = 0f;
    return dt => {
      elapsed += dt;
      var ii = Math.Min(1f, elapsed / time);
      var xx = interp(ii);
      xform.localPosition = Vector3.LerpUnclamped(start, dest, xx);
      xform.localRotation = Quaternion.SlerpUnclamped(startRot, destRot, xx);
      return time - elapsed;
    };
  }

  public static AnimFn TweenLocalTo (
      Transform xform, Vector3 dest, Quaternion destRot, float time,
      Interp xInterp, Interp yInterp, Interp zInterp, Interp rotInterp) {
    return TweenLocal(xform, xform.localPosition, dest, xform.localRotation, destRot, time,
        xInterp, yInterp, zInterp, rotInterp);
  }

  public static AnimFn TweenLocal (
      Transform xform, Vector3 start, Vector3 dest,
      Quaternion startRot, Quaternion destRot, float time,
      Interp xInterp, Interp yInterp, Interp zInterp, Interp rotInterp) {
    var elapsed = 0f;
    var range = dest - start;
    return dt => {
      elapsed += dt;
      var ii = Math.Min(1f, elapsed / time);
      xform.localPosition = new Vector3(
          start.x + range.x * xInterp(ii),
          start.y + range.y * yInterp(ii),
          start.z + range.z * zInterp(ii));
      xform.localRotation = Quaternion.Slerp(startRot, destRot, rotInterp(ii));
      return time - elapsed;
    };
  }

  public static AnimFn TweenLocalRotationTo (
      Transform xform, Quaternion dest, float time, Interp interp) {
    var elapsed = 0f;
    var start = xform.localRotation;
    return dt => {
      var ii = Math.Min(1f, elapsed / time);
      xform.localRotation = Quaternion.SlerpUnclamped(start, dest, interp(ii));
      return time - elapsed;
    };
  }

  /// <summary>Apply relative movement.</summary>
  public static AnimFn TweenLocalRelative (
      Transform xform, Vector3 offset, Quaternion rotation, float time,
      Interp xInterp, Interp yInterp, Interp zInterp, Interp rotInterp) {
    var lastPos = Vector3.zero;
    var lastRot = Quaternion.identity;
    var elapsed = 0f;
    return dt => {
      elapsed += dt;
      var ii = Math.Min(1f, elapsed / time);
      var pos = new Vector3(
          offset.x * xInterp(ii),
          offset.y * yInterp(ii),
          offset.z * zInterp(ii));
      var rot = Quaternion.SlerpUnclamped(Quaternion.identity, rotation, rotInterp(ii));
      xform.localPosition += (pos - lastPos);
      xform.localRotation *= (rot * Quaternion.Inverse(lastRot));
      lastPos = pos;
      lastRot = rot;
      return time - elapsed;
    };
  }

  public static AnimFn Tween2LocalPositionToSpeed (
      Transform xform, Vector3 end, float speed, Interp interp) {
    var elapsed = 0f;
    Vector3 start = default;
    float duration = 0f;
    return dt => {
      if (elapsed == 0) {
        start = xform.localPosition;
        duration = Vector3.Distance(start, end) / speed;
      }
      elapsed += dt;
      var ii = Math.Min(1f, elapsed / duration);
      xform.localPosition = Vector3.LerpUnclamped(start, end, interp(ii));
      return duration - elapsed;
    };
  }

  public static AnimFn Tween2LocalToSpeed (
      Transform xform, Vector3 end, Quaternion endRot, float speed, Interp interp) {
    var elapsed = 0f;
    Vector3 start = default;
    Quaternion startRot = default;
    float duration = 0f;
    return dt => {
      if (elapsed == 0) {
        start = xform.localPosition;
        startRot = xform.localRotation;
        duration = Vector3.Distance(start, end) / speed;
      }
      elapsed += dt;
      var ii = Math.Min(1f, elapsed / duration);
      var xx = interp(ii);
      xform.localPosition = Vector3.LerpUnclamped(start, end, xx);
      xform.localRotation = Quaternion.SlerpUnclamped(startRot, endRot, xx);
      return duration - elapsed;
    };
  }

  public static AnimFn Tween2LocalToSpeed (
      Transform xform, Vector3 end, float speed, Interp interp) {
    var elapsed = 0f;
    Vector3 start = default;
    float duration = 0f;
    return dt => {
      if (elapsed == 0) {
        start = xform.localPosition;
        duration = Vector3.Distance(start, end) / speed;
      }
      elapsed += dt;
      var ii = Math.Min(1f, elapsed / duration);
      var xx = interp(ii);
      xform.localPosition = Vector3.LerpUnclamped(start, end, xx);
      return duration - elapsed;
    };
  }

  public static AnimFn Tween2LocalTo (
      Transform xform, Vector3 end, Quaternion endRot, float duration, Interp interp) {
    var elapsed = 0f;
    Vector3 start = default;
    Quaternion startRot = default;
    return dt => {
      if (elapsed == 0) {
        start = xform.localPosition;
        startRot = xform.localRotation;
      }
      elapsed += dt;
      var ii = Math.Min(1f, elapsed / duration);
      var xx = interp(ii);
      xform.localPosition = Vector3.LerpUnclamped(start, end, xx);
      xform.localRotation = Quaternion.SlerpUnclamped(startRot, endRot, xx);
      return duration - elapsed;
    };
  }

  public static AnimFn Tween2LocalTo (
      Transform xform, Quaternion end, float duration, Interp interp) {
    var elapsed = 0f;
    Quaternion start = default;
    return dt => {
      if (elapsed == 0) start = xform.localRotation;
      elapsed += dt;
      var ii = Math.Min(1f, elapsed / duration);
      var xx = interp(ii);
      xform.localRotation = Quaternion.SlerpUnclamped(start, end, xx);
      return duration - elapsed;
    };
  }

  /// <remarks>
  /// Beware: The starting position is captured at the time you call this method, not when
  /// the animation is first ticked. If configuring a sequential animation this should only be
  /// first, otherwise use TweenLocalPosition().
  /// </remarks>
  public static AnimFn TweenLocalPositionTo (
      Transform xform, Vector3 end, float time, Interp interp) {
    return TweenLocalPosition(xform, xform.localPosition, end, time, interp);
  }

  public static AnimFn TweenLocalPosition (
      Transform xform, Vector3 start, Vector3 end, float time, Interp interp) {
    return TweenVector3(v => xform.localPosition = v, start, end, time, interp);
  }

  public static AnimFn TweenLocalScale (
      Transform xform, Vector3 start, Vector3 end, float time, Interp interp) {
    return TweenVector3(v => xform.localScale = v, start, end, time, interp);
  }

  public static AnimFn TweenVector2 (
      Action<Vector2> setter, Vector2 start, Vector2 end, float time, Interp interp) {
    var elapsed = 0f;
    return dt => {
      elapsed += dt;
      setter(Vector2.LerpUnclamped(start, end, interp(Math.Min(1f, elapsed / time))));
      return time - elapsed;
    };
  }

  public static AnimFn TweenVector3 (
      Action<Vector3> setter, Vector3 start, Vector3 end, float time, Interp interp) {
    var elapsed = 0f;
    return dt => {
      elapsed += dt;
      setter(Vector3.LerpUnclamped(start, end, interp(Math.Min(1f, elapsed / time))));
      return time - elapsed;
    };
  }

  public static AnimFn LobTo (Transform xform, Vector3 end, float height, float time) {
    return Lob(xform, xform.localPosition, end, height, time);
  }

  public static AnimFn Lob (
      Transform xform, Vector3 start, Vector3 end, float height, float time) {
    var elapsed = 0f;
    var maxHeight = start.y + height;
    return dt => {
      elapsed += dt;
      var t = Math.Min(1f, elapsed / time);
      var x = Mathf.LerpUnclamped(start.x, end.x, t);
      var z = Mathf.LerpUnclamped(start.z, end.z, t);
      // TODO: redo this with real ballistic arc calculation and a little bounce at the end
      float y;
      if (t < 0.5f) y = Mathf.LerpUnclamped(start.y, maxHeight, Interps.QuadOut(t/0.5f));
      else y = Mathf.LerpUnclamped(maxHeight, end.y, Interps.QuadIn((t-0.5f)/0.5f));
      xform.localPosition = new Vector3(x, y, z);
      return time - elapsed;
    };
  }

  public static AnimFn ThrobXY (Transform xform, float period = 6f, float scale = 0.1f) {
    var t = 0f;
    return dt => {
      t += dt;
      var s = (float)(Math.Sin(period*t) * scale) + 1f;
      xform.localScale = new Vector3(s, s, 1);
      return 1f;
    };
  }
}

}
