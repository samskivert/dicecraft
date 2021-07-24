namespace dicecraft {

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Player", fileName = "Player")]
public class PlayerData : ScriptableObject, Combatant.Data {

  public new string name;
  public Sprite image;
  public int maxHp;
  public Die.Type[] slots;
  public FaceData[] dice1;
  public FaceData[] dice2;

  public string Name => name;
  public Sprite Image => image;
  public int MaxHp (World world) => maxHp + world.playerHpUp;
  public Die.Type[] Slots => slots;
  public FaceData[] Dice1 => dice1;
  public FaceData[] Dice2 => dice2;
  public FaceData[] Dice3 => null;
}
}
