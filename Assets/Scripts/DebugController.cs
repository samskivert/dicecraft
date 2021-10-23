namespace dicecraft {

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

using React;

public class DebugController : MonoBehaviour {
  private GameController game;

  public TMP_Dropdown boardPicker;

  public GameObject spacesPanel;
  public GameObject spacePrefab;

  public GameObject enemiesPanel;
  public GameObject enemyPrefab;

  public GameObject lootPanel;
  public GameObject lootPrefab;
  public GameObject gotDiePrefab;

  public void Init (GameController game) {
    this.game = game;
    var boardV = boardPicker.PickValues(game.boards, 0, data => data.name);
    boardV.OnValue(ShowBoard);
  }

  private void ShowBoard (BoardData data) {
    spacesPanel.DestroyChildren();
    foreach (var space in data.spaces) {
      var spaceObj = Instantiate(spacePrefab, spacesPanel.transform);
      spaceObj.GetComponent<SpaceController>().Show(space);
    }

    enemiesPanel.DestroyChildren();
    foreach (var enemy in data.enemies) {
      var enemyObj = Instantiate(enemyPrefab, enemiesPanel.transform);
      enemyObj.GetComponent<DebugEnemyController>().Show(this, enemy);
    }

    lootPanel.DestroyChildren();
    foreach (var die in data.loot) {
      var lootObj = Instantiate(lootPrefab, lootPanel.transform);
      var slot = lootObj.GetComponent<SlotController>();
      slot.Show(die.faces[0]);
      slot.typeLabel.text = die.name;
      slot.button.onClick.AddListener(() => {
        var gotDie = Instantiate(gotDiePrefab, transform.parent);
        gotDie.GetComponent<GotDieController>().Show(die);
      });
    }
  }
}
}
