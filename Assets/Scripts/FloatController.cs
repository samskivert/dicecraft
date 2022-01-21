namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Util;

public class FloatController : MonoBehaviour {
  private RectTransform canvasRt;

  public IconData icons;
  public Sprite hpIcon;
  public Sprite gemIcon;

  public GameController game;
  public Canvas canvas;
  public GameObject floatPrefab;

  public void Float (GameObject over, Effect.Type type, int count) {
    // Debug.Log("Float effect " + type + ": " + count);
    FloatOver(icons.Effect(type), null, count < 0 ? count.ToString() : $"+{count}", over);
  }

  public void Float (GameObject over, Die.Type type, int count) {
    // Debug.Log("Float die " + type + ": " + count);
    FloatOver(icons.Die(type), null, count < 0 ? count.ToString() : $"+{count}", over);
  }

  public void FloatHp (GameObject over, int count) {
    // Debug.Log("Float HP: " + count);
    FloatOver(hpIcon, null, count < 0 ? count.ToString() : $"+{count}", over);
  }

  public void FloatGems (GameObject over, int count) {
    // Debug.Log("Float HP: " + count);
    FloatOver(gemIcon, null, count < 0 ? count.ToString() : $"+{count}", over);
  }

  public void Fling (GameObject fromObj, GameObject toObj, Die.Type type, int count) =>
    Fling(fromObj, toObj, icons.Die(type), null, count);

  public void Fling (GameObject fromObj, GameObject toObj, FaceData face) =>
    Fling(fromObj, toObj, face.image, icons.Die(face.dieType), face.amount);

  public void Fling (GameObject fromObj, GameObject toObj, Sprite sprite, Sprite back, int count) =>
    Fling(sprite, back, count.ToString(), fromObj, toObj);

  private void FloatOver (Sprite sprite, Sprite back, string text, GameObject over) {
    var start = canvasRt.InverseTransformPoint(over.transform.position);
    var finish = start;
    finish.y += 150;
    Tween(sprite, back, text, start, finish, Interps.QuadOut, 250);
  }

  private void Fling (
    Sprite sprite, Sprite back, string text, GameObject fromObj, GameObject toObj
  ) {
    var start = canvasRt.InverseTransformPoint(fromObj.transform.position);
    var finish = canvasRt.InverseTransformPoint(toObj.transform.position);
    Tween(sprite, back, text, start, finish, Interps.QuadIn, 1200);
  }

  private void Tween (
    Sprite sprite, Sprite back, string text,
    Vector3 start, Vector3 finish, Interp interp, float velocity
  ) {
    GameObject obj = null;
    RectTransform rect = null;
    var duration = Vector3.Distance(start, finish) / velocity;
    game.anim.Add(Anim.Serial(
      Anim.Action(() => {
        obj = Instantiate(floatPrefab, canvas.transform);
        rect = obj.GetComponent<RectTransform>();
        var backImage = obj.transform.Find("Back").GetComponent<Image>();
        if (back == null) backImage.enabled = false;
        else backImage.sprite = back;
        obj.transform.Find("Back/Icon").GetComponent<Image>().sprite = sprite;
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
