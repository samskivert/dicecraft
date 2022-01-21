namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Face", fileName = "Face")]
public class FaceData : ScriptableObject {

  public Sprite image;
  public Die.Type dieType;
  public Effect.Type effectType;
  public int amount;

  public string Descrip { get {
    var eff = effectType == Effect.Type.None ? "" : $"{effectType} ";
    switch (dieType) {
    case Die.Type.Slash:
    case Die.Type.Pierce:
    case Die.Type.Blunt:
    case Die.Type.Magic:
      return $"{amount} {eff}damage";
    case Die.Type.SelfEffect:
      switch (effectType) {
      case Effect.Type.Shield: return $"+{amount} shield";
      case Effect.Type.Evade: return $"+{amount}% Evade";
      default: return $"+{amount} {effectType}";
      }
    case Die.Type.Heal:
      return $"+{amount} HP";
    default:
      return $"{amount} {eff}{dieType}";
    }

  }}
  public override string ToString () => $"{name}/{dieType}/{effectType}/{amount}";
}
}
