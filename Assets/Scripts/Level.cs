namespace dicecraft {

using System;

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
      if (cell != null && cell.Type == Cell.Type.Start) playerPos.Update(ii);
      cells.Add(ii, cell);
    }
    // AwardDie(data.loot[nextLoot++]);
  }

  public const int MaxDie = 3;

  // public void ProcessSpace (int pos) {
  //   cells.TryGetValue(pos, out var sdata);
  //   if (sdata != null) switch (sdata.spaceType) {
  //   case Space.Type.Battle:
  //     battle.Emit(new Battle(player, data.enemies[nextBattle++]));
  //     // if we are running out of battles, clear this space
  //     if (RemainBattles > data.enemies.Length - nextBattle) spaces[pos] = null;
  //     return;
  //   case Space.Type.Chest:
  //     if (data.loot.Length > nextLoot) {
  //       AwardDie(data.loot[nextLoot++]);
  //       // if we are running out of loot, clear this space
  //       if (RemainSpaces(Space.Type.Chest) > data.loot.Length - nextLoot) spaces[pos] = null;
  //     }
  //     break;
  //   case Space.Type.Die:
  //     switch (sdata.dieType) {
  //     case Die.Type.Heal:
  //       player.hp.UpdateVia(hp => Math.Min(hp + sdata.level, player.MaxHp));
  //       break;
  //     case Die.Type.Shield:
  //       player.AddEffect(Effect.Type.Shield, sdata.level);
  //       break;
  //     case Die.Type.Slash:
  //     case Die.Type.Pierce:
  //     case Die.Type.Blunt:
  //       if (player.ApplyDamage(sdata.dieType, sdata.level)) onDied();
  //       break;
  //     default:
  //       Debug.Log("TODO: handle die space: " + sdata);
  //       break;
  //     }
  //     spaces[pos] = null;
  //     // move this spot to a random place on the level
  //     var newPos = spaces.Keys.Where(ii => spaces[ii] == null).PickUnknown(random);
  //     spaces[newPos] = sdata;
  //     break;
  //   case Space.Type.Trap:
  //     if (sdata.effectType != Effect.Type.None) player.AddEffect(sdata.effectType, sdata.level);
  //     else Debug.Log("TODO: handle trap space: " + sdata);
  //     break;
  //   }

  //   MaybeReRoll();
  // }

  // private void AwardDie (DieData die) {
  //   player.dice.Add(die);
  //   gotDie.Emit(die);
  // }
}
}