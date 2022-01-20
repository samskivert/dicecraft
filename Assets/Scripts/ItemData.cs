namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Item", fileName = "Item")]
public class ItemData : ScriptableObject, Cell.Info {

  public Sprite image;
  public Die.Type dieType;
  public Effect.Type effectType;
  public int level;

  public Cell.Type Type => Cell.Type.Chest;
  public Sprite Image => image;
  public bool Walkable => true;

  public override string ToString () => $"{name}/{dieType}/{effectType}/{level}";
}
}
