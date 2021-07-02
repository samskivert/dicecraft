namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Enemy", fileName = "Enemy")]
public class Enemy : ScriptableObject {

  public new string name;
  public Sprite image;
  public int maxHp;
}
}
