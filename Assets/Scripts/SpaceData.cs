namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Space", fileName = "Space")]
public class SpaceData : ScriptableObject {

  public Sprite image;
  public Space.Type spaceType;
  public int level;

  public override string ToString () => $"{name}/{spaceType}/{level}";
}
}
