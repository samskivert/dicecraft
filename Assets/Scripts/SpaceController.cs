namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class SpaceController : MonoBehaviour {
  private event Action onDestroy;
  private int index;

  public Sprite blankImage;
  public Image image;
  public Image playerImage;

  public BoardController board { get; private set; }

  public void Init (BoardController board, int index, SpaceData space) {
    this.board = board;
    this.index = index;
    image.sprite = space == null ? blankImage : space.image;

    board.game.board.playerPos.OnValue(idx => {
      if (idx == index) playerImage.sprite = board.game.board.player.image;
      playerImage.gameObject.SetActive(idx == index);
    });
  }

  private void OnDestroy () => onDestroy();
}
}
