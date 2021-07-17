namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Combatant {

  public interface Data {
    public string Name { get; }
    public Sprite Image { get; }
    public int MaxHp (World world);
    public Die.Type[] Slots { get; }
    public DieFace[] Dice1 { get; }
    public DieFace[] Dice2 { get; }
    public DieFace[] Dice3 { get; }
  }

  public readonly List<DieFace[]> dice = new List<DieFace[]>();
  public readonly List<DieFace> roll = new List<DieFace>();

  public readonly Data data;
  public int hp;
  public int shield;
  public int evade;

  public Combatant (World world, Data data) {
    void MaybeAdd (DieFace[] die) {
      if (die != null && die.Length > 0) dice.Add(die);
    }

    this.data = data;
    MaybeAdd(data.Dice1);
    MaybeAdd(data.Dice2);

    hp = data.MaxHp(world);
  }

  public void Roll (System.Random random) {
    roll.Clear();

    for (var ii = 0; ii < dice.Count; ii += 1) {
      var die = dice[ii];
      roll.Add(die[random.Next(die.Length)]);
    }
  }

  public void Play (DieController[] dice, SlotController[] slots) {
    foreach (var die in dice) die.Play(false);
  }
}

}
