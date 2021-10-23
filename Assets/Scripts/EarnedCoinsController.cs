namespace dicecraft {

using System;
using UnityEngine;
using TMPro;

public class EarnedCoinsController : MonoBehaviour {

  public TMP_Text coins;

  public void Init (int coins) {
    this.coins.text = $"+{coins}";
  }
}
}
