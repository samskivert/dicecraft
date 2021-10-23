namespace dicecraft {

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;

public class DieController : MonoBehaviour {
  private BattleController battle;

  public IconData icons;
  public Image above;
  public Image typeBack;
  public Image image;
  public Image below;
  public FaceData face { get; private set; }

  public bool burning { get; private set; }
  public bool frozen { get; private set; }

  public void Init (BattleController battle, FaceData face) {
    this.battle = battle;
    this.face = face;
    typeBack.sprite = icons.Die(face.dieType);
    image.sprite = face.image;
  }

  public void EnableClick (Combatant comb) {
    if (frozen) return;
    var button = gameObject.AddComponent<Button>();
    button.onClick.AddListener(() => Play(comb, true));
  }

  public void SetBurning (bool burning) {
    this.burning = burning;
    // TODO: when we have more than one over effect, set sprite
    above.gameObject.SetActive(burning);
  }

  public void SetFrozen () {
    frozen = true;
    // TODO: when we have more than one over effect, set sprite
    below.gameObject.SetActive(true);
  }

  public void Play (Combatant comb, bool clickable) {
    if (image.sprite == null || frozen) return;

    if (burning) {
      comb.hp.UpdateVia(hp => Math.Max(0, hp-Effect.FireDamage));
      SetBurning(false);
      battle.CheckGameOver();
      return;
    }

    foreach (var slot in battle.slots) {
      if (!slot.CanPlay(face)) continue;
      typeBack.sprite = null;
      image.sprite = null;
      slot.PlayDie(face, clickable, () => {
        typeBack.sprite = icons.Die(face.dieType);
        image.sprite = face.image;
        battle.UpdateAttack();
      });
      battle.UpdateAttack();
      break;
    }
  }
}
}
