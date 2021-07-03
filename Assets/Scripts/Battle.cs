namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

public enum DieType { Slash, Pierce, Blunt, Shield, Evade, Magic, Heal }

public enum EffectType { None, Fire, Ice, Poison }

public class Battle {
  public readonly Random random = new Random();

  public readonly Combatant player;
  public readonly Combatant enemy;

  public Battle (Player player, Enemy enemy) {
    this.player = new Combatant(player);
    this.enemy = new Combatant(enemy);
  }

  public void Attack (IEnumerable<DieFace> dice, Combatant attacker, Combatant defender) {
    var damage = 0;
    var shield = 0;
    var evade = 0;
    var heal = 0;
    foreach (var face in dice) {
      switch (face.dieType) {
      case DieType.Slash:
      case DieType.Pierce:
      case DieType.Blunt:
      case DieType.Magic:
        damage += face.amount;
        break;
      case DieType.Shield:
        shield += face.amount;
        break;
      case DieType.Evade:
        evade += face.amount;
        break;
      case DieType.Heal:
        heal += face.amount;
        break;
      }
    }
    // TODO: potentially evade this attack
    defender.hp = Math.Max(0, defender.hp-damage);
    attacker.hp = Math.Min(attacker.hp+heal, attacker.data.MaxHp);
    attacker.shield += shield;
    attacker.evade += evade;
  }
}

}
