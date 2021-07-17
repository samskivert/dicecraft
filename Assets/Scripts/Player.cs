namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Player", fileName = "Player")]
public class Player : ScriptableObject, Combatant.Data {

  public new string name;
  public Sprite image;
  public int maxHp;
  public Die.Type[] slots;
  public DieFace[] dice1;
  public DieFace[] dice2;

  public string Name => name;
  public Sprite Image => image;
  public int MaxHp (World world) => maxHp + world.playerHpUp;
  public Die.Type[] Slots => slots;
  public DieFace[] Dice1 => dice1;
  public DieFace[] Dice2 => dice2;
  public DieFace[] Dice3 => null;
}
}
