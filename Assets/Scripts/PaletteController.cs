namespace dicecraft {

using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;

public class PaletteController : MonoBehaviour {

  public GameObject labelPrefab;
  public GameObject cellButtonGridPrefab;
  public GameObject cellButtonPrefab;
  public ToggleGroup group;

  public IMutable<ScriptableObject> selectedCell = Values.Mutable<ScriptableObject>(null);

  private void Awake () {
    void AddHeader (string label) {
      var labelObj = Instantiate(labelPrefab, transform);
      labelObj.GetComponent<TMP_Text>().text = label;
      labelObj.SetActive(true);
    }

    void AddGrid<T> (string label, string query, bool addNone) where T : ScriptableObject {
      AddHeader(label);
      var grid = Instantiate(cellButtonGridPrefab, transform);
      grid.SetActive(true);
      void AddButton (T data) {
        var cell = (Cell.Info)data;
        var buttonObj = Instantiate(cellButtonPrefab, grid.transform);
        var button = buttonObj.GetComponent<CellButtonController>();
        button.image.sprite = cell?.Image;
        button.cell = data;
        button.toggle.group = group;
        button.toggle.onValueChanged.AddListener(sel => {
          if (sel) selectedCell.Update(data);
        });
      }
      if (addNone) AddButton(null);
      var guids = AssetDatabase.FindAssets(query);
      foreach (var path in guids.Select(AssetDatabase.GUIDToAssetPath)) AddButton(
        AssetDatabase.LoadAssetAtPath<T>(path));
    }

    AddGrid<WallData>("Walls", "t:WallData", true);
    AddGrid<EnemyData>("Enemies", "t:EnemyData", false);
  }
}
}
