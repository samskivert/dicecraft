namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatantController : MonoBehaviour {
  private Combatant comb;

  public TMP_Text nameLabel;
  public Image image;
  public TMP_Text hpLabel;
  public Image hpMeter;
  public TMP_Text shieldLabel;

  public void Init (Combatant comb) {
    this.comb = comb;
    nameLabel.text = comb.data.Name;
    image.sprite = comb.data.Image;
    Update();
  }

  public void Update () {
    hpLabel.text = $"HP: {comb.hp}/{comb.data.MaxHp}";
    hpMeter.fillAmount = comb.hp / (float)comb.data.MaxHp;
    shieldLabel.text = $"Shield: {comb.shield}";
  }
}
}
