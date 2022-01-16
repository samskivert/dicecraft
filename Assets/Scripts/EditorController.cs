namespace dicecraft {

using UnityEngine;

public class EditorController : MonoBehaviour {

  public CellGridController cellGrid;

  public PlayerData player;
  public LevelData level; // TEMP

  private void Awake () {
    cellGrid.Init(new Level(new Player(null, this.player), this.level));
  }
}
}
