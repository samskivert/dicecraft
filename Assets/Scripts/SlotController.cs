namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class SlotController : MonoBehaviour {
  private UnityAction unplay;

  public IconData icons;
  public TMP_Text typeLabel;
  public TMP_Text damageLabel;
  public Button button;
  public Image typeBack;
  public Image image;

  // public Die.Type type { get; private set; }
  public FaceData face { get; private set; }
  public int index { get; private set; }
  public int upgrades { get; private set; }

  private void Awake () {
    button.onClick.AddListener(() => {
      if (unplay != null) {
        unplay();
        Reset();
      }
    });
  }

  public bool CanPlay (FaceData face) {
    return unplay == null /* && type == face.dieType */;
  }

  public void Init (int index) { // Die.Type type, int upgrades
    // this.type = type;
    this.index = index;
    // this.upgrades = upgrades;
    typeLabel.text = "..."; // type.ToString();
    Reset();
  }

  public void Reset () {
    face = null;
    unplay = null;
    image.sprite = null;
    typeBack.sprite = null;
    damageLabel.text = " ";
  }

  public void Show (FaceData face) {
    typeLabel.text = face.name;
    typeBack.sprite = icons.Die(face.dieType);
    image.sprite = face.image;
    var amount = face.amount; // type.Boost(upgrades, face.amount);
    var eff = face.effectType == Effect.Type.None ? "" : $"{face.effectType} ";
    switch (face.dieType) {
    case Die.Type.Slash:
    case Die.Type.Pierce:
    case Die.Type.Blunt:
    case Die.Type.Magic:
      damageLabel.text = $"{amount} {eff}damage";
      break;
    case Die.Type.Shield:
      damageLabel.text = $"+{amount} shield";
      break;
    case Die.Type.Evade:
      damageLabel.text = $"+{amount}% evade";
      break;
    case Die.Type.Heal:
      damageLabel.text = $"+{amount} HP";
      break;
    }
  }

  public void PlayDie (FaceData face, bool clickable, UnityAction unplay) {
    this.face = face;
    this.unplay = unplay;
    button.interactable = clickable;

    Show(face);
    // Debug.Log($"Playing {face} on {type}");
  }
}
}
