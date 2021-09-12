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

  public void Init (BoardController owner, int index) {
    onDestroy += owner.board.roll.OnValue(dice => {
      var pips = dice[index];
      if (pips > 0) number.text = pips.ToString();
      die.SetActive(pips > 0);
    });
    button.onClick.AddListener(() => owner.UseDie(index));
  }

  private void OnDestroy () => onDestroy();
}
}
