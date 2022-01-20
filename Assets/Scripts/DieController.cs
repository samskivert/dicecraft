namespace dicecraft {

using System;
using UnityEngine;
using UnityEngine.UI;

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
    Show(face);
  }

  public void Show (FaceData face) {
    typeBack.sprite = face == null ? null : icons.Die(face.dieType);
    image.sprite = face?.image;
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

  public bool Play (Combatant comb, bool clickable) {
    if (image.sprite == null || frozen) return false;

    if (burning) {
      comb.hp.UpdateVia(hp => Math.Max(0, hp-Effect.FireDamage));
      SetBurning(false);
      return battle.CheckGameOver();
    }

    foreach (var slot in battle.slots) {
      if (!slot.CanPlay(face)) continue;
      Show(null);
      slot.PlayDie(face, clickable, () => {
        Show(face);
        battle.UpdateAttack();
      });
      battle.UpdateAttack();
      break;
    }
    // TODO: should we check whether the game ended here too?
    return false;
  }
}
}
