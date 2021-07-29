namespace dicecraft.Editor {

using System;

using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EnumArrayAttribute))]
public class EnumArrayDrawer : PropertyDrawer {

  public override void OnGUI (Rect rect, SerializedProperty property, GUIContent label) {
    var pathArray = property.propertyPath.Split(new [] { '[', ']' },
        StringSplitOptions.RemoveEmptyEntries);
    var pos = int.Parse(pathArray[pathArray.Length - 1]);
    var names = Enum.GetNames(((EnumArrayAttribute)attribute).enumType);
    var name = pos >= 0 && pos < names.Length ? names[pos] : $"<invalid {pos}>";
    EditorGUI.PropertyField(rect, property, new GUIContent(name), false);
  }
}
}
