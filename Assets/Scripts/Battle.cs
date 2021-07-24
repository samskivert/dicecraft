namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

public class Battle {
  private World world;

  public readonly Random random = new Random();

  public readonly Combatant player;
  public readonly Combatant enemy;

  public Battle (World world, EnemyData enemy) {
    this.world = world;
    this.player = new Combatant(world, world.player);
    this.enemy = new Combatant(world, enemy);
  }

  public void Attack (IEnumerable<(FaceData, int)> dice, Combatant attacker, Combatant defender) {
    var damage = 0;
    var shield = 0;
    var evade = 0;
    var heal = 0;
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
        shield += face.amount;
        break;
      case Die.Type.Evade:
        evade += face.amount;
        break;
      case Die.Type.Heal:
        heal += face.amount;
        break;
      }
    }
    // TODO: potentially evade this attack
    if (defender.shield > 0) {
      var used = Math.Min(damage, defender.shield);
      damage -= used;
      defender.shield -= used;
    }
    defender.hp = Math.Max(0, defender.hp-damage);
    attacker.hp = Math.Min(attacker.hp+heal, attacker.data.MaxHp(world));
    attacker.shield += shield;
    attacker.evade += evade;
  }
}

}
