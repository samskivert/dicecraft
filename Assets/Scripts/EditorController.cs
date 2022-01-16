namespace dicecraft {

using UnityEngine;

public class EditorController : MonoBehaviour {
  private Level _level;

  public CellGridController cellGrid;
  public PaletteController palette;

  public PlayerData player;
  public LevelData level; // TEMP

  private void Awake () {
    _level = new Level(new Player(null, this.player), this.level);
    cellGrid.Init(_level, CellClicked);
  }

  private void CellClicked (int index) {
    var cell = palette.selectedCell.current;
    _level.cells[index] = (Cell.Info)cell;
    level.cells[index] = cell;
  }
}
}
