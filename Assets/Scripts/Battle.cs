namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

public enum DamageType { Slash, Pierce, Blunt, Shield, Evade, Magic, Heal }

public enum EffectType { None, Fire, Ice, Poison }

public class Battle {
  public readonly Random random = new Random();

  public readonly Player player;
  public readonly Enemy enemy;

  public readonly List<DieFace[]> playerDice = new List<DieFace[]>();
  public readonly List<DieFace> roll = new List<DieFace>();

  public int playerHp;
  public int enemyHp;

  public Battle (Player player, Enemy enemy) {
    this.player = player;
    this.enemy = enemy;
    void MaybeAdd (DieFace[] die) {
      if (die != null && die.Length > 0) playerDice.Add(die);
    }
    MaybeAdd(player.dice1);
    MaybeAdd(player.dice2);

    playerHp = player.maxHp;
    enemyHp = enemy.maxHp;
  }

  public void Roll () {
    roll.Clear();

    for (var ii = 0; ii < playerDice.Count; ii += 1) {
      var die = playerDice[ii];
      roll.Add(die[random.Next(die.Length)]);
    }
  }

  public void Attack (IEnumerable<DieFace> dice) {
    var damage = 0;
    foreach (var face in dice) damage += face.damage; // TODO: effect types, letc.
    enemyHp = Math.Max(0, enemyHp-damage);
  }
}

}
