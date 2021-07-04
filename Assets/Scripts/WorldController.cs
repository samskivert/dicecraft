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

  public Player player;
  public Enemy[] enemies;

  public GameObject nodes;
  public GameObject nodePrefab;
  public GameObject blankPrefab;
  public GameObject pathPrefab;

  private void Start () {
    ShowWorld(new Dictionary<(int, int), Encounter>{
      { (0, 0), new Encounter.Start { exits = Exits((1, 1)) }},
      { (2, 0), new Encounter.Fight { enemy = enemies[1], exits = Exits((3, 0)) }},
      { (3, 0), new Encounter.Shop {} },
      { (1, 1), new Encounter.Fight { enemy = enemies[0], exits = Exits((0, 2), (2, 2), (3, 1)) } },
      { (3, 1), new Encounter.Fight { enemy = enemies[3], exits = Exits((4, 1)) }},
      { (4, 1), new Encounter.Exit {} },
      { (0, 2), new Encounter.Anvil {} },
      { (2, 2), new Encounter.Fight { enemy = enemies[2], exits = Exits((3, 2)) }},
      { (3, 2), new Encounter.Chest {} },
    });
  }

  public void ShowWorld (Dictionary<(int, int), Encounter> world) {
    var nobjs = new Dictionary<(int, int), GameObject>();

    for (var yy = 0; yy < Height; yy += 1) {
      for (var xx = 0; xx < Width; xx += 1) {
        if (world.TryGetValue((xx, yy), out var encounter)) {
          var nobj = Instantiate(nodePrefab, nodes.transform);
          nobj.GetComponent<NodeController>().Init(this, encounter);
          nobjs.Add((xx, yy), nobj);
        } else {
          Instantiate(blankPrefab, nodes.transform);
        }
      }
    }
  }

  private (int, int)[] Exits (params (int, int)[] exits) => exits;
}
}
