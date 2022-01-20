namespace dicecraft {

using UnityEngine;

public interface Unlockable {

  string SaveId { get; }
  string Name { get; }
  Sprite Image { get; }
  int Price { get; }
}

}
