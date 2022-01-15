namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Enemy", fileName = "Enemy")]
public class EnemyData : ScriptableObject, Cell.Info {

  public Sprite image;
  public int maxHp;
  public int xpAward;
  public int coinAward;
  public int slots;
  public DieData[] dice;
  public Die.Type weakness;
  public Die.Type resistance;

  public Cell.Type Type => Cell.Type.Enemy;
  public Sprite Image => image;
  public bool Walkable => true;
}
}
