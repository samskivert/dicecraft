namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Die", fileName = "Die")]
public class DieData : ScriptableObject, Cell.Info {

  public Sprite image;
  public FaceData[] faces;
  public int cost;

  public Cell.Type Type => Cell.Type.Shop;
  public Sprite Image => image;
  public bool Walkable => true;

  public override string ToString () => name;
}
}
