namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[CreateAssetMenu(menuName = "Dicecraft/Die", fileName = "Die")]
public class DieData : ScriptableObject {

  public Sprite image;
  public FaceData[] faces;

  public override string ToString () => name;
}
}
