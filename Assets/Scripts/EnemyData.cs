namespace dicecraft {

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Enemy", fileName = "Enemy")]
public class EnemyData : ScriptableObject {

  public Sprite image;
  public int maxHp;
  public int xpAward;
  public int coinAward;
  public int slots;
  public DieData[] dice;
  public Die.Type weakness;
  public Die.Type resistance;
}
}
