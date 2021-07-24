namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class BoardController : MonoBehaviour {
  private event Action onDestroy;

  public SpaceController[] spaces;
  public TMP_Text coinsLabel;

  public GameController game { get; private set; }

  public void Init (GameController game, BoardData board) {
    this.game = game;
    var idx = 0;
    foreach (var space in board.spaces) spaces[idx++].Init(this, space);

    onDestroy += game.world.playerCoins.OnValue(coins => {
      coinsLabel.text = coins.ToString();
    });
  }

  private void OnDestroy () => onDestroy();
}
}
