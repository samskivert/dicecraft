namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using React;

public class Board {

  public const int Width = 7;
  public const int Height = 5;
  public const int Spots = 2 * (Width + Height - 2);

  public static (int, int) Coord (int pos) {
    if (pos < Width-1) return (pos, 0);
    pos -= Width-1;
    if (pos < Height-1) return (Width-1, pos);
    pos -= Height-1;
    if (pos < Width-1) return (Width-1-pos, Height-1);
    pos -= Width-1;
    return (0, Height-1-pos);
  }

  public interface Data {
    public EnemyData[] Enemies { get; }
    public int[] LevelXps { get; }
    public int[] LevelHps { get; }
  }

  public readonly Data data;
  public readonly PlayerData player;
  public readonly System.Random random = new System.Random();

  public readonly IMutable<int> playerLevel = Values.Mutable(0);
  public readonly IMutable<int> playerXp = Values.Mutable(0);
  public readonly IMutable<int> playerCoins = Values.Mutable(0);
  public readonly IMutable<int[]> playerSlotLevels = Values.Mutable<int[]>(null);

  public int dieCount = 2;

  public int playerHpUp => data.LevelHps[playerLevel.current];
  public int nextLevelXp =>
    playerLevel.current < data.LevelXps.Length ? data.LevelXps[playerLevel.current] : 0;

  public IMutable<int> playerPos = Values.Mutable(0);
  public IMutable<int[]> roll = Values.Mutable(new int[0]);

  public Board (Data data, PlayerData player) {
    this.data = data;
    this.player = player;
    playerSlotLevels.Update(new int[player.slots.Length]);
  }

  public void Roll () {
    var dice = new int[dieCount];
    for (var ii = 0; ii < dieCount; ii += 1) dice[ii] = random.Next(6)+1;
    roll.Update(dice);
  }

  public void UseDie (int index) {
    var dice = roll.current;
    if (index >= dice.Length) {
      Debug.Log($"Invalid die index {index} (have {dice.Length}).");
      return;
    }
    if (dice[index] < 1) {
      Debug.Log($"Using invalid die ({index}, value {dice[index]}.");
      return;
    }

    playerPos.UpdateVia(pos => (pos + dice[index]) % Spots);
    dice[index] = -1;
    roll.ForceUpdate(dice);
  }

  public void Award (int xpAward, int coinAward) {
    var next = nextLevelXp;
    if (next == 0) return; // max!

    bool levelUp = false;
    playerXp.UpdateVia(xp => {
      var newXp = xp + xpAward;
      if (newXp >= next) {
        levelUp = true;
        newXp -= next;
      }
      return newXp;
    });
    if (levelUp) playerLevel.UpdateVia(level => level+1);

    playerCoins.UpdateVia(coins => coins + coinAward);
  }
}
}
