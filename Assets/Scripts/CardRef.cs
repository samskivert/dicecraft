namespace dicecraft {

using UnityEngine;

public class CardRef : Cell.Info {

  public SeriesData series;
  public int cardIndex;
  public CardData Card => series.cards[cardIndex];

  public Cell.Type Type => Cell.Type.CardChest;
  public Sprite Image => Card.image;
  public bool Walkable => true;

  public override string ToString () => $"{Card.name} [{series.name} - {cardIndex}]";
}
}
