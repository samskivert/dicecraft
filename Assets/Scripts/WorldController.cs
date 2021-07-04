namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldController : MonoBehaviour {

  public const int Width = 5;
  public const int Height = 3;

  public Enemy[] enemies;

  public Sprite shopSprite;
  public Sprite anvilSprite;
  public Sprite chestSprite;

  public GameObject nodes;
  public GameObject nodePrefab;
  public GameObject blankPrefab;

  private void Start () {
    ShowWorld(new Dictionary<(int, int), Encounter>{
      { (0, 0), new Encounter.Start {} },
      { (2, 0), new Encounter.Fight { enemy = enemies[1] } },
      { (3, 0), new Encounter.Shop {} },
      { (1, 1), new Encounter.Fight { enemy = enemies[0] } },
      { (3, 1), new Encounter.Fight { enemy = enemies[3] } },
      { (0, 2), new Encounter.Anvil {} },
      { (2, 2), new Encounter.Fight { enemy = enemies[2] } },
      { (3, 2), new Encounter.Chest {} },
    });
  }

  public void ShowWorld (Dictionary<(int, int), Encounter> world) {
    for (var yy = 0; yy < Height; yy += 1) {
      for (var xx = 0; xx < Width; xx += 1) {
        if (world.TryGetValue((xx, yy), out var encounter)) {
          var node = Instantiate(nodePrefab, nodes.transform);
        } else {
          Instantiate(blankPrefab, nodes.transform);
        }
      }
    }
  }
}
}
