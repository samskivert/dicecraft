namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;

using React;

public class CellController : MonoBehaviour {
  private event Action onDestroy;

  public Image floorImage;
  public Image thingImage;
  public Image playerImage;

  public void Init (Level level, int index, Sprite floorTile) {
    var cellV = level.cells.GetValue(index);
    onDestroy += cellV.OnValue(Show);
    floorImage.sprite = floorTile;
    level.playerPos.OnValue(pos => {
      playerImage.sprite = (pos == index) ? level.player.Image : null;
      playerImage.gameObject.SetActive(pos == index);
    });
  }

  public void Show (Cell.Info cell) => SetThing(cell?.Image);

  private void SetThing (Sprite sprite) {
    thingImage.sprite = sprite;
    thingImage.gameObject.SetActive(sprite != null);
  }

  private void OnDestroy () => onDestroy?.Invoke();
}
}
