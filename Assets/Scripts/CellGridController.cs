namespace dicecraft {

using UnityEngine;

public class CellGridController : MonoBehaviour {
  private CellController[] cells;
  private Level level;

  public GameObject cellPrefab;

  public void Init (Level level) {
    this.level = level;
    cells = new CellController[level.CellCount];
    for (var ii = 0; ii < cells.Length; ii += 1) {
      var cellObj = Instantiate(cellPrefab, transform);
      cells[ii] = cellObj.GetComponent<CellController>();
      cells[ii].Init(level, ii);
    }
  }
}
}
