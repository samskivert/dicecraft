namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;

public class SelectedUnlockController : MonoBehaviour {
  private Action onDestroy;

  public TMP_Text title;
  public Image lockImage;
  public Image image;

  public GameObject costPanel;
  public TMP_Text cost;
  public GameObject buyPrefab;
  public Button buyButton;

  public void Init (GameController game, IValue<Unlockable> selected) {
    onDestroy += selected.OnValue(unlock => {
      title.text = unlock.Name;
      image.sprite = unlock.Image;
      cost.text = unlock.Price.ToString();
      buyButton.interactable = game.gems.current >= unlock.Price;
    });

    var unlockedV = selected.SwitchMap(unlock => game.unlocked.ContainsValue(unlock));
    onDestroy += unlockedV.OnValue(unlocked => {
      costPanel.SetActive(!unlocked);
      lockImage.gameObject.SetActive(!unlocked);
    });

    buyButton.onClick.AddListener(() => game.BuyUnlock(selected.current));
  }

  private void OnDestroy () => onDestroy();
}
}
