namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Combatant : MonoBehaviour {
  private int maxHp;

  public TMP_Text nameLabel;
  public Image image;
  public TMP_Text hpLabel;
  public Image hpMeter;

  public void Init (string name, Sprite image, int maxHp) {
    nameLabel.text = name;
    this.image.sprite = image;
    this.maxHp = maxHp;
  }

  public void SetHp (int hp) {
    hpLabel.text = $"HP: {hp}/{maxHp}";
    hpMeter.fillAmount = hp / (float)maxHp;
  }
}
}
