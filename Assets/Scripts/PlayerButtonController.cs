namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;

public class PlayerButtonController : MonoBehaviour {
  private Action onDestroy;

  public TMP_Text title;
  public Image lockImage;
  public Image image;
  public Button button;

  public GameObject costPanel;
  public TMP_Text cost;
  public GameObject buyPrefab;

  public void Init (GameController game, PlayerData player) {
    title.text = player.name;
    image.sprite = player.image;
    cost.text = player.price.ToString();

    var unlockedV = game.unlocked.ContainsValue((Unlockable)player);
    onDestroy += unlockedV.OnValue(unlocked => {
      costPanel.SetActive(!unlocked);
      lockImage.gameObject.SetActive(!unlocked);
    });
    button.onClick.AddListener(() => {
      if (unlockedV.current) game.SetPlayer(player);
      else Instantiate(buyPrefab, game.canvas.transform).
        GetComponent<BuyController>().Show(game, player);
    });
  }

  private void OnDestroy () => onDestroy();
}
}
