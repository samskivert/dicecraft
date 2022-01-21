namespace dicecraft {

using UnityEngine.UI;

public class WonPopup : Popup {

  public EarnedGemsController earned;
  public Button ok;

  protected override Button escapeButton => ok;
  protected override Button returnButton => ok;

  public void Show (GameController game, int earnedGems) {
    earned.Init(earnedGems);
    ok.onClick.AddListener(game.ShowTitle);
  }
}
}
