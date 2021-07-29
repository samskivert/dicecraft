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
  public CombatantController player;

  public void Init (GameController game) {
    var board = game.board;
    var idx = 0;
    foreach (var space in spaces) space.Init(board, idx++);

    player.Init(board.player);

    onDestroy += board.player.coins.OnValue(coins => {
      coinsLabel.text = coins.ToString();
    });

    onDestroy += board.roll.OnValue(dice => {
      var dtx = dicePanel.transform;
      while (dtx.childCount < dice.Length) Instantiate(pipDiePrefab, dtx).
        GetComponent<PipDieController>().Init(board, dtx.childCount-1);
      while (dtx.childCount > dice.Length) Destroy(dtx.GetChild(dtx.childCount-1));
    });
  }

  private void OnDestroy () => onDestroy();
}
}
