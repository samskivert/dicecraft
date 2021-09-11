namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class TitleController : MonoBehaviour {

  public GameObject buttons;
  public GameObject buttonPrefab;

  public void Init (GameController game) {
    foreach (var board in game.boards) {
      var buttonObj = Instantiate(buttonPrefab, buttons.transform);
      buttonObj.GetComponent<BoardButtonController>().Init(game, board);
      // TODO: a name for each board
      // TODO: disable boards that are not unlocked yet
    }
  }
}
}
