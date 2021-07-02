namespace dicecraft {

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour {
  private Battle battle;
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
    SetBattle(new Battle("Woriar", 10, new List<DieFace[]> {
      new [] { faces[0] },
      new [] { faces[0], faces[1], faces[2] },
      new [] { faces[0], faces[1], faces[2], faces[3] },
    }, enemies[0]));
  }

  public void SetBattle (Battle battle) {
    this.battle = battle;
    playerName.text = battle.playerName;
    SetPlayerHP(battle.playerHp);
    enemyName.text = battle.enemy.name;
    enemySprite.sprite = battle.enemy.image;
    SetEnemyHP(battle.enemyHp);
    battle.Roll();
    ShowDice(battle.roll);

  }

  private void SetPlayerHP (int hp) {
    var maxHp = battle.playerMaxHp;
    playerHP.text = $"HP: {hp}/{maxHp}";
    playerHPImage.fillAmount = hp / (float)maxHp;
  }

  private void SetEnemyHP (int hp) {
    var maxHp = battle.enemy.maxHp;
    enemyHP.text = $"HP: {hp}/{maxHp}";
    enemyHPImage.fillAmount = hp / (float)maxHp;
  }

  private void ShowDice (IEnumerable<DieFace> faces) {
    var shown = 0;
    foreach (var face in faces) {
      rolls[shown].sprite = face.image;
      rolls[shown].transform.parent.gameObject.SetActive(true);
      shown += 1;
    }
    for (var ii = shown; ii < rolls.Length; ii += 1) {
      rolls[ii].transform.parent.gameObject.SetActive(false);
    }
  }
}
}
