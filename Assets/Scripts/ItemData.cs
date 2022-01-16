namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Item", fileName = "Item")]
public class ItemData : ScriptableObject, Cell.Info {

  public Sprite image;
  public Cell.Type type;

  public Cell.Type Type => type;
  public Sprite Image => image;
  public bool Walkable => true;

  public override string ToString () => name;
}
}
