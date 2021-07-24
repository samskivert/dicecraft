namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Board", fileName = "Board")]
public class BoardData : ScriptableObject {

  public Sprite image;
  public SpaceData[] spaces;

  public override string ToString () => name;
}
}
