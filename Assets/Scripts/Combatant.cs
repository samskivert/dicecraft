namespace dicecraft {

using System;
using System.Collections.Generic;

using UnityEngine;

using React;

public class Combatant {

  public virtual string Name { get; }
  public virtual Sprite Image { get; }

  public readonly List<DieData> dice = new List<DieData>();
  public readonly Emitter<DieData> gotDie = new Emitter<DieData>();

  public readonly List<ItemData> items = new List<ItemData>();
  public readonly Emitter<ItemData> gotItem = new Emitter<ItemData>();

  public virtual int MaxHp { get; }
  public virtual int Slots { get; }
  public virtual Die.Type Resistance { get; }
  public virtual Die.Type Weakness { get; }

  public readonly List<FaceData> roll = new List<FaceData>();
  public readonly IMutable<int> hp = Values.Mutable(0);
  public readonly MutableMap<Effect.Type, int> effects = RMaps.LocalMutable<Effect.Type, int>();

  public Emitter<(Effect.Type, int)> effected = new Emitter<(Effect.Type, int)>();
  public Emitter<(Die.Type, int)> diced = new Emitter<(Die.Type, int)>();

  public void AddEffect (Effect.Type type, int amount) {
    effects.Update(type, s => s + amount);
    effected.Emit((type, amount));
  }

  public bool ApplyDamage (Die.Type dieType, int amount) {
    var damage = amount;
    if (Resistance == dieType) damage -= 1;
    if (Weakness == dieType) damage += 1;
    // TODO: potentially evade this attack
    effects.TryGetValue(Effect.Type.Shield, out var shield);
    if (shield > 0) {
      var used = Math.Min(damage, shield);
      damage -= used;
      effects.Update(Effect.Type.Shield, s => s-used);
      effected.Emit((Effect.Type.Shield, -used));
    }
    hp.UpdateVia(hp => Math.Max(0, hp-damage));
    return hp.current == 0;
  }

  public void Heal (int amount) {
    var heal = Math.Min(amount, MaxHp-hp.current);
    hp.UpdateVia(hp => hp + heal);
    diced.Emit((Die.Type.Heal, heal));
  }

  public void Roll (System.Random random) {
    roll.Clear();
    foreach (var die in dice) roll.Add(random.Pick(die.faces));
  }

  public void Play (DieController[] dice, SlotController[] slots) {
    foreach (var die in dice) {
      if (die.frozen) continue;
      if (die.burning) {
        // if this ends the game, stop here
        if (die.Play(this, false)) return;
      }
      die.Play(this, false);
    }
  }
}

}
