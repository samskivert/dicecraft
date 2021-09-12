namespace dicecraft {

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

using React;

public class BoardController : MonoBehaviour {
  private event Action onDestroy;

  public SpaceController[] spaces;
  public TMP_Text coinsLabel;

  public GameObject dicePanel;
  public GameObject pipDiePrefab;
  public GameObject gotDiePrefab;
  public CombatantController player;
  public GameObject wonPanel;

  public Board board { get; private set; }

  public void Init (GameController game) {
    this.board = game.board;
    var idx = 0;
    foreach (var space in spaces) space.Init(board, idx++);

    player.Init(game, board.player);

    onDestroy += game.coins.OnValue(coins => {
      coinsLabel.text = coins.ToString();
    });

    onDestroy += board.roll.OnValue(dice => {
      var dtx = dicePanel.transform;
      while (dtx.childCount < dice.Length) Instantiate(pipDiePrefab, dtx).
        GetComponent<PipDieController>().Init(this, dtx.childCount-1);
      while (dtx.childCount > dice.Length) Destroy(dtx.GetChild(dtx.childCount-1));
    });

    onDestroy += board.gotDie.OnEmit(die => {
      var gotDie = Instantiate(gotDiePrefab, transform.parent);
      gotDie.GetComponent<GotDieController>().Show(die);
    });

    board.onDied += () => game.ShowLost();
  }

  public void UseDie (int index) {
    IEnumerator MovePlayer () {
      var moves = board.UseDie(index);
      var ppos = board.playerPos.current;
      for (var ii = 0; ii < moves; ii += 1) {
        ppos = (ppos + 1) % Board.Spots;
        board.playerPos.Update(ppos);
        yield return new WaitForSeconds(0.5f);
      }
      board.ProcessSpace(ppos);
    }
    StartCoroutine(MovePlayer());
  }

  public void ShowCompleted (UnityAction onClick) {
    wonPanel.SetActive(true);
    wonPanel.GetComponentInChildren<Button>().onClick.AddListener(onClick);
  }

  private void OnDestroy () => onDestroy();
}
}
