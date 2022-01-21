namespace dicecraft {

using System;

using UnityEngine;
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
  public GameObject wonPanel;
  public GameObject buyDiePrefab;
  public GameObject upPrefab;

  public IMutable<bool> moving = Values.Mutable(false);
  public Level level { get; private set; }

  public void Init (GameController game) {
    this.game = game;
    this.level = game.level;
    cellGrid.Init(game.level, null); // TODO: onClick?
    player.Init(game, game.player);
    onDestroy += game.gems.OnValue(gems => gemsLabel.text = gems.ToString());
    onDestroy += level.coins.OnValue(coins => coinsLabel.text = coins.ToString());
    onDestroy += level.shop.OnEmit(
      die => game.ShowPopup<DiePopup>(buyDiePrefab).Show(die, level));
    // when the player earns an item, they also get a gem (TODO: separate?)
    onDestroy += game.player.items.OnAdd((ii, item, oitem) => game.Award(1));
    level.onExit = ShowCompleted;

    onDestroy += level.cells.OnRemove((pos, ocell) => {
      Debug.Log("Cell removed " + ocell.Type);
      switch (ocell.Type) {
      case Cell.Type.HeartUp:
      case Cell.Type.DiceUp:
        game.ShowPopup<UpPopup>(upPrefab).Show(ocell.Type);
        break;
      }
    });
  }

  private void Update () {
    if (Input.GetKeyDown(KeyCode.UpArrow)) level.Move(0, -1);
    else if (Input.GetKeyDown(KeyCode.DownArrow)) level.Move(0, 1);
    else if (Input.GetKeyDown(KeyCode.RightArrow)) level.Move(1, 0);
    else if (Input.GetKeyDown(KeyCode.LeftArrow)) level.Move(-1, 0);
  }

  public void ShowCompleted () {
    wonPanel.SetActive(true);
    wonPanel.GetComponentInChildren<EarnedGemsController>().Init(level.earnedGems);
    wonPanel.GetComponentInChildren<Button>().onClick.AddListener(() => game.ShowTitle());
  }

  private void OnDestroy () => onDestroy();
}
}
