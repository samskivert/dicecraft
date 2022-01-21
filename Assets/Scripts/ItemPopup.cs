namespace dicecraft {

using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ItemPopup : Popup {

  public TMP_Text itemName;
  public TMP_Text descrip;
  public Image image;
  public Button use;
  public Button cancel;

  protected override Button escapeButton => cancel;
  protected override Button returnButton => use ?? cancel;

  public void Show (ItemData item, UnityAction onUse) {
    if (item == null) {
      itemName.text = "Missing item! Broken level.";
      return;
    }
    itemName.text = item.name;
    descrip.text = item.Descrip;
    image.sprite = item.image;
    // TODO: description
    if (onUse != null) use.onClick.AddListener(() => {
      onUse();
      Close();
    });
    cancel.onClick.AddListener(Close);
  }
}
}
