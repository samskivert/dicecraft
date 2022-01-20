namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class BuyController : MonoBehaviour {

  public TMP_Text levelTitle;
  public TMP_Text title;
  public Image levelImage;
  public Image image;
  public TMP_Text costCoins;
  public Button buyButton;
  public Button cancelButton;

  public void Show (GameController game, Unlockable unlock) {
    levelTitle.text = unlock.Name;
    levelImage.sprite = unlock.Image;
    costCoins.text = unlock.Price.ToString();

    buyButton.interactable = game.coins.current >= unlock.Price;
    buyButton.onClick.AddListener(() => {
      game.BuyUnlock(unlock);
      Destroy(gameObject);
    });
    cancelButton.onClick.AddListener(() => Destroy(gameObject));
  }
}
}
