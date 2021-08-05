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
        if (defender.Resistance == face.dieType) damage -= 1;
        if (defender.Weakness == face.dieType) damage += 1;
        break;
      case Die.Type.Shield:
        attacker.AddEffect(Effect.Type.Shield, face.amount);
        break;
      case Die.Type.Evade:
        attacker.AddEffect(Effect.Type.Evade, face.amount);
        break;
      case Die.Type.Heal:
        attacker.Heal(face.amount);
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
        defender.effected.Emit((Effect.Type.Shield, -used));
      }
      defender.hp.UpdateVia(hp => Math.Max(0, hp-damage));
    }
  }
}

}
