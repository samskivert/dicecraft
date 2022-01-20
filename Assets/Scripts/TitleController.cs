namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleController : MonoBehaviour {
  private event Action onDestroy;

  public GameObject levelButtons;
  public GameObject playerButtons;
  public GameObject buttonPrefab;
  public Button playButton;
  public TMP_Text coinsLabel;

  public void Init (GameController game) {
    var levelGroup = levelButtons.GetComponent<ToggleGroup>();
    foreach (var level in game.levels) {
      var levelObj = Instantiate(buttonPrefab, levelButtons.transform);
      var ctrl = levelObj.GetComponent<UnlockButtonController>();
      ctrl.Init(game, level);
      ctrl.toggle.group = levelGroup;
    }

    var playerGroup = levelButtons.GetComponent<ToggleGroup>();
    foreach (var player in game.players) {
      var playerObj = Instantiate(buttonPrefab, playerButtons.transform);
      var ctrl = playerObj.GetComponent<UnlockButtonController>();
      ctrl.Init(game, player);
      ctrl.toggle.group = playerGroup;
    }

    onDestroy += game.coins.OnValue(coins => {
      coinsLabel.text = coins.ToString();
    });
  }

  private void OnDestroy () => onDestroy();
}
}
