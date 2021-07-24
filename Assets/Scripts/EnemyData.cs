namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Enemy", fileName = "Enemy")]
public class EnemyData : ScriptableObject, Combatant.Data {

  public Sprite image;
  public int maxHp;
  public int xpAward;
  public int coinAward;
  public Die.Type[] slots;
  public DieData[] dice;

  public string Name => name;
  public Sprite Image => image;
  public int MaxHp (Player player) => maxHp;
  public Die.Type[] Slots => slots;
  public FaceData[] Dice1 => dice[0].faces;
  public FaceData[] Dice2 => dice.Length > 0 ? dice[1].faces : null;
  public FaceData[] Dice3 => dice.Length > 1 ? dice[2].faces : null;
}
}
