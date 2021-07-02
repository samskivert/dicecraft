namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class Slot : MonoBehaviour {
  private UnityAction unplay;

  public TMP_Text typeLabel;
  public TMP_Text damageLabel;
  public Button button;
  public Image image;

  public DamageType type { get; private set; }

  private void Awake () {
    button.onClick.AddListener(() => {
      if (unplay != null) {
        Reset();
        unplay();
        unplay = null;
      }
    });
  }

  public bool CanPlay (DieFace face) {
    return unplay == null && type == face.damageType;
  }

  public void Init (DamageType type) {
    this.type = type;
    typeLabel.text = type.ToString();
  }

  public void Reset () {
    image.sprite = null;
    damageLabel.text = " ";
  }

  public void PlayDie (DieFace face, UnityAction unplay) {
    image.sprite = face.image;
    var eff = face.effectType == EffectType.None ? "" : $" {face.effectType}";
    damageLabel.text = $"{face.damage} {eff}damage";
    this.unplay = unplay;

    Debug.Log($"Playing {face} on {type}");
  }
}
}
