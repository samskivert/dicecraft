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

  public IMutable<int> playerPos = Values.Mutable(0);
  public IMutable<int[]> roll = Values.Mutable(new int[0]);
  public MutableMap<int, SpaceData> spaces = RMaps.LocalMutable<int, SpaceData>();

  public Emitter<Battle> battle = new Emitter<Battle>();
  public Emitter<DieData> gotDie = new Emitter<DieData>();
  public Action onDied;

  public int RemainBattles => RemainSpaces(Space.Type.Battle);
  private int RemainSpaces (Space.Type type) => spaces.Values.Count(
    sd => sd != null && sd.spaceType == type);

  public Board (Player player, BoardData data) {
    this.player = player;
    this.data = data;
    for (var ii = 0; ii < data.spaces.Length; ii += 1) spaces.Add(
      ii, data.spaces[ii] ?? (ii > 0 ? random.Pick(data.fillers) : null));
    playerPos.Update(data.startSpace);
    AwardDie(data.loot[nextLoot++]);
  }

  public const int MaxDie = 3;

  public void Roll () {
    var dice = new int[player.BoardDice];
    for (var ii = 0; ii < player.BoardDice; ii += 1) dice[ii] = random.Next(MaxDie)+1;
    roll.Update(dice);
  }

  public void MaybeReRoll () {
    if (!roll.current.Any(die => die > 0)) Roll();
  }

  public int UseDie (int index) {
    var dice = roll.current;
    if (index >= dice.Length) {
      Debug.Log($"Invalid die index {index} (have {dice.Length}).");
      return 0;
    }
    var pips = dice[index];
    if (pips < 1) {
      Debug.Log($"Using invalid die ({index}, value {pips}.");
      return 0;
    }

    dice[index] = -1;
    roll.ForceUpdate(dice);
    return pips;
  }

  public void ProcessSpace (int pos) {
    spaces.TryGetValue(pos, out var sdata);
    if (sdata != null) switch (sdata.spaceType) {
    case Space.Type.Battle:
      battle.Emit(new Battle(player, data.enemies[nextBattle++]));
      // if we are running out of battles, clear this space
      if (RemainBattles > data.enemies.Length - nextBattle) spaces[pos] = null;
      return;
    case Space.Type.Chest:
      if (data.loot.Length > nextLoot) {
        AwardDie(data.loot[nextLoot++]);
        // if we are running out of loot, clear this space
        if (RemainSpaces(Space.Type.Chest) > data.loot.Length - nextLoot) spaces[pos] = null;
      }
      break;
    case Space.Type.Die:
      switch (sdata.dieType) {
      case Die.Type.Heal:
        player.hp.UpdateVia(hp => Math.Min(hp + sdata.level, player.MaxHp));
        break;
      case Die.Type.Shield:
        player.AddEffect(Effect.Type.Shield, sdata.level);
        break;
      case Die.Type.Slash:
      case Die.Type.Pierce:
      case Die.Type.Blunt:
        if (player.ApplyDamage(sdata.dieType, sdata.level)) onDied();
        break;
      default:
        Debug.Log("TODO: handle die space " + sdata.dieType);
        break;
      }
      spaces[pos] = null;
      break;
    case Space.Type.Trap:
      // TODO
      break;
    }

    MaybeReRoll();
  }

  private void AwardDie (DieData die) {
    player.dice.Add(die);
    gotDie.Emit(die);
  }
}
}
