namespace dicecraft {

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;

public class CombatantController : MonoBehaviour {
  private Player player;
  private Combatant comb;
  private Dictionary<Effect.Type, GameObject> effObjs = new Dictionary<Effect.Type, GameObject>();

  public TMP_Text nameLabel;
  public Image image;
  public TMP_Text hpLabel;
  public Image hpMeter;
  public GameObject effects;
  public GameObject effectPrefab;

  public void Init (Player player, Combatant comb) {
    this.player = player;
    this.comb = comb;
    nameLabel.text = comb.data.Name;
    image.sprite = comb.data.Image;

    Values.Join(comb.hp, comb.maxHp).OnValue(pair => {
      hpLabel.text = $"HP: {pair.Item1}/{pair.Item2}";
      hpMeter.fillAmount = pair.Item1 / (float)pair.Item2;
    });

    comb.effects.OnEntries((type, count, ocount) => {
      if (!effObjs.TryGetValue(type, out var effObj)) {
        if (count == 0) return;
        effObj = Instantiate(effectPrefab, effects.transform);
      }
      if (count > 0) effObj.GetComponent<EffectController>().Show(type, count);
      else {
        Destroy(effObj);
        effObjs.Remove(type);
      }
    });
  }
}
}
