namespace dicecraft {

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;
using Util;

public class CombatantController : MonoBehaviour {
  private event Action onDestroy;

  public TMP_Text nameLabel;
  public Image image;
  public TMP_Text hpLabel;
  public Image hpMeter;
  public GameObject effects;
  public GameObject effectPrefab;
  public GameObject buffPrefab;

  public GameObject diceBagPanel;
  public GameObject bagDiePrefab;
  public GameObject showDiePrefab;
  public GameObject gotDiePrefab;

  public GameObject itemBagPanel;
  public GameObject bagItemPrefab;
  public GameObject showItemPrefab;
  public GameObject gotItemPrefab;

  public void Init (GameController game, Combatant comb) {
    nameLabel.text = comb.Name;
    image.sprite = comb.Image;

    onDestroy += comb.hp.OnValue(hp => {
      game.anim.Add(Anim.Action(() => {
        hpLabel.text = $"{hp}/{comb.MaxHp}";
        hpMeter.fillAmount = hp / (float)comb.MaxHp;
      }));
    });

    if (comb.Resistance != Die.Type.None) {
      var resObj = Instantiate(buffPrefab, effects.transform);
      resObj.GetComponent<BuffController>().Show(comb.Resistance, -1);
    }
    if (comb.Weakness != Die.Type.None) {
      var resObj = Instantiate(buffPrefab, effects.transform);
      resObj.GetComponent<BuffController>().Show(comb.Weakness, 1);
    }

    foreach (var die in comb.dice) AddBagDie(die);
    onDestroy += comb.gotDie.OnEmit(die => {
      Instantiate(gotDiePrefab, transform.parent).GetComponent<GotDieController>().Show(die);
      AddBagDie(die);
    });

    foreach (var item in comb.items) AddBagItem(item);
    onDestroy += comb.gotItem.OnEmit(item => {
      Instantiate(gotItemPrefab, transform.parent).GetComponent<GotItemController>().Show(item);
      AddBagItem(item);
    });

    onDestroy += comb.effected.OnEmit(
      pair => game.floater.Float(gameObject, pair.Item1, pair.Item2));
    onDestroy += comb.diced.OnEmit(pair => {
      if (pair.Item1 == Die.Type.Heal) return; // don't animate heal; we'll see the HP float
      game.floater.Float(gameObject, pair.Item1, pair.Item2);
    });
    onDestroy += comb.hp.OnChange(
      (hp, oldHp) => game.floater.FloatHp(gameObject, hp-oldHp));

    var effObjs = new Dictionary<Effect.Type, GameObject>();
    onDestroy += comb.effects.OnEntries((type, count, ocount) => {
      game.anim.Add(Anim.Action(() => {
        if (!effObjs.TryGetValue(type, out var effObj)) {
          if (count == 0) return;
          effObjs.Add(type, effObj = Instantiate(effectPrefab, effects.transform));
        }
        if (count > 0) effObj.GetComponent<EffectController>().Show(type, count);
        else {
          Destroy(effObj);
          effObjs.Remove(type);
        }
      }));
    });
  }

    void AddBagDie (DieData die) {
      var dieObj = Instantiate(bagDiePrefab, diceBagPanel.transform);
      dieObj.GetComponent<DieController>().Show(die.faces[0]);
      dieObj.SetActive(true);
      var button = dieObj.AddComponent<Button>();
      button.onClick.AddListener(() => {
        Instantiate(showDiePrefab, transform.parent).GetComponent<GotDieController>().Show(die);
      });
    }

  public void AddBagItem (ItemData item) {
    var itemObj = Instantiate(bagItemPrefab, itemBagPanel.transform);
    itemObj.GetComponent<Image>().sprite = item.image;
    itemObj.SetActive(true);
    var button = itemObj.AddComponent<Button>();
    button.onClick.AddListener(() => {
      Instantiate(showItemPrefab, transform.parent).GetComponent<GotItemController>().Show(item);
    });
  }

  private void OnDestroy () => onDestroy();
}
}
