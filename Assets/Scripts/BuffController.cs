namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffController : MonoBehaviour {

  public static readonly string[] upLabels = new [] {
    "▲", "", ""
  };
  public static readonly string[] downLabels = new [] {
    "▼", "", ""
  };

  public IconData icons;

  public Image icon;
  public TMP_Text countLabel;

  public void Show (Die.Type type, int count) {
    icon.sprite = icons.Die(type);
    countLabel.text = count < 0 ? downLabels[-count-1] : upLabels[count-1];
  }
}
}
