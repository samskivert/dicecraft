namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Player", fileName = "Player")]
public class PlayerData : ScriptableObject, Unlockable {

  public new string name;
  public string saveId;
  public int price;
  public Sprite image;
  public int maxHp;
  public Die.Type weakness;
  public Die.Type resistance;
  public FaceData special;
  public int specialTurns;
  public DieData[] dice;

  public int[] levelXps;
  public int[] levelHps;

  public string SaveId => saveId;
  public string Name => name;
  public Sprite Image => image;
  public int Price => price;
}
}
