namespace dicecraft {

using System;

using UnityEngine;
using TMPro;

using React;

public class LevelController : MonoBehaviour {
  private event Action onDestroy;
  private GameController game;

  public TMP_Text gemsLabel;
  public TMP_Text coinsLabel;
  public CellGridController cellGrid;
  public CombatantController player;
  public GameObject buyDiePrefab;
  public GameObject upPrefab;
  public GameObject gotItemPrefab;

  public IMutable<bool> moving = Values.Mutable(false);
  public Level level { get; private set; }

  public void Init (GameController game) {
    this.game = game;
    this.level = game.level;
    cellGrid.Init(game.level, null); // TODO: onClick?
    player.Init(game, game.player);
    onDestroy += game.gems.OnValue(gems => gemsLabel.text = gems.ToString());
    onDestroy += level.coins.OnValue(coins => coinsLabel.text = coins.ToString());
    onDestroy += level.showShop.OnEmit(
      die => game.ShowPopup<DiePopup>(buyDiePrefab).Show(die, level));
    onDestroy += level.gotItem.OnEmit(OnGotItem);
    onDestroy += level.boughtDie.OnEmit(OnBoughtDie);
    onDestroy += level.earnedGems.OnChange((g, og) => {
      var delta = g-og;
      game.Award(delta);
      game.floater.FloatGems(gemsLabel.gameObject, delta);
    });
    level.onExit = () => game.ShowWon(level);

    onDestroy += level.cells.OnRemove((pos, ocell) => {
      switch (ocell.Type) {
      case Cell.Type.HeartUp:
      case Cell.Type.DiceUp:
        game.ShowPopup<UpPopup>(upPrefab).Show(ocell.Type);
        break;
      }
    });
  }

  private void OnGotItem (ItemData item) {
    game.floater.Fling(cellGrid.Cell(level.playerPos.current).gameObject,
                       player.itemBagPanel, item.image, null, () => level.player.AwardItem(item));
    game.ShowPopup<ItemPopup>(gotItemPrefab).Show(item, null);
  }

  private void OnBoughtDie (DieData die) {
    game.floater.Fling(cellGrid.Cell(level.playerPos.current).gameObject,
                       player.diceBagPanel, die.image, null, () => level.player.AddDie(die));
  }

  private void Update () {
    if (Input.GetKeyDown(KeyCode.UpArrow)) level.Move(0, -1);
    else if (Input.GetKeyDown(KeyCode.DownArrow)) level.Move(0, 1);
    else if (Input.GetKeyDown(KeyCode.RightArrow)) level.Move(1, 0);
    else if (Input.GetKeyDown(KeyCode.LeftArrow)) level.Move(-1, 0);
  }

  private void OnDestroy () => onDestroy();
}
}
