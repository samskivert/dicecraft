namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NodeController : MonoBehaviour {

  public Sprite currentSprite;
  public Image platform;

  public Sprite shopSprite;
  public Sprite anvilSprite;
  public Sprite chestSprite;
  public Sprite exitSprite;
  public Image image;

  public Button button;

  public void ShowEncounter (Encounter encounter) {
    switch (encounter) {
    case Encounter.Fight fight: image.sprite = fight.enemy.image; break;
    case Encounter.Shop shop:   image.sprite = shopSprite; break;
    case Encounter.Chest chest: image.sprite = chestSprite; break;
    case Encounter.Anvil anvil: image.sprite = anvilSprite; break;
    case Encounter.Exit exit:   image.sprite = exitSprite; break;
    case Encounter.Blank blank: image.gameObject.SetActive(false); break;
    }
  }

  public void ShowPlayer (Player player) {
    platform.sprite = currentSprite;
    image.sprite = player.image;
  }
}
}
