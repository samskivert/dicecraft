namespace dicecraft {

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Player", fileName = "Player")]
public class PlayerData : ScriptableObject {

  public new string name;
  public Sprite image;
  public int maxHp;
  public Die.Type weakness;
  public Die.Type resistance;
  public FaceData special;
  public int specialTurns;
}
}
