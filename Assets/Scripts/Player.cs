namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Player", fileName = "Player")]
public class Player : ScriptableObject {

  public new string name;
  public Sprite image;
  public int maxHp;
  public DieType[] slots;
  public DieFace[] dice1;
  public DieFace[] dice2;
}
}
