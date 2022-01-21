namespace dicecraft {

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemPopup : MonoBehaviour {

  public TMP_Text itemName;
  public TMP_Text descrip;
  public Image image;
  public Button use;
  public Button cancel;

  public void Show (int index, ItemData item, UnityAction onUse) {
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
    EventSystem.current.SetSelectedGameObject(cancel.gameObject);
  }

  private void Close () => Destroy(gameObject);
}
}
