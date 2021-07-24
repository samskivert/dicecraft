namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour, Player.LevelData {
  private GameObject screen;

  public Transform canvas;
  public GameObject battlePrefab;
  public GameObject boardPrefab;

  public PlayerData[] players;
  public EnemyData[] enemies;
  public int[] levelXps;
  public int[] levelHps;

  public BoardData startBoard; // TEMP

  // from Player.LevelData
  public int[] LevelXps => levelXps;
  public int[] LevelHps => levelHps;

  public Player player { get; private set; }
  public Board board { get; private set; }

  // public void EncounterClicked ((int, int) coord, Encounter encounter) {
  //   switch (encounter) {
  //   case Encounter.Fight fight:
  //     var battleScreen = SetScreen(battlePrefab);
  //     var battle = battleScreen.GetComponent<BattleController>();
  //     battle.Init(this, new Battle(player, fight.enemy), () => {
  //       world.encounters[coord] = new Encounter.Blank { exits = encounter.exits };
  //       world.playerPos = coord;
  //       ShowBoard();
  //     }, () => {
  //       ShowBoard();
  //     });
  //     break;
  //   case Encounter.Shop shop:
  //     break;
  //   case Encounter.Chest chest:
  //     break;
  //   case Encounter.Anvil anvil:
  //     break;
  //   case Encounter.Exit exit:
  //     break;
  //   }
  // }

  private void Start () {
    // world = new World(players[0], enemies, levelXps, levelHps);
    player = new Player(this, players[0]);
    board = new Board(startBoard);
    ShowBoard();
  }

  private void ShowBoard () {
    var boardScreen = SetScreen(boardPrefab);
    boardScreen.GetComponent<BoardController>().Init(this);
    this.board.Roll();
  }

  private GameObject SetScreen (GameObject prefab) {
    if (screen != null) Destroy(screen);
    screen = Instantiate(prefab, canvas);
    return screen;
  }
}
}
