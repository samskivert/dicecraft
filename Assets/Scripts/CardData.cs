namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Card", fileName = "Card")]
public class CardData : ScriptableObject {

  public Sprite image;
  
  public override string ToString () => name;
}
}
