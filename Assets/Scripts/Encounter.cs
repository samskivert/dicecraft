namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class Encounter {

  public class Fight : Encounter {
    public EnemyData enemy;
  }

  public class Blank : Encounter {}
  public class Shop : Encounter {}
  public class Chest : Encounter {}
  public class Anvil : Encounter {}
  public class Exit : Encounter {}

  public (int, int)[] exits;
}
}
