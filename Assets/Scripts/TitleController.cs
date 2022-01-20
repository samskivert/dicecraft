namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleController : MonoBehaviour {
  private event Action onDestroy;

  public GameObject levelButtons;
  public GameObject levelButtonPrefab;
  public GameObject playerButtons;
  public GameObject playerButtonPrefab;
  public Button playButton;
  public TMP_Text coinsLabel;

  public void Init (GameController game) {
    foreach (var level in game.levels) {
      var levelObj = Instantiate(levelButtonPrefab, levelButtons.transform);
      levelObj.GetComponent<LevelButtonController>().Init(game, level);
    }
    foreach (var player in game.players) {
      var playerObj = Instantiate(playerButtonPrefab, playerButtons.transform);
      playerObj.GetComponent<PlayerButtonController>().Init(game, player);
    }

    onDestroy += game.coins.OnValue(coins => {
      coinsLabel.text = coins.ToString();
    });
  }

  private void OnDestroy () => onDestroy();
}
}
