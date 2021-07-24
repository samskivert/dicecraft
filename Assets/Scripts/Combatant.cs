namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Combatant {

  public interface Data {
    public string Name { get; }
    public Sprite Image { get; }
    public int MaxHp (Player player);
    public Die.Type[] Slots { get; }
    public IEnumerable<DieData> Dice (Player player);
  }

  public readonly List<FaceData> roll = new List<FaceData>();

  public readonly Data data;
  public readonly int maxHp;
  public readonly IEnumerable<DieData> dice;
  public int hp;
  public int shield;
  public int evade;

  public Combatant (Player player, Data data) {
    this.data = data;
    hp = maxHp = data.MaxHp(player);
    dice = data.Dice(player);
  }

  public void Roll (System.Random random) {
    roll.Clear();
    foreach (var die in dice) roll.Add(die.faces[random.Next(die.faces.Length)]);
  }

  public void Play (DieController[] dice, SlotController[] slots) {
    foreach (var die in dice) die.Play(false);
  }
}

}
