namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffController : MonoBehaviour {

  public IconData icons;

  public Image icon;
  public TMP_Text countLabel;

  public void Show (Die.Type type, int count) {
    icon.sprite = icons.Die(type);
    countLabel.text = count < 0 ? count.ToString() : $"+{count}";
  }
}
}
