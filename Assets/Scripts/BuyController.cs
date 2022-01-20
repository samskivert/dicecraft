namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;

public class BuyController : MonoBehaviour {

  public TMP_Text title;
  public Image image;
  public TMP_Text costCoins;
  public Button buyButton;
  public Button cancelButton;

  public void Show (GameController game, IMutable<Unlockable> selected, Unlockable unlock) {
    title.text = unlock.Name;
    image.sprite = unlock.Image;
    costCoins.text = unlock.Price.ToString();

    buyButton.interactable = game.coins.current >= unlock.Price;
    buyButton.onClick.AddListener(() => {
      game.BuyUnlock(unlock);
      selected.Update(unlock);
      Destroy(gameObject);
    });
    cancelButton.onClick.AddListener(() => {
      selected.ForceUpdate(selected.current);
      Destroy(gameObject);
    });
  }
}
}
