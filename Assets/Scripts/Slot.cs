namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour {

  public TMP_Text typeLabel;
  public Image image;
  public TMP_Text damageLabel;

  public DamageType type { get; private set; }

  public void Init (DamageType type) {
    this.type = type;
    typeLabel.text = type.ToString();
  }

  public void Reset () {
    image.sprite = null;
    damageLabel.text = " ";
  }

  public void SetDie (DieFace face) {
    image.sprite = face.image;
    var eff = face.effectType == EffectType.None ? "" : $" {face.effectType}";
    damageLabel.text = $"{face.damage} {eff}damage";
  }
}
}
