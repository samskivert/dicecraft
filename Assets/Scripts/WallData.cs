namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Wall", fileName = "Wall")]
public class WallData : ScriptableObject, Cell.Info {

  public Sprite image;

  public Cell.Type Type => Cell.Type.Wall;
  public Sprite Image => image;
  public bool Walkable => false;

  public override string ToString () => name;
}
}
