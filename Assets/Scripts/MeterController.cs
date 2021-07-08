namespace dicecraft {

using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class MeterController : MonoBehaviour {

  public Image outer;
  public Image meter;
  public TMP_Text label;

  public void AnimateXP (int startXP, int endXP, int maxXP) {
    void ShowXP (int xp) {
      label.text = $"XP: {xp}";
      meter.fillAmount = xp / (float)maxXP;
    }
    ShowXP(startXP);
    IEnumerator Animate () {
      int xp = startXP;
      while (xp < endXP) {
        yield return new WaitForSeconds(0.5f);
        xp += 1;
        ShowXP(xp);
      }
    }
    StartCoroutine(Animate());
  }
}
}
