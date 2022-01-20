namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GotItemController : MonoBehaviour {

  public TMP_Text itemName;
  public Image image;
  public Button ok;

  public void Show (ItemData item) {
    itemName.text = item.name;
    image.sprite = item.image;
    // TODO: description
    ok.onClick.AddListener(() => Destroy(gameObject));
  }
}
}
