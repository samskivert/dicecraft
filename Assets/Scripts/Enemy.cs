namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Enemy", fileName = "Enemy")]
public class Enemy : ScriptableObject {

  public new string name;
  public Sprite image;
  public int maxHp;
  public DamageType[] slots;
  public DieFace[] dice1;
  public DieFace[] dice2;
  public DieFace[] dice3;
}
}
