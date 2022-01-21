namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;

public class TitleController : MonoBehaviour {
  private event Action onDestroy;
  private GameController game;

  public GameObject levelButtons;
  public GameObject playerButtons;
  public GameObject buttonPrefab;
  public Button playButton;
  public TMP_Text gemsLabel;

  public SelectedUnlockController selLevel;
  public SelectedUnlockController selPlayer;

  public void Init (GameController game) {
    this.game = game;
    var levelGroup = levelButtons.GetComponent<ToggleGroup>();
    foreach (var level in game.levels) {
      var levelObj = Instantiate(buttonPrefab, levelButtons.transform);
      var ctrl = levelObj.GetComponent<UnlockButtonController>();
      ctrl.Init(game, game.selLevel, level);
    }
    selLevel.Init(game, game.selLevel);

    var playerGroup = playerButtons.GetComponent<ToggleGroup>();
    foreach (var player in game.players) {
      var playerObj = Instantiate(buttonPrefab, playerButtons.transform);
      var ctrl = playerObj.GetComponent<UnlockButtonController>();
      ctrl.Init(game, game.selPlayer, player);
    }
    selPlayer.Init(game, game.selPlayer);

    onDestroy += Values.Join(game.selLevel, game.selPlayer).OnValue(pair => {
      var unlocked = game.unlocked.Contains(pair.Item1) && game.unlocked.Contains(pair.Item2);
      playButton.interactable = unlocked;
    });

    playButton.onClick.AddListener(game.StartLevel);

    onDestroy += game.gems.OnValue(gems => {
      gemsLabel.text = gems.ToString();
    });
  }

  private void Update () {
    if (Input.GetKeyDown(KeyCode.Return) && playButton.interactable) game.StartLevel();
  }

  private void OnDestroy () => onDestroy();
}
}
