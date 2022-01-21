namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;

public class DiePopup : MonoBehaviour {

  public TMP_Text dieName;
  public TMP_Text costLabel;
  public Button close;
  public Button buy;

  public GameObject faces;
  public GameObject slotPrefab;

  public void Show (DieData die, Level level = null) {
    dieName.text = die.name;
    foreach (var face in die.faces) Instantiate(slotPrefab, faces.transform).
      GetComponent<SlotController>().Show(face);
    close.onClick.AddListener(Close);
    if (buy != null) {
      costLabel.text = die.cost.ToString();
      buy.interactable = level.coins.current >= die.cost;
      buy.onClick.AddListener(() => {
        level.coins.UpdateVia(coins => coins-die.cost);
        level.player.AddDie(die);
        Close();
      });
    }
  }

  private void Close () => Destroy(gameObject);
}
}
