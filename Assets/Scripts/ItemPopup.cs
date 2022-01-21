namespace dicecraft {

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ItemPopup : MonoBehaviour {

  public TMP_Text itemName;
  public Image image;
  public Button use;
  public Button cancel;

  public void Show (int index, ItemData item, UnityAction onUse) {
    itemName.text = item.name;
    image.sprite = item.image;
    // TODO: description
    if (onUse != null) use.onClick.AddListener(() => {
      onUse();
      Close();
    });
    cancel.onClick.AddListener(Close);
  }

  private void Close () => Destroy(gameObject);
}
}
