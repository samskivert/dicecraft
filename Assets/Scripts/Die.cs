namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Die", fileName = "Die")]
public class Die : ScriptableObject {

  public enum Type { Slash, Pierce, Blunt, Shield, Evade, Magic, Heal }

  public Sprite image;
  public DieFace[] faces;

  public override string ToString () => name;
}

public static class DieUtil {

  public static int Boost (this Die.Type type, int upgrades, int amount) {
    switch (type) {
    case Die.Type.Slash:
    case Die.Type.Pierce:
    case Die.Type.Blunt:
    case Die.Type.Heal:
      return amount + ((upgrades > 2) ? 2 : (upgrades > 0) ? 1 : 0);
    case Die.Type.Shield:
      return amount + ((upgrades > 1) ? 1 : 0);
    case Die.Type.Evade:
      return (upgrades > 2) ? amount*3 : (upgrades > 0) ? amount*2 : amount;
    case Die.Type.Magic:
      return amount; // TODO
    default:
      throw new Exception($"Unhandled type: {type}");
    }
  }
}
}
