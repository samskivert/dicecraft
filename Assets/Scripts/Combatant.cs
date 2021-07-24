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
    public FaceData[] Dice1 { get; }
    public FaceData[] Dice2 { get; }
    public FaceData[] Dice3 { get; }
  }

  public readonly List<FaceData[]> dice = new List<FaceData[]>();
  public readonly List<FaceData> roll = new List<FaceData>();

  public readonly Data data;
  public readonly int maxHp;
  public int hp;
  public int shield;
  public int evade;

  public Combatant (Player player, Data data) {
    void MaybeAdd (FaceData[] die) {
      if (die != null && die.Length > 0) dice.Add(die);
    }

    this.data = data;
    MaybeAdd(data.Dice1);
    MaybeAdd(data.Dice2);

    hp = maxHp = data.MaxHp(player);
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
