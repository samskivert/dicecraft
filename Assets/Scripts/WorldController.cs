namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldController : MonoBehaviour {

  public GameObject nodes;
  public GameObject nodePrefab;
  public GameObject blankPrefab;
  public GameObject pathPrefab;

  public void Init (GameController game, World world) {
    var nobjs = new Dictionary<(int, int), GameObject>();

    for (var yy = 0; yy < World.Height; yy += 1) {
      for (var xx = 0; xx < World.Width; xx += 1) {
        var coord = (xx, yy);
        if (world.encounters.TryGetValue(coord, out var encounter)) {
          var nobj = Instantiate(nodePrefab, nodes.transform);
          nobjs.Add(coord, nobj);
          var node = nobj.GetComponent<NodeController>();
          node.Init(world, coord, encounter);
          if (coord == world.playerPos) node.ShowPlayer(world.player);
        } else {
          Instantiate(blankPrefab, nodes.transform);
        }
      }
    }

    this.RunIn(1, () => {
      foreach (var entry in world.encounters) {
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
}
}
