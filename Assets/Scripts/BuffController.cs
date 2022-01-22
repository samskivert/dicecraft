namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffController : MonoBehaviour {

  public IconData icons;
  public Image icon;
  public TMP_Text countLabel;
  public Button button;
  public GameObject buffPopupPrefab;

  public void Show (GameController game, Die.Type type, int count) {
    icon.sprite = icons.Die(type);
    countLabel.text = type.BuffIcon(count);
    if (game != null) button.onClick.AddListener(
      () => game.ShowPopup<BuffPopup>(buffPopupPrefab).Show(type, count));
    else Destroy(button);
  }
}
}
