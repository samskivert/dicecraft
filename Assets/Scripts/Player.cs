namespace dicecraft {

using System;
using UnityEngine;

using React;

public class Player : Combatant {

  private static int[] SlotsPerLevel = new [] { 1, 2, 2, 2, 2 };
  private static int[] DicePerLevel  = new [] { 2, 2, 3, 3, 3 };

  public readonly PlayerData data;

  public readonly IMutable<int> level = Values.Mutable(0);
  public readonly IMutable<int> xp = Values.Mutable(0);

  public int hpUp => data.levelHps[level.current];
  public int nextLevelXp => level.current < data.levelXps.Length ? data.levelXps[level.current] : 0;

  public override string Name => data.name;
  public override Sprite Image => data.image;
  public override int MaxHp => data.maxHp + hpUp;
  public override int Slots => SlotsPerLevel[level.current];
  public override Die.Type Resistance => data.resistance;
  public override Die.Type Weakness => data.weakness;
  public int BoardDice => DicePerLevel[level.current];

  public Player (PlayerData data) {
    this.data = data;
    dice.AddRange(data.dice);
    hp.Update(MaxHp);
  }

  public void AwardItem (ItemData item) {
    int next = 1;
    foreach (var entry in items) next = Math.Max(next, entry.Key+1);
    items.Add(next, item);
  }

  public string LevelReward (int level) {
    int oldHp = data.maxHp + data.levelHps[level];
    int newHp = data.maxHp + data.levelHps[level+1];
    var reward = $"HP +{newHp-oldHp}!";
    int oldSlots = SlotsPerLevel[level], newSlots = SlotsPerLevel[level+1];
    if (newSlots > oldSlots) reward += " Attack Slot +1!";
    int oldDice = DicePerLevel[level], newDice = DicePerLevel[level+1];
    if (newDice > oldDice) reward += " Board Dice +1!";
    return reward;
  }

  public void Award (int xpAward) {
    var next = nextLevelXp;
    if (next == 0) return; // max!

    bool levelUp = false;
    xp.UpdateVia(xp => {
      var newXp = xp + xpAward;
      if (newXp >= next) {
        levelUp = true;
        newXp -= next;
      }
      return newXp;
    });
    if (levelUp) {
      level.UpdateVia(level => level+1);
      hp.Update(MaxHp);
    }
  }
}
}
