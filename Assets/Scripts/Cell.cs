namespace dicecraft {

using UnityEngine;

public static class Cell {

  public enum Type { Wall, Entry, Exit, Chest, Shop, HeartUp, DiceUp, Enemy, Gem, CardChest }

  public interface Info {
    Type Type { get; }
    Sprite Image { get; }
    bool Walkable { get; }
  }

  public interface Floor {
    Sprite[] images { get; }
  }
}
}
