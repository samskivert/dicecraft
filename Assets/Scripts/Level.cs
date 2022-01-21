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

  public IMutable<int> coins = Values.Mutable(0);
  public IMutable<int> playerPos = Values.Mutable(0);
  public IMutable<int> earnedGems = Values.Mutable(0);
  public MutableMap<int, Cell.Info> cells = RMaps.LocalMutable<int, Cell.Info>();
  public MutableMap<int, Cell.Info> items = RMaps.LocalMutable<int, Cell.Info>();

  public Emitter<Battle> battle = new Emitter<Battle>();
  public Emitter<DieData> showShop = new Emitter<DieData>();
  public Emitter<ItemData> gotItem = new Emitter<ItemData>();
  public Emitter<DieData> boughtDie = new Emitter<DieData>();
  public Action onExit;
  public Action onDied;

  public int Row (int pos) => pos / data.width;
  public int Col (int pos) => pos % data.width;
  public int Pos (int row, int col) => row * data.width + col;

  public int CellCount => data.cells.Length;

  public Level (Player player, LevelData data, CellData chest = null, CellData shop = null) {
    this.player = player;
    this.data = data;
    for (var ii = 0; ii < data.cells.Length; ii += 1) {
      var cell = (Cell.Info)data.cells[ii];
      if (cell != null && cell.Type == Cell.Type.Entry) playerPos.Update(ii);
      if (cell is ItemData item && chest != null) {
        items.Add(ii, item);
        cells.Add(ii, chest);
      } else if (cell is DieData die && shop != null) {
        items.Add(ii, die);
        cells.Add(ii, shop);
      } else if (cell != null) {
        cells.Add(ii, cell);
      }
    }
    playerPos.OnChange(ProcessSpace);
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

  public bool MoveTo (int pos) {
    if (!CanReach(playerPos.current, pos)) return false;
    if (cells.TryGetValue(pos, out var cell) && !cell.Walkable) return false;
    playerPos.Update(pos);
    return true;
  }

  private bool CanReach (int fromPos, int toPos) {
    var tocheck = new List<int>();
    var seen = new HashSet<int>();
    bool Add (int pos, int dx, int dy) {
      var next = Pos(Row(pos)+dy, Col(pos)+dx);
      if (next == toPos) return true;
      if (seen.Add(next) && !cells.ContainsKey(next)) tocheck.Add(next);
      return false;
    }
    tocheck.Add(fromPos);
    while (tocheck.Count > 0) {
      var nextPos = tocheck.Count-1;
      var next = tocheck[nextPos];
      tocheck.RemoveAt(nextPos);
      if (Add(next, -1,  0)) return true;
      if (Add(next,  1,  0)) return true;
      if (Add(next,  0, -1)) return true;
      if (Add(next,  0,  1)) return true;
    }
    return false;
  }

  public void WonBattle (int coinAward) {
    cells.Remove(playerPos.current);
    coins.UpdateVia(coins => coins + coinAward);
  }

  public void BoughtDie (DieData die) {
    coins.UpdateVia(coins => coins-die.cost);
    boughtDie.Emit(die);
    cells.Remove(playerPos.current);
  }

  private void ProcessSpace (int pos, int oldPos) {
    cells.TryGetValue(pos, out var cell);
    if (cell != null) switch (cell.Type) {
    case Cell.Type.Enemy:
      battle.Emit(new Battle(player, (EnemyData)cell, oldPos));
      break;
    case Cell.Type.Chest:
      gotItem.Emit((ItemData)items.GetValueOrDefault(pos));
      cells.Remove(pos);
      break;
    case Cell.Type.Shop:
      showShop.Emit((DieData)items.GetValueOrDefault(pos));
      break;
    case Cell.Type.HeartUp:
      player.HealthUp();
      cells.Remove(pos);
      break;
    case Cell.Type.DiceUp:
      player.DiceUp();
      cells.Remove(pos);
      break;
    case Cell.Type.Gem:
      earnedGems.UpdateVia(g => g+1);
      cells.Remove(pos);
      break;
    case Cell.Type.Exit:
      onExit();
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
}
}
