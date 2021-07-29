namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EffectController : MonoBehaviour {

  [EnumArray(typeof(Effect.Type))] public Sprite[] effectIcons;

  public Image icon;
  public TMP_Text countLabel;

  public void Show (Effect.Type type, int count) {
    icon.sprite = effectIcons[(int)type];
    countLabel.text = count.ToString();
  }
}
}
