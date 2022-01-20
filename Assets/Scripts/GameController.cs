namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using React;
using Util;

public class GameController : MonoBehaviour, Player.LevelData {
  private GameObject screen;
  private LevelData selLevel;
  private PlayerData selPlayer;

  public readonly AnimPlayer anim = new AnimPlayer();

  public Transform canvas;
  public GameObject titlePrefab;
  public GameObject levelPrefab;
  public GameObject battlePrefab;
  public GameObject lostPopupPrefab;

  public GameObject debugPrefab;
  public Button debugButton;

  public FloatController floater;

  public int[] levelXps;
  public int[] levelHps;

  public LevelData[] levels;
  public PlayerData[] players;

  public readonly IMutable<int> coins = Values.Mutable(0);
  public readonly MutableSet<Unlockable> unlocked = RSets.LocalMutable<Unlockable>();

  // from Player.LevelData
  public int[] LevelXps => levelXps;
  public int[] LevelHps => levelHps;

  public Level level { get; private set; }
  public Player player { get; private set; }

  public void SetLevel (LevelData level) { selLevel = level; }
  public void SetPlayer (PlayerData player) { selPlayer = player; }
  public void StartLevel () {
    player = new Player(this, selPlayer);
    level = new Level(player, selLevel);
    level.battle.OnEmit(StartBattle);
    ShowLevel();
  }

  public void Award (int coinAward) {
    coins.UpdateVia(coins => coins + coinAward);
  }

  public void BuyUnlock (Unlockable unlock) {
    coins.UpdateVia(coins => coins - unlock.Price);
    unlocked.Add(unlock);
  }

  public void ShowLost (Level level) {
    var popObj = Instantiate(lostPopupPrefab, canvas.transform);
    popObj.GetComponentInChildren<EarnedCoinsController>().Init(level.earnedCoins);
    popObj.GetComponentInChildren<Button>().onClick.AddListener(() => {
      Destroy(popObj);
      ShowTitle();
    });
  }

  private void Start () {
    // sync the player's coins and unlocked levels & players to prefs
    coins.Update(PlayerPrefs.GetInt("coins"));
    coins.OnEmit(coins => PlayerPrefs.SetInt("coins", coins));

    // debugButton.gameObject.SetActive(Application.isEditor);
    // debugButton.onClick.AddListener(() => {
    //   if (screen.GetComponent<DebugController>() != null) {
    //     ShowTitle();
    //     debugButton.GetComponentInChildren<TMP_Text>().text = "Debug";
    //   } else {
    //     ShowDebug();
    //     debugButton.GetComponentInChildren<TMP_Text>().text = "Back";
    //   }
    // });

    foreach (var level in levels) {
      if (String.IsNullOrEmpty(level.saveId)) {
        Debug.LogWarning($"Missing save id for level: {level}");
        return;
      } else if (level.saveId.Contains(":")) {
        Debug.LogWarning($"Invalid save id ({level.saveId}) for level: {level}");
        return;
      }
    }

    var unlockedIds = PlayerPrefs.GetString("unlocked");
    if (!String.IsNullOrEmpty(unlockedIds)) {
      var ids = new HashSet<string>(unlockedIds.Split(':'));
      foreach (var level in levels) if (ids.Contains(level.saveId)) unlocked.Add(level);
      foreach (var player in players) if (ids.Contains(player.saveId)) unlocked.Add(player);
    }
    unlocked.Add(levels[0]);
    unlocked.Add(players[0]);
    unlocked.OnEmit(unlocks => {
      var saveIds = String.Join(":", unlocks.Select(level => level.SaveId));
      PlayerPrefs.SetString("unlocked", saveIds);
    });

    ShowTitle();
  }

  private void Update () => anim.Update(Time.deltaTime);

  private void ShowTitle () {
    SetScreen(titlePrefab).GetComponent<TitleController>().Init(this);
    level = null;
  }

  // private void ShowDebug () {
  //   SetScreen(debugPrefab).GetComponent<DebugController>().Init(this);
  // }

  private void ShowLevel () {
    var bctrl = SetScreen(levelPrefab).GetComponent<LevelController>();
    bctrl.Init(this);
  }

  private void StartBattle (Battle battle) {
    var battleCtrl = SetScreen(battlePrefab).GetComponent<BattleController>();
    battleCtrl.Init(this, level, battle, ShowLevel);
  }

  private GameObject SetScreen (GameObject prefab) {
    if (screen != null) Destroy(screen);
    screen = Instantiate(prefab, canvas);
    return screen;
  }
}
}
