namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Enemy", fileName = "Enemy", order = 1)]
public class Enemy : ScriptableObject {

  public string name;
  public Sprite image;
  public int maxHp;
}
}
