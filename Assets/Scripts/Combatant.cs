namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using React;

public class Combatant {

  public interface Data {
    public string Name { get; }
    public Sprite Image { get; }
    public int StartHp (Player player);
    public int MaxHp (Player player);
    public int Slots (Player player);
    public IEnumerable<DieData> Dice (Player player);
  }

  public readonly List<FaceData> roll = new List<FaceData>();

  public readonly Data data;
  public readonly List<DieData> dice = new List<DieData>();
  public readonly IMutable<int> maxHp = Values.Mutable(0);
  public readonly IMutable<int> hp = Values.Mutable(0);
  public readonly MutableMap<Effect.Type, int> effects = RMaps.LocalMutable<Effect.Type, int>();
  public int slots;

  public Combatant (Player player, Data data) {
    this.data = data;
    hp.Update(data.StartHp(player));
    maxHp.Update(data.MaxHp(player));
    dice.AddRange(data.Dice(player));
    slots = data.Slots(player);
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
