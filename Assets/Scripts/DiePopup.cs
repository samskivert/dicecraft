namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiePopup : Popup {

  public TMP_Text dieName;
  public TMP_Text costLabel;
  public Button close;
  public Button buy;

  public GameObject faces;
  public GameObject diePrefab;

  protected override Button returnButton => buy;
  protected override Button escapeButton => close;

  public void Show (DieData die, Level level = null) {
    if (die == null) {
      dieName.text = "Missing die! Broken level.";
      return;
    }
    dieName.text = die.name;
    foreach (var face in die.faces) Instantiate(diePrefab, faces.transform).
      GetComponent<DieController>().Show(face);
    close.onClick.AddListener(Close);
    if (buy != null) {
      costLabel.text = die.cost.ToString();
      buy.interactable = level.coins.current >= die.cost;
      buy.onClick.AddListener(() => {
        level.BoughtDie(die);
        Close();
      });
    }
  }
}
}
