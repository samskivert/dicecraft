namespace dicecraft {

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour {
  private Enemy currentEnemy;
  private DieFace[] currentRoll;

  public Enemy[] enemies;
  public DieFace[] faces;

  public TMP_Text playerName;
  public SpriteRenderer playerSprite;
  public TMP_Text playerHP;
  public Image playerHPImage;

  public TMP_Text enemyName;
  public SpriteRenderer enemySprite;
  public TMP_Text enemyHP;
  public Image enemyHPImage;

  public SpriteRenderer[] rolls;

  private void Awake () {
    if (enemies.Length > 0) SetEnemy(enemies[0]); // TEMP
    ShowDice(faces); // TEMP
  }

  public void SetEnemy (Enemy enemy) {
    currentEnemy = enemy;
    enemyName.text = enemy.name;
    enemySprite.sprite = enemy.image;
    SetEnemyHP(enemy.maxHp);
  }

  public void SetEnemyHP (int hp) {
    var maxHp = currentEnemy.maxHp;
    enemyHP.text = $"HP: {hp}/{maxHp}";
    enemyHPImage.fillAmount = hp / (float)maxHp;
  }

  public void ShowDice (DieFace[] faces) {
    currentRoll = faces;
    var shown = Math.Min(faces.Length, rolls.Length);
    for (var ii = 0; ii < shown; ii += 1) {
      rolls[ii].sprite = faces[ii].image;
      rolls[ii].transform.parent.gameObject.SetActive(true);
    }
    for (var ii = shown; ii < rolls.Length; ii += 1) {
      rolls[ii].transform.parent.gameObject.SetActive(false);
    }
  }
}
}
