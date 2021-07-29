namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GotDieController : MonoBehaviour {

  public DieController[] dieFaces;
  public TMP_Text dieName;
  public Button ok;

  public void Show (DieData die) {
    dieName.text = die.name;
    for (var ii = 0; ii < dieFaces.Length; ii += 1) dieFaces[ii].Init(null, die.faces[ii], false);
    ok.onClick.AddListener(() => Destroy(gameObject));
  }
}
}
