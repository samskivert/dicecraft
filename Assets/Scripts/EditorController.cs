namespace dicecraft {

using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditorController : MonoBehaviour {
  private LevelData _levelData;
  private Level _level;

  public CellGridController cellGrid;
  public PaletteController palette;
  public TMP_Dropdown levels;
  public Button save;

  public PlayerData player;

  private void Awake () {
    var guids = AssetDatabase.FindAssets("t:LevelData");
    var paths = guids.Select(AssetDatabase.GUIDToAssetPath).ToList();
    var names = paths.Select(path => path.Substring(path.LastIndexOf("/")+1)).ToList();
    levels.ClearOptions();
    levels.AddOptions(names);
    levels.onValueChanged.AddListener(value => SetLevel(paths[value]));
    if (paths.Count > 0) SetLevel(paths[0]);

    save.onClick.AddListener(() => {
      if (_levelData != null) {
        Debug.Log("Saving level: " + _levelData.name);
        AssetDatabase.SaveAssetIfDirty(_levelData);
      }
    });
  }

  private void SetLevel (string path) {
    _levelData = AssetDatabase.LoadAssetAtPath<LevelData>(path);
    _level = new Level(new Player(this.player), _levelData, null);
    cellGrid.Init(_level, CellClicked);
  }

  private void CellClicked (int index) {
    var cell = palette.selectedCell.current;
    _level.cells[index] = (Cell.Info)cell;
    _levelData.cells[index] = cell;
    EditorUtility.SetDirty(_levelData);
  }
}
}
