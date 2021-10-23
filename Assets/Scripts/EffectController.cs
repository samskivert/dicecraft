namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EffectController : MonoBehaviour {

  public IconData icons;

  public Image icon;
  public TMP_Text countLabel;

  public void Show (Effect.Type type, int count) {
    icon.sprite = icons.Effect(type);
    countLabel.text = count.ToString();
  }
}
}
