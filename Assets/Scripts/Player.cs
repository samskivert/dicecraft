namespace dicecraft {

using System;
using UnityEngine;

public class Player : Combatant {

  public readonly PlayerData data;

  public int hpUp = 0;
  public int slots = 1;

  public override string Name => data.name;
  public override Sprite Image => data.image;
  public override int MaxHp => data.maxHp + hpUp;
  public override int Slots => slots;
  public override Die.Type Resistance => data.resistance;
  public override Die.Type Weakness => data.weakness;

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

  public void HealthUp () {
    hpUp += 2; // TODO: allow PlayerData to config
    hp.Update(MaxHp);
  }

  public void DiceUp () {
    slots += 1;
  }
}
}
