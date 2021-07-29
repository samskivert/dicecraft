namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using React;

public class Battle {

  public readonly Random random = new Random();

  public readonly Player player;
  public readonly Enemy enemy;

  public Battle (Player player, EnemyData enemyData) {
    this.player = player;
    this.enemy = new Enemy(enemyData);
  }

  public void Attack (IEnumerable<(FaceData, int)> dice, Combatant attacker, Combatant defender) {
    var damage = 0;
    foreach (var pair in dice) {
      var face = pair.Item1;
      var upgrades = pair.Item2;

      switch (face.dieType) {
      case Die.Type.Slash:
      case Die.Type.Pierce:
      case Die.Type.Blunt:
      case Die.Type.Magic:
        damage += face.amount;
        break;
      case Die.Type.Shield:
        attacker.effects.Update(Effect.Type.Shield, s => s + face.amount);
        break;
      case Die.Type.Evade:
        attacker.effects.Update(Effect.Type.Evade, s => s + face.amount);
        break;
      case Die.Type.Heal:
        attacker.hp.UpdateVia(hp => hp + face.amount);
        break;
      }
    }

    if (damage > 0) {
      // TODO: potentially evade this attack
      defender.effects.TryGetValue(Effect.Type.Shield, out var shield);
      if (shield > 0) {
        var used = Math.Min(damage, shield);
        damage -= used;
        defender.effects.Update(Effect.Type.Shield, s => s-used);
      }
      defender.hp.UpdateVia(hp => Math.Max(0, hp-damage));
    }
  }
}

}
