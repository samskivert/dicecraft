namespace dicecraft {

using UnityEngine.UI;
using TMPro;

public class CardPopup : Popup {

  public TMP_Text title;
  public Image image;
  public TMP_Text series;
  public TMP_Text number;
  public Button close;

  protected override Button escapeButton => close;
  protected override Button returnButton => close;

  public void Show (SeriesData series, int cardIndex) {
    close.onClick.AddListener(Close);
    var card = series.cards[cardIndex];
    title.text = card.name;
    image.sprite = card.image;
    this.series.text = series.name;
    number.text = $"{cardIndex + 1} / {series.cards.Length}";
  }
}
}
