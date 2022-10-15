namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Series", fileName = "Series")]
public class SeriesData : ScriptableObject {

  public CardData[] cards;
  
  public override string ToString () => name;
}
}
