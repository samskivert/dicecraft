namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;

public class UnlockButtonController : MonoBehaviour {
  private Action onDestroy;

  public TMP_Text title;
  public Image lockImage;
  public Image image;
  public Toggle toggle;

  public GameObject costPanel;
  public TMP_Text cost;
  public GameObject buyPrefab;

  public void Init (GameController game, IMutable<Unlockable> selected, Unlockable unlock) {
    title.text = unlock.Name;
    image.sprite = unlock.Image;
    cost.text = unlock.Price.ToString();

    var unlockedV = game.unlocked.ContainsValue(unlock);
    onDestroy += unlockedV.OnValue(unlocked => {
      costPanel.SetActive(!unlocked);
      lockImage.gameObject.SetActive(!unlocked);
    });
    toggle.isOn = selected.current == unlock;
    toggle.onValueChanged.AddListener(sel => {
      if (!sel) return;
      else if (unlockedV.current) selected.Update(unlock);
      else Instantiate(buyPrefab, game.canvas.transform).
        GetComponent<BuyController>().Show(game, unlock);
    });
  }

  private void OnDestroy () => onDestroy();
}
}
