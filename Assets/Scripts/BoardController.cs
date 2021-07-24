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

  public void Init (GameController game) {
    this.game = game;
    var board = game.board;
    var idx = 0;
    foreach (var space in board.data.spaces) spaces[idx].Init(this, idx++, space);

    onDestroy += game.player.coins.OnValue(coins => {
      coinsLabel.text = coins.ToString();
    });

    onDestroy += board.roll.OnValue(dice => {
      var dtx = dicePanel.transform;
      while (dtx.childCount < dice.Length) Instantiate(pipDiePrefab, dtx).
        GetComponent<PipDieController>().Init(this, dtx.childCount-1);
      while (dtx.childCount > dice.Length) Destroy(dtx.GetChild(dtx.childCount-1));
    });
  }

  private void OnDestroy () => onDestroy();
}
}
