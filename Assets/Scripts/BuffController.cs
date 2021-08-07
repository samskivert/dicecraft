namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffController : MonoBehaviour {

  [EnumArray(typeof(Die.Type))] public Sprite[] dieIcons;

  public Image icon;
  public TMP_Text countLabel;

  public void Show (Die.Type type, int count) {
    icon.sprite = dieIcons[(int)type];
    countLabel.text = count < 0 ? count.ToString() : $"+{count}";
  }
}
}
