namespace dicecraft {

using System;
using System.Collections.Generic;

public enum DamageType { Slash, Pierce, Blunt }

public enum EffectType { None, Fire, Ice, Poison }

public class Battle {
  public readonly Random random = new Random();

  public readonly string playerName;
  public readonly int playerMaxHp;
  public readonly List<DieFace[]> playerDice;

  public readonly Enemy enemy;

  public readonly List<DieFace> roll = new List<DieFace>();

  public int playerHp;
  public int enemyHp;

  public Battle (string playerName, int playerMaxHp, List<DieFace[]> playerDice, Enemy enemy) {
    this.playerName = playerName;
    this.playerMaxHp = playerMaxHp;
    this.playerDice = playerDice;
    this.enemy = enemy;

    playerHp = playerMaxHp;
    enemyHp = enemy.maxHp;
  }

  public void Roll () {
    roll.Clear();

    for (var ii = 0; ii < playerDice.Count; ii += 1) {
      var die = playerDice[ii];
      roll.Add(die[random.Next(die.Length)]);
    }
  }
}

}
