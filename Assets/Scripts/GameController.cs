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
  public GameObject titlePrefab;
  public GameObject boardPrefab;
  public GameObject battlePrefab;

  public EnemyData[] enemies;
  public int[] levelXps;
  public int[] levelHps;

  public BoardData[] boards;

  // from Player.LevelData
  public int[] LevelXps => levelXps;
  public int[] LevelHps => levelHps;

  public Board board { get; private set; }

  public void StartBoard (BoardData data) {
    board = new Board(new Player(this, data.player), data);
    board.battle.OnEmit(StartBattle);
    ShowBoard();
  }

  private void Start () {
    ShowTitle();
  }

  private void ShowTitle () {
    var titleScreen = SetScreen(titlePrefab);
    titleScreen.GetComponent<TitleController>().Init(this);
    board = null;
  }

  private void ShowBoard () {
    var boardScreen = SetScreen(boardPrefab);
    boardScreen.GetComponent<BoardController>().Init(this);
    this.board.Roll();
  }

  private void StartBattle (Battle battle) {
    var battleScreen = SetScreen(battlePrefab);
    var battleCtrl = battleScreen.GetComponent<BattleController>();
    battleCtrl.Init(battle, () => {
      ShowBoard();
      board.MaybeReRoll();
    }, ShowTitle);
  }

  private GameObject SetScreen (GameObject prefab) {
    if (screen != null) Destroy(screen);
    screen = Instantiate(prefab, canvas);
    return screen;
  }
}
}
