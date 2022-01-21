namespace dicecraft {

using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class WonBattlePopup : Popup {

  public TMP_Text coinLabel;
  public Button ok;

  protected override Button escapeButton => ok;
  protected override Button returnButton => ok;

  public void Show (int earnedCoins, UnityAction onWin) {
    coinLabel.text = $"+{earnedCoins}";
    ok.onClick.AddListener(() => {
      Close();
      onWin();
    });
  }
}
}
