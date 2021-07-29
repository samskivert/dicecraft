namespace dicecraft {

using System;
using UnityEngine;

/// <summary>Used to name array indices with a corresponding enum.</summary>
///
/// Use it like so:
/// ```
/// [EnumArray(typeof(Feedback.Type))]
/// public Color32[] feedbackColors;
/// ```
public class EnumArrayAttribute : PropertyAttribute {

  public readonly Type enumType;

  public EnumArrayAttribute (Type type) { enumType = type; }
}
}
