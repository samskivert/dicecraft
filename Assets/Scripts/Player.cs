namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using React;

public class Player : Combatant {

  public interface LevelData {
    public int[] LevelXps { get; }
    public int[] LevelHps { get; }
  }

  public readonly LevelData levelData;
  public readonly PlayerData data;

  public readonly IMutable<int> level = Values.Mutable(0);
  public readonly IMutable<int> xp = Values.Mutable(0);
  public readonly IMutable<int> coins = Values.Mutable(0);
  public readonly List<DieData> dice = new List<DieData>();

  public int hpUp => levelData.LevelHps[level.current];
  public int nextLevelXp =>
    level.current < levelData.LevelXps.Length ? levelData.LevelXps[level.current] : 0;

  public override string Name => data.name;
  public override Sprite Image => data.image;
  public override IList<DieData> Dice => dice;
  public override int MaxHp => data.maxHp + hpUp;
  public override int Slots => level.current + 1;

  public Player (LevelData levelData, PlayerData data) {
    this.levelData = levelData;
    this.data = data;
    hp.Update(MaxHp);
  }

  public void Award (int xpAward, int coinAward) {
    var next = nextLevelXp;
    if (next == 0) return; // max!

    bool levelUp = false;
    xp.UpdateVia(xp => {
      var newXp = xp + xpAward;
      if (newXp >= next) {
        levelUp = true;
        newXp -= next;
      }
      return newXp;
    });
    if (levelUp) {
      level.UpdateVia(level => level+1);
      hp.Update(MaxHp);
    }

    coins.UpdateVia(coins => coins + coinAward);
  }
}
}
