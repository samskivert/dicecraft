namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PipDieController : MonoBehaviour {
  private event Action onDestroy;

  public GameObject die;
  public TMP_Text number;
  public Button button;

  public void Init (BoardController owner, int index) {
    // onDestroy += owner.board.roll.OnValue(dice => {
    //   var pips = dice[index];
    //   if (pips > 0) number.text = pips.ToString();
    //   die.SetActive(pips > 0);
    // });
    // button.onClick.AddListener(() => owner.UseDie(index));
    // owner.moving.OnValue(moving => button.interactable = !moving);
  }

  private void OnDestroy () => onDestroy();
}
}
