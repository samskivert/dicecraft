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
  public GameObject gotDiePrefab;
  public CombatantController player;
  public GameObject wonPanel;

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

    onDestroy += board.gotDie.OnEmit(die => {
      var gotDie = Instantiate(gotDiePrefab, transform.parent);
      gotDie.GetComponent<GotDieController>().Show(die);
    });
  }

  public void ShowCompleted (UnityAction onClick) {
    wonPanel.SetActive(true);
    wonPanel.GetComponentInChildren<Button>().onClick.AddListener(onClick);
  }

  private void OnDestroy () => onDestroy();
}
}
