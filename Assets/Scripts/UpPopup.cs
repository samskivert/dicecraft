namespace dicecraft {

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UpPopup : MonoBehaviour {

  public TMP_Text title;
  public TMP_Text info;
  public Button close;

  public void Show (Cell.Type type) {
    switch (type) {
    case Cell.Type.HeartUp:
      title.text = "Max HP +2!";
      info.text = "Full HP restored.";
      break;
    case Cell.Type.DiceUp:
      title.text = "Dice Slot +1!";
      info.text = "Adds one more Attack per Turn.";
      break;
    }
    close.onClick.AddListener(Close);
    EventSystem.current.SetSelectedGameObject(close.gameObject);
  }

  private void Close () => Destroy(gameObject);
}
}
