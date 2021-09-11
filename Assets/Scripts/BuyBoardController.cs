namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class BuyBoardController : MonoBehaviour {

  public TMP_Text boardTitle;
  public Image boardImage;
  public TMP_Text costCoins;
  public Button buyButton;
  public Button cancelButton;

  public void Show (GameController game, BoardData board) {
    boardTitle.text = board.name;
    boardImage.sprite = board.image;
    costCoins.text = board.price.ToString();

    buyButton.interactable = game.coins.current >= board.price;
    buyButton.onClick.AddListener(() => {
      game.BuyBoard(board);
      Destroy(gameObject);
    });
    cancelButton.onClick.AddListener(() => Destroy(gameObject));
  }
}
}
