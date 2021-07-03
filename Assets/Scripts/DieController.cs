namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DieController : MonoBehaviour {
  private GameController game;

  public Image image;
  public DieFace face { get; private set; }

  public void Init (GameController game, DieFace face, bool clickable) {
    this.game = game;
    this.face = face;
    image.sprite = face.image;
    if (clickable) {
      var button = gameObject.AddComponent<Button>();
      button.onClick.AddListener(Play);
    }
  }

  public void Play () {
    foreach (var slot in game.slots) {
      if (slot.CanPlay(face)) {
        image.sprite = null;
        slot.PlayDie(face, () => {
          image.sprite = face.image;
          game.UpdateAttack();
        });
        game.UpdateAttack();
      }
    }
  }
}
}
