namespace dicecraft {

using UnityEngine.UI;
using TMPro;

public class BuffPopup : Popup {

  public TMP_Text title;
  public BuffController buff;
  public TMP_Text descrip;
  public Button close;

  protected override Button escapeButton => close;
  protected override Button returnButton => close;

  public void Show (Die.Type type, int count) {
    close.onClick.AddListener(Close);
    title.text = count < 0 ? "Resistance" : "Weakness";
    buff.Show(null, type, count);
    descrip.text = count < 0 ? $"{type}: {count}" : $"{type}: +{count}";
  }
}
}
