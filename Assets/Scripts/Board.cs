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

  private int nextBattle;
  private int nextLoot;

  public readonly Player player;
  public readonly BoardData data;
  public readonly System.Random random = new System.Random();

  public int dieCount = 2;

  public IMutable<int> playerPos = Values.Mutable(0);
  public IMutable<int[]> roll = Values.Mutable(new int[0]);
  public MutableMap<int, SpaceData> spaces = RMaps.LocalMutable<int, SpaceData>();

  public Emitter<Battle> battle = new Emitter<Battle>();

  public Board (Player player, BoardData data) {
    this.player = player;
    this.data = data;
    for (var ii = 0; ii < data.spaces.Length; ii += 1) spaces.Add(ii, data.spaces[ii]);
    // TODO: emit loot award
    player.dice.Add(data.loot[nextLoot++]);
  }

  public void Roll () {
    var dice = new int[dieCount];
    for (var ii = 0; ii < dieCount; ii += 1) dice[ii] = random.Next(6)+1;
    roll.Update(dice);
  }

  public void MaybeReRoll () {
    if (!roll.current.Any(die => die > 0)) Roll();
  }

  public void UseDie (int index) {
    var dice = roll.current;
    if (index >= dice.Length) {
      Debug.Log($"Invalid die index {index} (have {dice.Length}).");
      return;
    }
    var pips = dice[index];
    if (pips < 1) {
      Debug.Log($"Using invalid die ({index}, value {pips}.");
      return;
    }

    dice[index] = -1;
    roll.ForceUpdate(dice);

    var newPos = (playerPos.current + pips) % Spots;
    playerPos.Update(newPos);

    spaces.TryGetValue(newPos, out var sdata);
    if (sdata != null) switch (sdata.spaceType) {
    case Space.Type.Battle:
      battle.Emit(new Battle(player, data.enemies[nextBattle++]));
      break;
    case Space.Type.Chest:
    case Space.Type.Die:
    case Space.Type.Trap:
      // TODO
      MaybeReRoll();
      break;
    }
    else MaybeReRoll();
  }
}
}
