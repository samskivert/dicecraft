namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;

public class BoardButtonController : MonoBehaviour {
  private Action onDestroy;

  public TMP_Text title;
  public Image lockImage;
  public Image image;
  public Button button;

  public GameObject costPanel;
  public TMP_Text cost;
  public GameObject buyBoardPrefab;

  public void Init (GameController game, BoardData board) {
    title.text = board.name;
    image.sprite = board.image;
    cost.text = board.price.ToString();

    var unlockedV = game.unlocked.ContainsValue(board);
    onDestroy += unlockedV.OnValue(unlocked => {
      costPanel.SetActive(!unlocked);
      lockImage.gameObject.SetActive(!unlocked);
    });
    button.onClick.AddListener(() => {
      if (unlockedV.current) game.StartBoard(board);
      else Instantiate(buyBoardPrefab, game.canvas.transform).
        GetComponent<BuyBoardController>().Show(game, board);
    });
  }

  private void OnDestroy () => onDestroy();
}
}
