namespace dicecraft {

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;

public class DieController : MonoBehaviour {
  private BattleController battle;

  public Sprite empty;
  public IconData icons;
  public TMP_Text nameLabel;
  public TMP_Text descripLabel;
  public Image above;
  public Image typeBack;
  public Image image;
  public Image below;
  public FaceData face { get; private set; }

  public bool burning { get; private set; }
  public bool frozen { get; private set; }

  public bool IsEmpty => image.sprite == empty;
  public bool CanPlay  => !IsEmpty && !frozen;

  public void Init (BattleController battle, FaceData face) {
    this.battle = battle;
    this.face = face;
    Show(face);
  }

  public void Show (FaceData face, bool showNames = true) {
    if (face == null) {
      typeBack.sprite = null;
      image.sprite = empty;
      nameLabel.text = "";
      descripLabel.text = "";
    } else {
      typeBack.sprite = icons.Die(face.dieType);
      image.sprite = face.image;
      nameLabel.text = face.name;
      descripLabel.text = face.Descrip;
    }
    nameLabel.gameObject.SetActive(showNames);
    descripLabel.gameObject.SetActive(showNames);
  }

  public void EnableClick (Combatant comb) {
    if (frozen) return;
    gameObject.AddComponent<Button>().onClick.AddListener(() => {
      if (!CanPlay) return;
      // if it has a status effect that's cleared by clicking, clear it and check for game over
      else if (Clear(comb)) battle.CheckGameOver();
      // otherwise go ahead and try to play it
      else Play(comb);
    });
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

  public bool Clear (Combatant comb) {
    if (burning) {
      comb.hp.UpdateVia(hp => Math.Max(0, hp-Effect.FireDamage));
      SetBurning(false);
      return true;
    }
    return false;
  }

  public bool Play (Combatant comb) {
    foreach (var slot in battle.slots) {
      if (!slot.CanPlay(face)) continue;
      Show(null);
      battle.PlayDie(slot, face);
      return true;
    }
    return false;
  }
}
}
