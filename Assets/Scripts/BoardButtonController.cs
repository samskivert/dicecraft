namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class BoardButtonController : MonoBehaviour {

  public TMP_Text title;
  public Image lockImage;
  public Image image;
  public Button button;

  public GameObject costPanel;
  public TMP_Text cost;

  public void Init (GameController game, BoardData board) {
    title.text = board.name;
    image.sprite = board.image;
    cost.text = board.price.ToString();

    if (game.unlocked.Contains(board)) {
      costPanel.SetActive(false);
      button.onClick.AddListener(() => game.StartBoard(board));
      lockImage.gameObject.SetActive(false);
    } else {
      // TODO: show unlock popup
      // button.onClick.AddListener(() => game.StartBoard(board));
    }
  }
}
}
