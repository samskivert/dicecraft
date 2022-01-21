namespace dicecraft {

using UnityEngine;
using TMPro;

public class EarnedGemsController : MonoBehaviour {

  public TMP_Text gems;

  public void Init (int gems) {
    this.gems.text = $"+{gems}";
  }
}
}
