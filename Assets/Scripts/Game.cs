namespace dicecraft {

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour {
  private Enemy enemy;

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
    if (enemies.Length > 0) SetEnemy(enemies[0]);

    for (var ii = 0; ii < rolls.Length; ii += 1) {
      var face = faces[ii % faces.Length];
      ShowDie(ii, face);
    }
  }

  public void SetEnemy (Enemy enemy) {
    this.enemy = enemy;
    enemyName.text = enemy.name;
    enemySprite.sprite = enemy.image;
    SetEnemyHP(enemy.maxHp);
  }

  public void SetEnemyHP (int hp) {
    enemyHP.text = $"HP: {hp}/{enemy.maxHp}";
    enemyHPImage.fillAmount = hp / (float)enemy.maxHp;
  }

  public void ShowDie (int slot, DieFace face) {
    rolls[slot].sprite = face.image;
  }
}
}
