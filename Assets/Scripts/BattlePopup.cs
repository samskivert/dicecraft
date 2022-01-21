namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;

public class BattlePopup : MonoBehaviour {

  public CombatantController enemy;
  public Button close;
  public Button start;

  public void Show (GameController game, Battle battle) {
    enemy.Init(game, battle.enemy);
    close.onClick.AddListener(Close);
    start.onClick.AddListener(() => {
      Close();
      game.StartBattle(battle);
    });
  }

  private void Close () => Destroy(gameObject);
}
}
