namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Util;

public class GameController : MonoBehaviour, Player.LevelData {
  private GameObject screen;

  public readonly AnimPlayer anim = new AnimPlayer();

  public Transform canvas;
  public GameObject titlePrefab;
  public GameObject boardPrefab;
  public GameObject battlePrefab;

  public FloatController floater;

  public EnemyData[] enemies;
  public int[] levelXps;
  public int[] levelHps;

  public BoardData[] boards;

  public readonly HashSet<BoardData> unlocked = new HashSet<BoardData>();

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
    unlocked.Add(boards[0]);
    ShowTitle();
  }

  private void Update () => anim.Update(Time.deltaTime);

  private void ShowTitle () {
    var titleScreen = SetScreen(titlePrefab);
    titleScreen.GetComponent<TitleController>().Init(this);
    board = null;
  }

  private void ShowBoard () {
    var boardScreen = SetScreen(boardPrefab);
    var bctrl = boardScreen.GetComponent<BoardController>();
    bctrl.Init(this);
    if (board.RemainBattles == 0) bctrl.ShowCompleted(ShowTitle);
    else board.MaybeReRoll();
  }

  private void StartBattle (Battle battle) {
    var battleScreen = SetScreen(battlePrefab);
    var battleCtrl = battleScreen.GetComponent<BattleController>();
    battleCtrl.Init(this, battle, () => {
      ShowBoard();
    }, ShowTitle);
  }

  private GameObject SetScreen (GameObject prefab) {
    if (screen != null) Destroy(screen);
    screen = Instantiate(prefab, canvas);
    return screen;
  }
}
}
