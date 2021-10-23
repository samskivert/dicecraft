namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GotDieController : MonoBehaviour {

  public TMP_Text dieName;
  public Button ok;

  public GameObject faces;
  public GameObject slotPrefab;

  public void Show (DieData die) {
    dieName.text = die.name;
    foreach (var face in die.faces) Instantiate(slotPrefab, faces.transform).
      GetComponent<SlotController>().Show(face);
    ok.onClick.AddListener(() => Destroy(gameObject));
  }
}
}
