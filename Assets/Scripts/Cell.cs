namespace dicecraft {

using UnityEngine;

public static class Cell {

  public enum Type { Wall, Start, Enemy, Chest, Shop, HeartUp, DiceUp }

  public interface Info {
    Type Type { get; }
    Sprite Image { get; }
    bool Walkable { get; }
  }
}
}
