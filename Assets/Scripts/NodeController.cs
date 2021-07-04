namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeController : MonoBehaviour {

  public Sprite shopSprite;
  public Sprite anvilSprite;
  public Sprite chestSprite;

  public Image image;

  public void Init (Encounter encounter) {
    switch (encounter) {

    case Encounter.Fight fight:
      image.sprite = fight.enemy.image;
      break;
    case Encounter.Shop shop:
      image.sprite = shopSprite;
      break;
    case Encounter.Chest chest:
      image.sprite = chestSprite;
      break;
    case Encounter.Anvil anvil:
      image.sprite = anvilSprite;
      break;
    case Encounter.Start start:
      // TODO
      break;
    }
  }
}
}
