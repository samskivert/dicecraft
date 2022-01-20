namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Level", fileName = "Level")]
public class LevelData : ScriptableObject, Unlockable {

  public string saveId;
  public Sprite image;
  public int price;
  public int width;
  public Sprite[] floorTiles;
  public ScriptableObject[] cells;

  public string SaveId => saveId;
  public string Name => name;
  public Sprite Image => image;
  public int Price => price;

  public override string ToString () => name;
}
}
