namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Util;

public class FloatController : MonoBehaviour {

  private RectTransform canvasRt;

  [EnumArray(typeof(Effect.Type))] public Sprite[] effectIcons;
  [EnumArray(typeof(Die.Type))] public Sprite[] dieIcons;
  public Sprite hpIcon;

  public GameController game;
  public Canvas canvas;
  public GameObject floatPrefab;

  public void Float (GameObject over, Effect.Type type, int count) {
    // Debug.Log("Float effect " + type + ": " + count);
    FloatOver(effectIcons[(int)type], count < 0 ? count.ToString() : $"+{count}", over);
  }

  public void Float (GameObject over, Die.Type type, int count) {
    // Debug.Log("Float die " + type + ": " + count);
    FloatOver(dieIcons[(int)type], count < 0 ? count.ToString() : $"+{count}", over);
  }

  public void FloatHp (GameObject over, int count) {
    // Debug.Log("Float HP: " + count);
    FloatOver(hpIcon, count < 0 ? count.ToString() : $"+{count}", over);
  }

  public void Fling (GameObject fromObj, GameObject toObj, Die.Type type, int count) =>
    Fling(fromObj, toObj, dieIcons[(int)type], count);

  public void Fling (GameObject fromObj, GameObject toObj, Sprite sprite, int count) =>
    Fling(sprite, count.ToString(), fromObj, toObj);

  private void FloatOver (Sprite sprite, string text, GameObject over) {
    var start = canvasRt.InverseTransformPoint(over.transform.position);
    var finish = start;
    finish.y += 150;
    Tween(sprite, text, start, finish, Interps.QuadOut, 250);
  }

  private void Fling (Sprite sprite, string text, GameObject fromObj, GameObject toObj) {
    var start = canvasRt.InverseTransformPoint(fromObj.transform.position);
    var finish = canvasRt.InverseTransformPoint(toObj.transform.position);
    Tween(sprite, text, start, finish, Interps.QuadIn, 1200);
  }

  private void Tween (
    Sprite sprite, string text, Vector3 start, Vector3 finish, Interp interp, float velocity
  ) {
    GameObject obj = null;
    RectTransform rect = null;
    var duration = Vector3.Distance(start, finish) / velocity;
    game.anim.Add(Anim.Serial(
      Anim.Action(() => {
        obj = Instantiate(floatPrefab, canvas.transform);
        rect = obj.GetComponent<RectTransform>();
        obj.transform.Find("Icon").GetComponent<Image>().sprite = sprite;
        obj.transform.Find("Count").GetComponent<TMP_Text>().text = text;
        rect.anchoredPosition = start;
      }),
      Anim.TweenVector3(pos => rect.anchoredPosition = pos, start, finish, duration, interp),
      Anim.Action(() => Destroy(obj))));
    game.anim.AddBarrier();
  }

  private void Start () {
    canvasRt = canvas.GetComponent<RectTransform>();
  }
}
}
