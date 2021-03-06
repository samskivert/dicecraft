namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Board", fileName = "Board")]
public class BoardData : ScriptableObject {

  public string saveId;
  public Sprite image;
  public PlayerData player;
  public SpaceData[] spaces;
  public DieData[] loot;
  public EnemyData[] enemies;
  public int startSpace;
  public int price;

  public override string ToString () => name;
}
}
