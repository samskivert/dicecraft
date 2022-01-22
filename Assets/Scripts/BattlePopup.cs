namespace dicecraft {

using System;
using UnityEngine.UI;

public class BattlePopup : Popup {

  public CombatantController enemy;
  public Button close;
  public Button start;

  protected override Button returnButton => start;
  protected override Button escapeButton => close;

  public void Show (GameController game, Battle battle, Action onCancel) {
    enemy.Init(game, battle.enemy, true);
    close.onClick.AddListener(() => {
      onCancel();
      Close();
    });
    start.onClick.AddListener(() => {
      Close();
      game.StartBattle(battle);
    });
  }
}
}
