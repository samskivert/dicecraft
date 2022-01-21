namespace dicecraft {

using UnityEngine.UI;
using TMPro;

public class UpPopup : Popup {

  public TMP_Text title;
  public TMP_Text info;
  public Button close;

  protected override Button returnButton => close;
  protected override Button escapeButton => close;

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
  }

  private void Close () => Destroy(gameObject);
}
}
