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
      { (1, 1), new Encounter.Fight { enemy = enemies[0], exits = Exits((0, 2), (2, 0), (2, 2), (3, 1)) } },
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

    this.RunIn(1, () => {
      foreach (var entry in world) {
        var coord = entry.Key;
        var encounter = entry.Value;
        var start = nobjs[coord].transform.localPosition;
        if (encounter.exits != null) foreach (var exit in encounter.exits) {
          var end = nobjs[exit].transform.localPosition;
          var center = start + (end - start)/2;
          var path = Instantiate(pathPrefab, nodes.transform);
          path.transform.SetAsFirstSibling();
          path.transform.localPosition = center;
          var length = Vector3.Distance(start, end);
          var pathrt = path.GetComponent<RectTransform>();
          pathrt.sizeDelta = new Vector3(length, 16); // TODO: get image width?
          var angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;
          pathrt.transform.Rotate(Vector3.forward, angle);
        }
      }
    });
  }

  private (int, int)[] Exits (params (int, int)[] exits) => exits;
}
}
