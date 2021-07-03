namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/DieFace", fileName = "DieFace")]
public class DieFace : ScriptableObject {

  public new string name;
  public Sprite image;
  public DieType dieType;
  public EffectType effectType;
  public int amount;

  public override string ToString () => $"{name}/{dieType}/{effectType}/{amount}";
}
}
