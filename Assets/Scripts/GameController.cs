namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using React;
using Util;

public class GameController : MonoBehaviour {
  private GameObject screen;
  private List<Popup> popups = new List<Popup>();

  public readonly AnimPlayer anim = new AnimPlayer();

  public Transform canvas;
  public GameObject titlePrefab;
  public GameObject levelPrefab;
  public GameObject battlePrefab;
  public GameObject wonPopupPrefab;
  public GameObject lostPopupPrefab;
  public GameObject battlePopupPrefab;
  public GameObject cardPopupPrefab;

  public GameObject debugPrefab;
  public Button debugButton;

  public FloatController floater;
  public CellData chest;
  public CellData shop;

  public LevelData[] levels;
  public PlayerData[] players;
  public SeriesData[] series;

  public readonly IMutable<int> gems = Values.Mutable(0);
  public readonly MutableSet<Unlockable> unlocked = RSets.LocalMutable<Unlockable>();

  public Level level { get; private set; }
  public Player player { get; private set; }

  public readonly IMutable<Unlockable> selLevel = Values.Mutable<Unlockable>(null);
  public readonly IMutable<Unlockable> selPlayer = Values.Mutable<Unlockable>(null);

  public void Award (int gemAward) {
    gems.UpdateVia(gems => gems + gemAward);
  }

  public void BuyUnlock (Unlockable unlock) {
    if (gems.current < unlock.Price) Debug.LogWarning("NSF " + gems.current + " < " + unlock.Price);
    else if (unlocked.Add(unlock)) gems.UpdateVia(gems => gems - unlock.Price);
  }

  public void ShowTitle () {
    SetScreen(titlePrefab).GetComponent<TitleController>().Init(this);
    level = null;
  }

  public void StartLevel () {
    player = new Player((PlayerData)selPlayer.current);
    level = new Level(player, (LevelData)selLevel.current, chest, shop);
    level.onDied = () => ShowLost(level);
    level.battle.OnEmit(battle => ShowPopup<BattlePopup>(battlePopupPrefab).Show(
      this, battle, () => level.playerPos.Update(battle.oldPos)));
    ShowLevel();
  }

  public void StartBattle (Battle battle) {
    var battleCtrl = SetScreen(battlePrefab).GetComponent<BattleController>();
    battleCtrl.Init(this, level, battle, ShowLevel);
  }

  public void ShowWon (Level level) {
    ShowPopup<WonLevelPopup>(wonPopupPrefab).Show(this, level.earnedGems.current);
  }

  public void ShowLost (Level level) {
    var popObj = Instantiate(lostPopupPrefab, canvas.transform);
    popObj.GetComponentInChildren<EarnedGemsController>().Init(level.earnedGems.current);
    popObj.GetComponentInChildren<Button>().onClick.AddListener(() => {
      Destroy(popObj);
      ShowTitle();
    });
  }

  public P ShowPopup<P> (GameObject prefab) where P : Popup {
    var pop = Instantiate(prefab, canvas).GetComponent<P>();
    pop.game = this;
    popups.Add(pop);
    return pop;
  }

  public bool ClearPopups () {
    if (popups.Count == 0) return false;
    for (var ii = popups.Count-1; ii >= 0; ii -= 1) popups[ii].ForceClose();
    return true;
  }

  internal void PopupCleared (Popup popup) {
    popups.Remove(popup);
  }

  private void ResetSaveData () {
    gems.Update(0);
    unlocked.Clear();
    unlocked.Add(levels[0]);
    unlocked.Add(players[0]);
    selLevel.Update(levels[0]);
    selPlayer.Update(players[0]);
  }

  private void Start () {
    // sync the player's gems and unlocked levels & players to prefs
    gems.Update(PlayerPrefs.GetInt("gems"));
    gems.OnEmit(gems => PlayerPrefs.SetInt("gems", gems));

    selLevel.Update(levels[0]);
    selPlayer.Update(players[0]);

    debugButton.gameObject.SetActive(Application.isEditor);
    debugButton.onClick.AddListener(() => {
      // if (screen.GetComponent<DebugController>() != null) {
      //   ShowTitle();
      //   debugButton.GetComponentInChildren<TMP_Text>().text = "Debug";
      // } else {
      //   ShowDebug();
      //   debugButton.GetComponentInChildren<TMP_Text>().text = "Back";
      // }
      // ResetSaveData();
      ShowCardPopup(series[0], 0);
    });

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

  private void ShowCardPopup (SeriesData series, int cardIndex) {
    var popup = ShowPopup<CardPopup>(cardPopupPrefab);
    popup.Show(series, cardIndex);
  }
  
  private void Update () => anim.Update(Time.deltaTime);

  // private void ShowDebug () {
  //   SetScreen(debugPrefab).GetComponent<DebugController>().Init(this);
  // }

  private void ShowLevel () {
    var bctrl = SetScreen(levelPrefab).GetComponent<LevelController>();
    bctrl.Init(this);
  }

  private GameObject SetScreen (GameObject prefab) {
    if (screen != null) Destroy(screen);
    screen = Instantiate(prefab, canvas);
    return screen;
  }
}
}
