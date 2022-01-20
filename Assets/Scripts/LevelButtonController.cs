namespace dicecraft {

using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using React;

public class LevelButtonController : MonoBehaviour {
  private Action onDestroy;

  public TMP_Text title;
  public Image lockImage;
  public Image image;
  public Button button;

  public GameObject costPanel;
  public TMP_Text cost;
  public GameObject buyPrefab;

  public void Init (GameController game, LevelData level) {
    title.text = level.name;
    image.sprite = level.image;
    cost.text = level.price.ToString();

    var unlockedV = game.unlocked.ContainsValue(level);
    onDestroy += unlockedV.OnValue(unlocked => {
      costPanel.SetActive(!unlocked);
      lockImage.gameObject.SetActive(!unlocked);
    });
    button.onClick.AddListener(() => {
      if (unlockedV.current) game.SetLevel(level);
      else Instantiate(buyPrefab, game.canvas.transform).
        GetComponent<BuyController>().Show(game, level);
    });
  }

  private void OnDestroy () => onDestroy();
}
}
