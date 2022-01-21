namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour {

  protected virtual Button escapeButton => null;
  protected virtual Button returnButton => null;

  private void Update () {
    void Trigger (Button button) {
      if (button != null) button.onClick.Invoke();
    }
    if (Input.GetKeyDown(KeyCode.Return)) Trigger(returnButton);
    else if (Input.GetKeyDown(KeyCode.Escape)) Trigger(escapeButton);
  }

  protected void Close () => Destroy(gameObject);
}
}
