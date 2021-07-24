namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Face", fileName = "Face")]
public class FaceData : ScriptableObject {

  public Sprite image;
  public Die.Type dieType;
  public EffectType effectType;
  public int amount;

  public override string ToString () => $"{name}/{dieType}/{effectType}/{amount}";
}
}
