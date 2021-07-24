namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DieController : MonoBehaviour {
  private BattleController battle;

  public Image image;
  public FaceData face { get; private set; }

  public void Init (BattleController battle, FaceData face, bool clickable) {
    this.battle = battle;
    this.face = face;
    image.sprite = face.image;
    if (clickable) {
      var button = gameObject.AddComponent<Button>();
      button.onClick.AddListener(() => Play(true));
    }
  }

  public void Play (bool clickable) {
    foreach (var slot in battle.slots) {
      if (!slot.CanPlay(face)) continue;
      image.sprite = null;
      slot.PlayDie(face, clickable, () => {
        image.sprite = face.image;
        battle.UpdateAttack();
      });
      battle.UpdateAttack();
      break;
    }
  }
}
}
