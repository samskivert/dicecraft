namespace dicecraft {

using System;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour {

  protected Action onDestroy;
  internal GameController game;

  protected virtual Button escapeButton => null;
  protected virtual Button returnButton => null;

  public void ForceClose () => Trigger(escapeButton);

  protected void Close () => Destroy(gameObject);

  protected virtual void Awake () {
    onDestroy += () => game.PopupCleared(this);
  }

  private void Update () {
    if (Input.GetKeyDown(KeyCode.Return)) Trigger(returnButton);
    else if (Input.GetKeyDown(KeyCode.Escape)) Trigger(escapeButton);
  }

  private void Trigger (Button button) {
    if (button != null && button.interactable) button.onClick.Invoke();
  }

  protected virtual void OnDestroy () => onDestroy();
}
}
