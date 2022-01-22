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

  public readonly MutableMap<int, ItemData> items = RMaps.LocalMutable<int, ItemData>();

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

  public bool UseItem (int index) {
    if (!items.Remove(index, out var item)) return false;
    switch (item.dieType) {
    case Die.Type.Heal:
      Heal(item.level);
      break;
    case Die.Type.SelfEffect:
      AddEffect(item.effectType, item.level);
      break;
    default:
      Debug.LogWarning("Unexpected die type for item " + item.dieType);
      break;
    }
    return true;
  }

  public void AddDie (DieData die) {
    dice.Add(die);
    gotDie.Emit(die);
  }

  public void Roll (System.Random random) {
    roll.Clear();
    foreach (var die in dice) roll.Add(random.Pick(die.faces));
  }
}
}
