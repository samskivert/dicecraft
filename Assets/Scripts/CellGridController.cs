namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;

public class CellGridController : MonoBehaviour {
  private CellController[] cells;
  private Level level;

  public GameObject cellPrefab;

  public CellController Cell (int pos) => cells[pos];

  public void Init (Level level, Action<int> onClick = null) {
    if (cells != null) {
      foreach (var cell in cells) Destroy(cell.gameObject);
    }

    this.level = level;
    cells = new CellController[level.CellCount];
    var rando = new System.Random(); // TODO: seed?
    for (var ii = 0; ii < cells.Length; ii += 1) {
      var cellObj = Instantiate(cellPrefab, transform);
      cells[ii] = cellObj.GetComponent<CellController>();
      var floorTile = rando.Pick(level.data.floorTiles);
      cells[ii].Init(level, ii, floorTile);

      if (onClick != null) {
        var index = ii;
        var button = cellObj.AddComponent<Button>();
        button.onClick.AddListener(() => onClick(index));
      }
    }
  }
}
}
