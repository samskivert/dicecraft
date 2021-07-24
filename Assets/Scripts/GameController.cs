namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour, Board.Data {
  private GameObject screen;

  public Transform canvas;
  public GameObject worldPrefab;
  public GameObject battlePrefab;
  public GameObject boardPrefab;

  public PlayerData[] players;
  public EnemyData[] enemies;
  public int[] levelXps;
  public int[] levelHps;

  public BoardData startBoard; // TEMP

  // from Board.Data
  public EnemyData[] Enemies => enemies;
  public int[] LevelXps => levelXps;
  public int[] LevelHps => levelHps;

  public World world { get; private set; }
  public Board board { get; private set; }

  public void EncounterClicked ((int, int) coord, Encounter encounter) {
    switch (encounter) {
    case Encounter.Fight fight:
      var battleScreen = SetScreen(battlePrefab);
      var battle = battleScreen.GetComponent<BattleController>();
      battle.Init(this, new Battle(world, fight.enemy), () => {
        world.encounters[coord] = new Encounter.Blank { exits = encounter.exits };
        world.playerPos = coord;
        ShowWorld();
      }, () => {
        ShowWorld();
      });
      break;
    case Encounter.Shop shop:
      break;
    case Encounter.Chest chest:
      break;
    case Encounter.Anvil anvil:
      break;
    case Encounter.Exit exit:
      break;
    }
  }

  private void Start () {
    // world = new World(players[0], enemies, levelXps, levelHps);
    board = new Board(this, players[0]);
    ShowBoard(startBoard);
  }

  private void ShowWorld () {
    var worldScreen = SetScreen(worldPrefab);
    worldScreen.GetComponent<WorldController>().Init(this);
  }

  private void ShowBoard (BoardData board) {
    var boardScreen = SetScreen(boardPrefab);
    boardScreen.GetComponent<BoardController>().Init(this, board);
    this.board.Roll();
  }

  private GameObject SetScreen (GameObject prefab) {
    if (screen != null) Destroy(screen);
    screen = Instantiate(prefab, canvas);
    return screen;
  }
}
}
