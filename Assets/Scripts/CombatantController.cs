namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatantController : MonoBehaviour {
  private Player player;
  private Combatant comb;

  public TMP_Text nameLabel;
  public Image image;
  public TMP_Text hpLabel;
  public Image hpMeter;
  public TMP_Text shieldLabel;

  public void Init (Player player, Combatant comb) {
    this.player = player;
    this.comb = comb;
    nameLabel.text = comb.data.Name;
    image.sprite = comb.data.Image;
    Refresh();
  }

  public void Refresh () {
    var maxHp = comb.data.MaxHp(player);
    hpLabel.text = $"HP: {comb.hp}/{maxHp}";
    hpMeter.fillAmount = comb.hp / (float)maxHp;
    shieldLabel.text = $"Shield: {comb.shield}";
  }
}
}
