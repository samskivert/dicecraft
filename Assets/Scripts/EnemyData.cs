namespace dicecraft {

using System.Collections.Generic;
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
  public IEnumerable<DieData> Dice (Player player) => dice;
}
}
