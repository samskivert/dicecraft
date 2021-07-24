namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/DieFace", fileName = "DieFace")]
public class DieFace : ScriptableObject {

  public Sprite image;
  public Die.Type dieType;
  public EffectType effectType;
  public int amount;

  public override string ToString () => $"{name}/{dieType}/{effectType}/{amount}";
}
}
