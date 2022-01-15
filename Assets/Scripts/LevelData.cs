namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Level", fileName = "Level")]
public class LevelData : ScriptableObject {

  public string saveId;
  public Sprite image;
  public int price;
  public int width;
  public Sprite[] floorTiles;
  public ScriptableObject[] cells;

  public override string ToString () => name;
}
}
