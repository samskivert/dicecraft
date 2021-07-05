namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour {
  private GameObject screen;

  public Transform canvas;
  public GameObject worldPrefab;
  public GameObject battlePrefab;

  public Player[] players;
  public Enemy[] enemies;

  public World world { get; private set; }

  public void StartBattle ((int, int) coord) {
    // TODO
  }

  private void Start () {
    var world = new World(players[0], enemies);
    var worldScreen = SetScreen(worldPrefab);
    worldScreen.GetComponent<WorldController>().Init(world);
  }

  private GameObject SetScreen (GameObject prefab) {
    if (screen != null) Destroy(screen);
    screen = Instantiate(prefab, canvas);
    return screen;
  }
}
}
