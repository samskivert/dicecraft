namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotController : MonoBehaviour {

  public Sprite empty;
  public IconData icons;
  public TMP_Text typeLabel;
  public TMP_Text damageLabel;
  public Image typeBack;
  public Image image;

  public FaceData face { get; private set; }
  public int index { get; private set; }

  public bool CanPlay (FaceData face) {
    return this.face == null /* && type == face.dieType */;
  }

  public void Init (int index) {
    this.index = index;
    Reset();
  }

  public void Reset () {
    face = null;
    image.sprite = empty;
    typeBack.sprite = null;
    typeLabel.text = " ";
    damageLabel.text = " ";
  }

  public void Show (FaceData face) {
    this.face = face;
    typeLabel.text = face.name;
    typeBack.sprite = icons.Die(face.dieType);
    image.sprite = face.image;
    damageLabel.text = face.Descrip;
  }
}
}
