namespace dicecraft {

using UnityEngine.UI;
using TMPro;

public class CellPopup : Popup {

  public TMP_Text title;
  public Image image;
  public TMP_Text descrip;
  public Button ok;

  protected override Button escapeButton => ok;
  protected override Button returnButton => ok;

  public void Show (CellData cell) {
    switch (cell.type) {
    case Cell.Type.HeartUp:
      title.text = "Max HP Up!";
      descrip.text = "Max HP +2"; // TODO: get from PlayerData
      break;
    case Cell.Type.DiceUp:
      title.text = "Extra Dice Slot!";
      descrip.text = "Dice Slot +1"; // TODO: show 1 -> 2 or something like that
      break;
    }
    image.sprite = cell.image;
    ok.onClick.AddListener(Close);
  }
}
}
