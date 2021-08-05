namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using React;

public class Combatant {

  public virtual string Name { get; }
  public virtual Sprite Image { get; }
  public virtual IList<DieData> Dice { get; }
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

  public void Heal (int amount) {
    var heal = Math.Min(amount, MaxHp-hp.current);
    hp.UpdateVia(hp => hp + heal);
    diced.Emit((Die.Type.Heal, heal));
  }

  public void Roll (System.Random random) {
    roll.Clear();
    foreach (var die in Dice) roll.Add(die.faces[random.Next(die.faces.Length)]);
  }

  public void Play (DieController[] dice, SlotController[] slots) {
    foreach (var die in dice) die.Play(false);
  }
}

}
