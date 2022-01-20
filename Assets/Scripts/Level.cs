namespace dicecraft {

using System;
using System.Collections.Generic;

using UnityEngine;
using React;

public class Level {

  private int nextBattle;
  private int nextLoot;

  public readonly Player player;
  public readonly LevelData data;
  public readonly System.Random random = new System.Random();

  public IMutable<int> playerPos = Values.Mutable(0);
  public MutableMap<int, Cell.Info> cells = RMaps.LocalMutable<int, Cell.Info>();

  public Emitter<Battle> battle = new Emitter<Battle>();
  public Emitter<DieData> gotDie = new Emitter<DieData>();
  public Action onDied;

  public int Row (int pos) => pos / data.width;
  public int Col (int pos) => pos % data.width;
  public int Pos (int row, int col) => row * data.width + col;

  public int CellCount => data.cells.Length;
  public int earnedCoins;

  // public int RemainBattles => RemainSpaces(Space.Type.Battle);
  // private int RemainSpaces (Space.Type type) => cells.Values.Count(
  //   sd => sd != null && sd.spaceType == type);

  public Level (Player player, LevelData data) {
    this.player = player;
    this.data = data;
    for (var ii = 0; ii < data.cells.Length; ii += 1) {
      var cell = (Cell.Info)data.cells[ii];
      if (cell != null && cell.Type == Cell.Type.Entry) playerPos.Update(ii);
      cells.Add(ii, cell);
    }
    playerPos.OnValue(ProcessSpace);
    // AwardDie(data.loot[nextLoot++]);
  }

  public const int MaxDie = 3;

  public void Move (int dx, int dy) {
    var pos = playerPos.current;
    var nrow = Row(pos) + dy;
    var ncol = Col(pos) + dx;
    var npos = Pos(nrow, ncol);
    var cell = cells.GetValueOrDefault(npos);
    if (cell != null && !cell.Walkable) {
      Debug.Log("Can't move to " + npos + " " + cell);
      return;
    }
    playerPos.Update(npos);
  }

  public void WonBattle (int awardCoins) {
    earnedCoins += awardCoins;
    cells.Remove(playerPos.current);
  }

  private void ProcessSpace (int pos) {
    cells.TryGetValue(pos, out var cell);
    if (cell != null) switch (cell.Type) {
    case Cell.Type.Enemy:
      battle.Emit(new Battle(player, (EnemyData)cell));
      return;
    case Cell.Type.Chest:
      // if (data.loot.Length > nextLoot) {
      //   AwardDie(data.loot[nextLoot++]);
      //   // if we are running out of loot, clear this space
      //   if (RemainSpaces(Space.Type.Chest) > data.loot.Length - nextLoot) spaces[pos] = null;
      // }
      break;
    // case Cell.Type.Die:
    //   switch (cell.dieType) {
    //   case Die.Type.Heal:
    //     player.hp.UpdateVia(hp => Math.Min(hp + cell.level, player.MaxHp));
    //     break;
    //   case Die.Type.Shield:
    //     player.AddEffect(Effect.Type.Shield, cell.level);
    //     break;
    //   case Die.Type.Slash:
    //   case Die.Type.Pierce:
    //   case Die.Type.Blunt:
    //     if (player.ApplyDamage(cell.dieType, cell.level)) onDied();
    //     break;
    //   default:
    //     Debug.Log("TODO: handle die space: " + cell);
    //     break;
    //   }
    //   spaces[pos] = null;
    //   // move this spot to a random place on the level
    //   var newPos = spaces.Keys.Where(ii => spaces[ii] == null).PickUnknown(random);
    //   spaces[newPos] = cell;
    //   break;
    // case Space.Type.Trap:
    //   if (cell.effectType != Effect.Type.None) player.AddEffect(cell.effectType, cell.level);
    //   else Debug.Log("TODO: handle trap space: " + cell);
    //   break;
    }
  }

  // private void AwardDie (DieData die) {
  //   player.dice.Add(die);
  //   gotDie.Emit(die);
  // }
}
}
