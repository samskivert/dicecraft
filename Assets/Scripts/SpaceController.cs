namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

using React;

public class SpaceController : MonoBehaviour {
  private event Action onDestroy;

  public Sprite blankImage;
  public Image image;
  public Image playerImage;

  public void Init (Board board, int index) {
    onDestroy += board.spaces.GetValue(index).OnValue(Show);

    onDestroy += board.playerPos.OnValue(idx => {
      if (idx == index) playerImage.sprite = board.player.data.image;
      playerImage.gameObject.SetActive(idx == index);
    });
  }

  public void Show (SpaceData space) {
    image.sprite = space == null ? blankImage : space.image;
  }

  private void OnDestroy () => onDestroy?.Invoke();
}
}
