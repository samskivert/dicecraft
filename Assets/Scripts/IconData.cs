namespace dicecraft {

using System;

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Icons", fileName = "Icons")]
public class IconData : ScriptableObject {

  [EnumArray(typeof(Die.Type))] public Sprite[] dieIcons;
  [EnumArray(typeof(Effect.Type))] public Sprite[] effectIcons;

  public Sprite Die (Die.Type die) => dieIcons[(int)die];
  public Sprite Effect (Effect.Type effect) => effectIcons[(int)effect];

  public override string ToString () => name;
}
}
