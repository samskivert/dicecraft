namespace dicecraft.Util {

using System;

/// <summary>An interpolation function.</summary>
/// This maps a value from [0, 1] to a value (mostly) from [0, 1] based on some interpolation
/// function. Input values outside the range [0, 1] are generally handled in the same way as values
/// inside the range and the mapped value may range outside [0, 1] for interpolation functions that
/// are "juicy".
public delegate float Interp (float v);

/// <summary>Repository of Interp functions.</summary>
public static class Interps {

  public static readonly Interp Zero = v => 0f;
  public static readonly Interp Linear = v => v;
  public static readonly Interp Smoothstep = v => v*v * (3 - 2*v);
  public static readonly Interp Smootherstep = v => v*v*v * (v * (v * 6 - 15) + 10);

  public static readonly Interp QuadIn = v => v*v;
  public static readonly Interp QuadOut = v => 1-QuadIn(1-v);
  public static readonly Interp QuadInOut = v => (v < 0.5) ? QuadIn(v*2)/2 : 1-QuadIn((1-v)*2)/2;

  public static readonly Interp CubicIn = v => v*v*v;
  public static readonly Interp CubicOut = v => 1-CubicIn(1-v);
  public static readonly Interp CubicInOut = v => (v < 0.5) ? CubicIn(v*2)/2 : 1-CubicIn((1-v)*2)/2;

  public static readonly Interp SineIn = v => (float)(1 -Math.Cos(v * Math.PI/2));
  public static readonly Interp SineOut = v => (float)(Math.Sin(v * Math.PI/2));
  public static readonly Interp SineInOut = v => (float)(-0.5 * (Math.Cos(Math.PI*v) - 1));

  public static readonly Interp ExpoIn = v => (float)(v > 0 ? Math.Pow(2, 10*(v-1)) : 0f);
  public static readonly Interp ExpoOut = v => (float)(v < 1 ? 1-Math.Pow(2, -10*v) : 1f);
  public static readonly Interp ExpoInOut = v => (float)(v <= 0 ? 0f :
      v < 0.5 ? Math.Pow(2, 10*(2*v-1))/2 :
      v < 1 ? (2-Math.Pow(2, -10*2*(v-0.5)))/2 :
      1f);

  public static readonly Interp CircIn = v => (float)(-(Math.Sqrt(1-v*v)-1));
  public static readonly Interp CircOut = v => 1-CircIn(1-v);
  public static readonly Interp CircInOut = v => (v < 0.5) ? CircIn(v*2)/2 : 1-CircIn((1-v)*2)/2;

  const float C = 1.70158f;
  public static readonly Interp BackIn = v => v * v * ((C + 1) * v - C);
  public static readonly Interp BackOut = v => 1-BackIn(1-v);
  public static readonly Interp BackInOut = v => {
    float s = C * 1.525f, dv = v*2;
    if (dv < 1) return (dv*dv * ((s + 1) * dv - s))/2;
    float idv = dv-2;
    return (idv * idv * ((s + 1) * idv + s) + 2)/2;
  };

  const float TAU = (float)(2 * Math.PI);
  public static Interp ElasticIn (float a = 1f, float p = .4f) {
    float pot = p/TAU, ca = Math.Max(1, a), s = (float)(Math.Asin(1 / ca) * pot);
    return v => (float)(ca * Math.Pow(2, 10 * (v-1)) * Math.Sin((s-v-1) / pot));
  }

  public static Interp ElasticOut (float a = 1f, float p = .4f) {
    float pot = p/TAU, ca = Math.Max(1, a), s = (float)(Math.Asin(1 / ca) * pot);
    return v => (float)(1 - ca * Math.Pow(2, -10 * v) * Math.Sin((v+s) / pot));
  }

  public static Interp ElasticInOut (float a = 1f, float p = .4f) {
    float pot = p/TAU, ca = Math.Max(1, a), s = (float)(Math.Asin(1 / ca) * pot);
    return v => ((v = v*2 - 1) < 0
      ? (float)(ca * Math.Pow(2, 10 * v) * Math.Sin((s-v) / pot))
      : (float)(2 - ca * Math.Pow(2, -10 * v) * Math.Sin((s+v) / pot)) / 2);
  }

  const float b1 =  4/11f, b2 =  6/11f, b3 =  8/11f, b4 =   3/4f, b5 = 9/11f;
  const float b6 = 10/11f, b7 = 15/16f, b8 = 21/22f, b9 = 63/64f, b0 = 1/b1/b1;
  public static readonly Interp BounceIn = v => 1-BounceOut(1-v);
  public static readonly Interp BounceOut = t => (t = +t) < b1 ? b0 * t * t :
      t < b3 ? b0 * (t -= b2) * t + b4 :
      t < b6 ? b0 * (t -= b5) * t + b7 :
      b0 * (t -= b8) * t + b9;
  public static readonly Interp BounceInOut = t =>
      ((t *= 2) <= 1 ? 1-BounceOut(1-t) : 1+BounceOut(t-1)) / 2;

  /// <summary>An interpolator that "shakes" a value around `0`.</summary>
  /// It goes `cycles` times through a sine of `amp`.
  public static Interp Shake (float amp, int cycles) {
    float scale = TAU * cycles;
    return t => (float)Math.Sin(scale * t) * amp;
  }
}
}
