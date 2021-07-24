namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

public static class Die {

  public enum Type { None, Slash, Pierce, Blunt, Shield, Evade, Magic, Heal }

  public static int Boost (this Type type, int upgrades, int amount) {
    switch (type) {
    case Type.Slash:
    case Type.Pierce:
    case Type.Blunt:
    case Type.Heal:
      return amount + ((upgrades > 2) ? 2 : (upgrades > 0) ? 1 : 0);
    case Type.Shield:
      return amount + ((upgrades > 1) ? 1 : 0);
    case Type.Evade:
      return (upgrades > 2) ? amount*3 : (upgrades > 0) ? amount*2 : amount;
    case Type.Magic:
      return amount; // TODO
    default:
      throw new Exception($"Unhandled type: {type}");
    }
  }
}
}
