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

  public readonly AnimPlayer anim = new AnimPlayer();

  public Transform canvas;
  public GameObject titlePrefab;
  public GameObject levelPrefab;
  public GameObject battlePrefab;
  public GameObject lostPopupPrefab;

  public GameObject debugPrefab;
  public Button debugButton;

  public FloatController floater;
  public CellData chest;

  public LevelData[] levels;
  public PlayerData[] players;

  public readonly IMutable<int> gems = Values.Mutable(0);
  public readonly MutableSet<Unlockable> unlocked = RSets.LocalMutable<Unlockable>();

  public Level level { get; private set; }
  public Player player { get; private set; }

  public readonly IMutable<Unlockable> selLevel = Values.Mutable<Unlockable>(null);
  public readonly IMutable<Unlockable> selPlayer = Values.Mutable<Unlockable>(null);

  public void StartLevel () {
    player = new Player((PlayerData)selPlayer.current);
    level = new Level(player, (LevelData)selLevel.current, chest);
    level.battle.OnEmit(StartBattle);
    ShowLevel();
  }

  public void Award (int gemAward) {
    gems.UpdateVia(gems => gems + gemAward);
  }

  public void BuyUnlock (Unlockable unlock) {
    gems.UpdateVia(gems => gems - unlock.Price);
    unlocked.Add(unlock);
  }

  public void ShowLost (Level level) {
    var popObj = Instantiate(lostPopupPrefab, canvas.transform);
    popObj.GetComponentInChildren<EarnedGemsController>().Init(level.earnedGems);
    popObj.GetComponentInChildren<Button>().onClick.AddListener(() => {
      Destroy(popObj);
      ShowTitle();
    });
  }

  private void Start () {
    // sync the player's gems and unlocked levels & players to prefs
    gems.Update(PlayerPrefs.GetInt("gems"));
    gems.OnEmit(gems => PlayerPrefs.SetInt("gems", gems));

    selLevel.Update(levels[0]);
    selPlayer.Update(players[0]);

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
