namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatantController : MonoBehaviour {
  private World world;
  private Combatant comb;

  public TMP_Text nameLabel;
  public Image image;
  public TMP_Text hpLabel;
  public Image hpMeter;
  public TMP_Text shieldLabel;

  public void Init (World world, Combatant comb) {
    this.world = world;
    this.comb = comb;
    nameLabel.text = comb.data.Name;
    image.sprite = comb.data.Image;
    Refresh();
  }

  public void Refresh () {
    var maxHp = comb.data.MaxHp(world);
    hpLabel.text = $"HP: {comb.hp}/{maxHp}";
    hpMeter.fillAmount = comb.hp / (float)maxHp;
    shieldLabel.text = $"Shield: {comb.shield}";
  }
}
}
