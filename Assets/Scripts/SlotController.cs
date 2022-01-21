namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class SlotController : MonoBehaviour {
  private UnityAction unplay;

  public Sprite empty;
  public IconData icons;
  public TMP_Text typeLabel;
  public TMP_Text damageLabel;
  public Button button;
  public Image typeBack;
  public Image image;

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

  public void Init (int index) {
    this.index = index;
    Reset();
  }

  public void Reset () {
    face = null;
    unplay = null;
    image.sprite = empty;
    typeBack.sprite = null;
    typeLabel.text = " ";
    damageLabel.text = " ";
  }

  public void Show (FaceData face) {
    typeLabel.text = face.name;
    typeBack.sprite = icons.Die(face.dieType);
    image.sprite = face.image;
    damageLabel.text = face.Descrip;
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
