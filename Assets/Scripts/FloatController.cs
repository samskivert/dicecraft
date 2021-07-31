namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatController : MonoBehaviour {
  private const float Duration = 1f;

  private RectTransform ownRect;
  private float elapsed;
  private Vector3 start;
  private Vector3 finish;

  [EnumArray(typeof(Effect.Type))] public Sprite[] effectIcons;
  [EnumArray(typeof(Die.Type))] public Sprite[] dieIcons;
  public Sprite hpIcon;

  public Image icon;
  public TMP_Text countLabel;

  public void Float (GameObject over, Effect.Type type, int count) {
    icon.sprite = effectIcons[(int)type];
    countLabel.text = count < 0 ? count.ToString() : $"+{count}";
    FloatOver(over);
  }

  public void Float (GameObject over, Die.Type type, int count) {
    icon.sprite = dieIcons[(int)type];
    countLabel.text = count < 0 ? count.ToString() : $"+{count}";
    FloatOver(over);
  }

  public void FloatHp (GameObject over, int count) {
    icon.sprite = hpIcon;
    countLabel.text = count < 0 ? count.ToString() : $"+{count}";
    FloatOver(over);
  }

  private void Awake () {
    ownRect = GetComponent<RectTransform>();

  }
  private void FloatOver (GameObject over) {
    start = over.GetComponent<RectTransform>().anchoredPosition;
    finish = start;
    finish.y += 150;
    ownRect.anchoredPosition = start;
  }

  private void Update () {
    elapsed += Time.deltaTime;
    var current = Vector3.Lerp(start, finish, elapsed/Duration);
    ownRect.anchoredPosition = current;
    if (elapsed >= Duration) Destroy(gameObject);
  }

}
}
