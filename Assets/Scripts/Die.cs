namespace dicecraft {

using System;

public static class Die {

  public enum Type { None, Slash, Pierce, Blunt, SelfEffect, OtherEffect, Magic, Heal }

  public static int Boost (this Type type, int upgrades, int amount) {
    switch (type) {
    case Type.Slash:
    case Type.Pierce:
    case Type.Blunt:
    case Type.Heal:
      return amount + ((upgrades > 2) ? 2 : (upgrades > 0) ? 1 : 0);
    case Type.SelfEffect:
      return amount + ((upgrades > 1) ? 1 : 0);
    case Type.OtherEffect:
      return (upgrades > 2) ? amount*3 : (upgrades > 0) ? amount*2 : amount;
    case Type.Magic:
      return amount; // TODO
    default:
      throw new Exception($"Unhandled type: {type}");
    }
  }

  public static string BuffIcon (this Type type, int count) =>
    count < 0 ? downLabels[-count-1] : upLabels[count-1];
  private static readonly string[] upLabels = new [] {
    "▲", "", ""
  };
  private static readonly string[] downLabels = new [] {
    "▼", "", ""
  };
}
}
