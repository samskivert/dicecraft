namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatantController : MonoBehaviour {
  private Combatant.Data data;

  public TMP_Text nameLabel;
  public Image image;
  public TMP_Text hpLabel;
  public Image hpMeter;

  public void Init (Combatant.Data data) {
    this.data = data;
    nameLabel.text = data.Name;
    image.sprite = data.Image;
  }

  public void SetHp (int hp) {
    hpLabel.text = $"HP: {hp}/{data.MaxHp}";
    hpMeter.fillAmount = hp / (float)data.MaxHp;
  }
}
}
