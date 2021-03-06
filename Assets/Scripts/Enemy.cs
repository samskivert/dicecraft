namespace dicecraft {

using UnityEngine;

public class Enemy : Combatant {

  public readonly EnemyData data;

  public override string Name => data.name;
  public override Sprite Image => data.image;
  public override int MaxHp => data.maxHp;
  public override int Slots => data.slots;
  public override Die.Type Resistance => data.resistance;
  public override Die.Type Weakness => data.weakness;

  public Enemy (EnemyData data) {
    this.data = data;
    hp.Update(data.maxHp);
    dice.AddRange(data.dice);
  }
}
}
