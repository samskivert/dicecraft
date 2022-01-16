namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;
using Util;

public class GameController : MonoBehaviour, Player.LevelData {
  private GameObject screen;

  public readonly AnimPlayer anim = new AnimPlayer();

  public Transform canvas;
  public GameObject titlePrefab;
  public GameObject boardPrefab;
  public GameObject battlePrefab;
  public GameObject lostPopupPrefab;

  public GameObject debugPrefab;
  public Button debugButton;

  public FloatController floater;

  public int[] levelXps;
  public int[] levelHps;

  public BoardData[] boards;
  public LevelData[] levels;

  public readonly IMutable<int> coins = Values.Mutable(0);
  public readonly MutableSet<BoardData> unlocked = RSets.LocalMutable<BoardData>();

  // from Player.LevelData
  public int[] LevelXps => levelXps;
  public int[] LevelHps => levelHps;

  public Board board { get; private set; }
  public Level level { get; private set; }

  public void StartBoard (BoardData data) {
    board = new Board(new Player(this, data.player), data);
    board.battle.OnEmit(StartBattle);
    ShowBoard();
  }

  public void Award (int coinAward) {
    coins.UpdateVia(coins => coins + coinAward);
  }

  public void BuyBoard (BoardData board) {
    coins.UpdateVia(coins => coins - board.price);
    unlocked.Add(board);
  }

  public void ShowLost (Board board) {
    var popObj = Instantiate(lostPopupPrefab, canvas.transform);
    popObj.GetComponentInChildren<EarnedCoinsController>().Init(board.earnedCoins);
    popObj.GetComponentInChildren<Button>().onClick.AddListener(() => {
      Destroy(popObj);
      ShowTitle();
    });
  }

  private void Start () {
    // sync the player's coins and unlocked boards to prefs
    coins.Update(PlayerPrefs.GetInt("coins"));
    coins.OnEmit(coins => PlayerPrefs.SetInt("coins", coins));

    debugButton.gameObject.SetActive(Application.isEditor);
    debugButton.onClick.AddListener(() => {
      if (screen.GetComponent<DebugController>() != null) {
        ShowTitle();
        debugButton.GetComponentInChildren<TMP_Text>().text = "Debug";
      } else {
        ShowDebug();
        debugButton.GetComponentInChildren<TMP_Text>().text = "Back";
      }
    });

    foreach (var board in boards) {
      if (String.IsNullOrEmpty(board.saveId)) {
        Debug.LogWarning($"Missing save id for board: {board}");
        return;
      } else if (board.saveId.Contains(":")) {
        Debug.LogWarning($"Invalid save id ({board.saveId}) for board: {board}");
        return;
      }
    }

    var unlockedIds = PlayerPrefs.GetString("unlocked");
    if (!String.IsNullOrEmpty(unlockedIds)) {
      var ids = new HashSet<string>(unlockedIds.Split(':'));
      foreach (var board in boards) if (ids.Contains(board.saveId)) unlocked.Add(board);
    }
    unlocked.Add(boards[0]);
    unlocked.OnEmit(uboards => {
      var saveIds = String.Join(":", uboards.Select(board => board.saveId));
      PlayerPrefs.SetString("unlocked", saveIds);
    });

    ShowTitle();
  }

  private void Update () => anim.Update(Time.deltaTime);

  private void ShowTitle () {
    SetScreen(titlePrefab).GetComponent<TitleController>().Init(this);
    board = null;
  }

  private void ShowDebug () {
    SetScreen(debugPrefab).GetComponent<DebugController>().Init(this);
  }

  private void ShowBoard () {
    var bctrl = SetScreen(boardPrefab).GetComponent<BoardController>();
    bctrl.Init(this);
    if (board.RemainBattles == 0) bctrl.ShowCompleted(ShowTitle);
    else board.MaybeReRoll();
  }

  private void StartBattle (Battle battle) {
    var battleCtrl = SetScreen(battlePrefab).GetComponent<BattleController>();
    battleCtrl.Init(this, board, battle, ShowBoard);
  }

  private GameObject SetScreen (GameObject prefab) {
    if (screen != null) Destroy(screen);
    screen = Instantiate(prefab, canvas);
    return screen;
  }
}
}
