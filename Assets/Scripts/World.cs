namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;

public class World {

  public const int Width = 5;
  public const int Height = 3;

  public Player player;
  public Enemy[] enemies;

  public int[] levelXps;
  public int[] levelHps;
  public int playerLevel;
  public int playerXp;
  public readonly IMutable<int> playerCoins = Values.Mutable(0);

  public int playerHpUp => levelHps[playerLevel];
  public int nextLevelXp => playerLevel < levelXps.Length ? levelXps[playerLevel] : 0;

  public (int, int) entryPos = (0, 0);
  public (int, int) playerPos;

  public Dictionary<(int, int), Encounter> encounters;

  public World (Player player, Enemy[] enemies, int[] levelXps, int[] levelHps) {
    this.player = player;
    this.enemies = enemies;
    this.levelXps = levelXps;
    this.levelHps = levelHps;

    playerPos = entryPos;
    encounters = new Dictionary<(int, int), Encounter>{
      { (0, 0), new Encounter.Blank { exits = Exits((1, 1)) }},
      { (2, 0), new Encounter.Fight { enemy = enemies[1], exits = Exits((3, 0)) }},
      { (3, 0), new Encounter.Shop {} },
      { (1, 1), new Encounter.Fight { enemy = enemies[0], exits = Exits((0, 2), (2, 0), (2, 2), (3, 1)) } },
      { (3, 1), new Encounter.Fight { enemy = enemies[3], exits = Exits((4, 1)) }},
      { (4, 1), new Encounter.Exit {} },
      { (0, 2), new Encounter.Anvil {} },
      { (2, 2), new Encounter.Fight { enemy = enemies[2], exits = Exits((3, 2)) }},
      { (3, 2), new Encounter.Chest {} },
    };
  }

  public bool CanReach ((int, int) coord) {
    bool Search ((int, int) pos) {
      if (pos == coord) return true;
      if (!encounters.TryGetValue(pos, out var encounter)) {
        Debug.Log("Missing encounter? " + pos);
        return false;
      }
      if (encounter is Encounter.Blank && encounter.exits != null) {
        return encounter.exits.Any(Search);
      }
      return false;
    }
    return Search(entryPos);
  }

  public void Award (int xpAward, int coinAward) {
    var next = nextLevelXp;
    if (next == 0) return; // max!
    var newXp = playerXp + xpAward;
    if (newXp >= next) {
      playerLevel += 1;
      newXp -= next;
    }
    playerXp = newXp;

    playerCoins.UpdateVia(coins => coins + coinAward);
  }

  private static (int, int)[] Exits (params (int, int)[] exits) => exits;
}
}
