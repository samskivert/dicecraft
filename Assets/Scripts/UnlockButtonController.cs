namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;

using React;

public class UnlockButtonController : MonoBehaviour {
  private Action onDestroy;

  public Image lockImage;
  public Image image;
  public Button button;

  public void Init (GameController game, IMutable<Unlockable> selected, Unlockable unlock) {
    image.sprite = unlock.Image;
    var unlockedV = game.unlocked.ContainsValue(unlock);
    onDestroy += unlockedV.OnValue(unlocked => lockImage.gameObject.SetActive(!unlocked));
    button.onClick.AddListener(() => selected.Update(unlock));
  }

  private void OnDestroy () => onDestroy();
}
}
