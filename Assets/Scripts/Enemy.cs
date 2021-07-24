namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Enemy", fileName = "Enemy")]
public class Enemy : ScriptableObject, Combatant.Data {

  public Sprite image;
  public int maxHp;
  public int xpAward;
  public int coinAward;
  public Die.Type[] slots;
  public Die[] dice;

  public string Name => name;
  public Sprite Image => image;
  public int MaxHp (World world) => maxHp;
  public Die.Type[] Slots => slots;
  public DieFace[] Dice1 => dice[0].faces;
  public DieFace[] Dice2 => dice.Length > 0 ? dice[1].faces : null;
  public DieFace[] Dice3 => dice.Length > 1 ? dice[2].faces : null;
}
}
