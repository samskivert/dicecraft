namespace dicecraft {

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Player", fileName = "Player")]
public class PlayerData : ScriptableObject, Combatant.Data {

  public new string name;
  public Sprite image;
  public int maxHp;

  public string Name => name;
  public Sprite Image => image;
  public int MaxHp (Player player) => maxHp + player.hpUp;
  public int Slots (Player player) => player.level.current + 1;
  public IEnumerable<DieData> Dice (Player player) => player.dice;
}
}
