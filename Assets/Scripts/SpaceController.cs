namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class SpaceController : MonoBehaviour {
  // private event Action onDestroy;

  public Sprite blankImage;
  public Image image;

  public BoardController board { get; private set; }

  public void Init (BoardController board, SpaceData space) {
    this.board = board;

    image.sprite = space == null ? blankImage : space.image;
  }

  // private void OnDestroy () => onDestroy();
}
}
