namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

using React;

public class LevelController : MonoBehaviour {
  private event Action onDestroy;
  private GameController game;

  public TMP_Text gemsLabel;
  public TMP_Text coinsLabel;
  public CellGridController cellGrid;

  public CombatantController player;
  public Image xpMeter;
  public TMP_Text xpMeterLabel;
  public TMP_Text levelLabel;

  public GameObject wonPanel;

  public IMutable<bool> moving = Values.Mutable(false);
  public Level level { get; private set; }

  public void Init (GameController game) {
    this.game = game;
    this.level = game.level;
    cellGrid.Init(game.level, null); // TODO: onClick?
    player.Init(game, game.player);
    onDestroy += game.gems.OnValue(gems => gemsLabel.text = gems.ToString());
    onDestroy += level.coins.OnValue(coins => coinsLabel.text = coins.ToString());
    level.onDied += () => game.ShowLost(level);
    // when the player earns an item, they also get a gem (TODO: separate?)
    game.player.items.OnAdd((ii, item, oitem) => game.Award(1));
  }

  private void Update () {
    if (Input.GetKeyDown(KeyCode.UpArrow)) level.Move(0, -1);
    else if (Input.GetKeyDown(KeyCode.DownArrow)) level.Move(0, 1);
    else if (Input.GetKeyDown(KeyCode.RightArrow)) level.Move(1, 0);
    else if (Input.GetKeyDown(KeyCode.LeftArrow)) level.Move(-1, 0);
  }

  public void ShowCompleted (UnityAction onClick) {
    wonPanel.SetActive(true);
    wonPanel.GetComponentInChildren<Button>().onClick.AddListener(onClick);
    wonPanel.GetComponentInChildren<EarnedGemsController>().Init(level.earnedGems);
  }

  private void OnDestroy () => onDestroy();
}
}
