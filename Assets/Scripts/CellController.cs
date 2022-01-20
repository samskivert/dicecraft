namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;

using React;

public class CellController : MonoBehaviour {
  private event Action onDestroy;

  public Image floorImage;
  public Image thingImage;

  public void Init (Level level, int index, Sprite floorTile) {
    onDestroy += level.cells.GetValue(index).OnValue(Show);
    floorImage.sprite = floorTile;
  }

  public void Show (Cell.Info cell) {
    thingImage.sprite = cell == null ? null : cell.Image;
    thingImage.gameObject.SetActive(cell != null);
  }

  private void OnDestroy () => onDestroy?.Invoke();
}
}
