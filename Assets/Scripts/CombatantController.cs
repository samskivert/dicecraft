namespace dicecraft {

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;

public class CombatantController : MonoBehaviour {
  private event Action onDestroy;

  public TMP_Text nameLabel;
  public Image image;
  public TMP_Text hpLabel;
  public Image hpMeter;
  public GameObject effects;
  public GameObject effectPrefab;

  public void Init (Combatant comb) {
    nameLabel.text = comb.Name;
    image.sprite = comb.Image;

    onDestroy += comb.hp.OnValue(hp => {
      hpLabel.text = $"HP: {hp}/{comb.MaxHp}";
      hpMeter.fillAmount = hp / (float)comb.MaxHp;
    });

    var effObjs = new Dictionary<Effect.Type, GameObject>();
    onDestroy += comb.effects.OnEntries((type, count, ocount) => {
      if (!effObjs.TryGetValue(type, out var effObj)) {
        if (count == 0) return;
        effObjs.Add(type, effObj = Instantiate(effectPrefab, effects.transform));
      }
      if (count > 0) effObj.GetComponent<EffectController>().Show(type, count);
      else {
        Destroy(effObj);
        effObjs.Remove(type);
      }
    });
  }

  private void OnDestroy () => onDestroy();
}
}
