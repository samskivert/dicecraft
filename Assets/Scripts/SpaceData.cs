namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Space", fileName = "Space")]
public class SpaceData : ScriptableObject {

  public Sprite image;
  public Space.Type spaceType;
  public Die.Type dieType;
  public Effect.Type effectType;
  public int level;

  public override string ToString () => $"{name}/{spaceType}/{dieType}/{effectType}/{level}";
}
}
