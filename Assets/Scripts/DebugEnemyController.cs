namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugEnemyController : MonoBehaviour {

  public Image image;
  public TMP_Text nameLabel;
  public TMP_Text hpLabel;
  public GameObject buffs;
  public GameObject buffPrefab;
  public GameObject diePrefab;
  public GameObject gotDiePrefab;

  public void Show (GameObject owner, EnemyData data) {
    nameLabel.text = data.name;
    image.sprite = data.image;
    hpLabel.text = data.maxHp.ToString();

    if (data.resistance != Die.Type.None) {
      var resObj = Instantiate(buffPrefab, buffs.transform);
      resObj.GetComponent<BuffController>().Show(data.resistance, -1);
    }
    if (data.weakness != Die.Type.None) {
      var resObj = Instantiate(buffPrefab, buffs.transform);
      resObj.GetComponent<BuffController>().Show(data.weakness, 1);
    }

    foreach (var die in data.dice) {
      var dieObj = Instantiate(diePrefab, transform);
      dieObj.GetComponent<DieController>().Show(die.faces[0]);
      dieObj.AddComponent<Button>().onClick.AddListener(() => {
        var gotDie = Instantiate(gotDiePrefab, owner.transform.parent);
        gotDie.GetComponent<GotDieController>().Show(die);
      });
    }
  }
}
}
