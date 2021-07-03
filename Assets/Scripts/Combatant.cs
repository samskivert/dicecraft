namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Combatant {

  public interface Data {
    public string Name { get; }
    public Sprite Image { get; }
    public int MaxHp { get; }
    public DieType[] Slots { get; }
    public DieFace[] Dice1 { get; }
    public DieFace[] Dice2 { get; }
    public DieFace[] Dice3 { get; }
  }

  public readonly List<DieFace[]> dice = new List<DieFace[]>();
  public readonly List<DieFace> roll = new List<DieFace>();

  public readonly Data data;
  public int hp;

  public Combatant (Data data) {
    void MaybeAdd (DieFace[] die) {
      if (die != null && die.Length > 0) dice.Add(die);
    }

    this.data = data;
    MaybeAdd(data.Dice1);
    MaybeAdd(data.Dice2);

    hp = data.MaxHp;
  }

  public void Roll (System.Random random) {
    roll.Clear();

    for (var ii = 0; ii < dice.Count; ii += 1) {
      var die = dice[ii];
      roll.Add(die[random.Next(die.Length)]);
    }
  }

  // public void Attack (IEnumerable<DieFace> dice) {
  //   var damage = 0;
  //   foreach (var face in dice) damage += face.amount; // TODO: effect types, letc.
  //   enemyHp = Math.Max(0, enemyHp-damage);
  // }
}

}
