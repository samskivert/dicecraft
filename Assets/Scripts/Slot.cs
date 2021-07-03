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

  public DieType type { get; private set; }
  public DieFace face { get; private set; }

  private void Awake () {
    button.onClick.AddListener(() => {
      if (unplay != null) {
        unplay();
        Reset();
      }
    });
  }

  public bool CanPlay (DieFace face) {
    return unplay == null && type == face.dieType;
  }

  public void Init (DieType type) {
    this.type = type;
    typeLabel.text = type.ToString();
  }

  public void Reset () {
    face = null;
    unplay = null;
    image.sprite = null;
    damageLabel.text = " ";
  }

  public void PlayDie (DieFace face, UnityAction unplay) {
    this.face = face;
    this.unplay = unplay;

    image.sprite = face.image;
    var eff = face.effectType == EffectType.None ? "" : $" {face.effectType}";
    switch (face.dieType) {
    case DieType.Slash:
    case DieType.Pierce:
    case DieType.Blunt:
    case DieType.Magic:
      damageLabel.text = $"{face.amount} {eff}damage";
      break;
    case DieType.Shield:
      damageLabel.text = $"+{face.amount} shield";
      break;
    case DieType.Evade:
      damageLabel.text = $"+{face.amount}% evade";
      break;
    case DieType.Heal:
      damageLabel.text = $"+{face.amount} HP";
      break;
    }

    Debug.Log($"Playing {face} on {type}");
  }
}
}
