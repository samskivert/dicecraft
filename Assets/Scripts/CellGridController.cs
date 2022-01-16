namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;

public class CellGridController : MonoBehaviour {
  private CellController[] cells;
  private Level level;

  public GameObject cellPrefab;

  public void Init (Level level, Action<int> onClick = null) {
    this.level = level;
    cells = new CellController[level.CellCount];
    for (var ii = 0; ii < cells.Length; ii += 1) {
      var cellObj = Instantiate(cellPrefab, transform);
      cells[ii] = cellObj.GetComponent<CellController>();
      cells[ii].Init(level, ii);

      if (onClick != null) {
        var index = ii;
        var button = cellObj.AddComponent<Button>();
        button.onClick.AddListener(() => onClick(index));
      }
    }
  }
}
}
