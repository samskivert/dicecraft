namespace dicecraft {

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Player", fileName = "Player")]
public class PlayerData : ScriptableObject, Combatant.Data {

  public new string name;
  public Sprite image;
  public int maxHp;
  public Die.Type[] slots;

  public string Name => name;
  public Sprite Image => image;
  public int MaxHp (Player player) => maxHp + player.hpUp;
  public Die.Type[] Slots => slots;
  public IEnumerable<DieData> Dice (Player player) => player.dice;
}
}
