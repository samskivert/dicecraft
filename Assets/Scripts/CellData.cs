namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Cell", fileName = "Cell")]
public class CellData : ScriptableObject, Cell.Info {

  public Sprite image;
  public Cell.Type type;

  public Cell.Type Type => type;
  public Sprite Image => image;
  public bool Walkable => true;

  public override string ToString () => name;
}
}
