namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Enemy", fileName = "Enemy")]
public class Enemy : ScriptableObject, Combatant.Data {

  public new string name;
  public Sprite image;
  public int maxHp;
  public int xpAward;
  public int coinAward;
  public DieType[] slots;
  public DieFace[] dice1;
  public DieFace[] dice2;
  public DieFace[] dice3;

  public string Name => name;
  public Sprite Image => image;
  public int MaxHp => maxHp;
  public DieType[] Slots => slots;
  public DieFace[] Dice1 => dice1;
  public DieFace[] Dice2 => dice2;
  public DieFace[] Dice3 => dice3;
}
}
