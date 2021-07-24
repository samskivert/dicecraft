namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class PipDieController : MonoBehaviour {
  private event Action onDestroy;

  public GameObject die;
  public TMP_Text number;
  public Button button;

  public void Init (BoardController board, int index) {
    onDestroy += board.game.board.roll.OnValue(dice => {
      var pips = dice[index];
      if (pips > 0) number.text = pips.ToString();
      die.SetActive(pips > 0);
    });
    button.onClick.AddListener(() => board.game.board.UseDie(index));
  }

  private void OnDestroy () => onDestroy();
}
}
