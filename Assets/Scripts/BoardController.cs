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

  public GameObject dicePanel;
  public GameObject pipDiePrefab;

  public GameController game { get; private set; }

  public void Init (GameController game, BoardData board) {
    this.game = game;
    var idx = 0;
    foreach (var space in board.spaces) spaces[idx].Init(this, idx++, space);

    onDestroy += game.board.playerCoins.OnValue(coins => {
      coinsLabel.text = coins.ToString();
    });

    onDestroy += game.board.roll.OnValue(dice => {
      var dtx = dicePanel.transform;
      while (dtx.childCount < dice.Length) Instantiate(pipDiePrefab, dtx).
        GetComponent<PipDieController>().Init(this, dtx.childCount-1);
      while (dtx.childCount > dice.Length) Destroy(dtx.GetChild(dtx.childCount-1));
    });
  }

  private void OnDestroy () => onDestroy();
}
}
