namespace dicecraft {

using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class WonController : MonoBehaviour {

  public Image outer;
  public Image meter;
  public TMP_Text coinLabel;
  public TMP_Text meterLabel;
  public TMP_Text levelLabel;
  public TMP_Text rewardLabel;

  public void AnimateXP (Player player, int startXP, int endXP, int maxXP) {
    void ShowXP (int xp) {
      meterLabel.text = $"XP: {xp}";
      meter.fillAmount = xp / (float)maxXP;
    }
    var startLevel = player.level.current;
    levelLabel.text = $"Level: {startLevel+1}";

    IEnumerator Animate (int endXP) {
      int xp = startXP;
      while (xp < endXP) {
        yield return new WaitForSeconds(0.5f);
        xp += 1;
        ShowXP(xp);
      }
    }

    ShowXP(startXP);
    if (endXP < maxXP) {
      StartCoroutine(Animate(endXP));
    } else {
      IEnumerator ShowLevelUp () {
        yield return StartCoroutine(Animate(maxXP));
        levelLabel.text = "Level Up!";
        var reward = player.LevelReward(startLevel);
        if (reward != null) {
          rewardLabel.gameObject.SetActive(true);
          rewardLabel.text = reward;
        }
        ShowXP(0);
        if (endXP > maxXP) {
          yield return Animate(endXP - maxXP);
        }
      }
      StartCoroutine(ShowLevelUp());
    }
  }
}
}
