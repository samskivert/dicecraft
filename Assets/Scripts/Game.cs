namespace dicecraft {

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour {
  private Battle battle;

  public Sprite playerSprite;
  public Enemy[] enemies;
  public DieFace[] faces;

  public Combatant player;
  public Combatant enemy;
  public GameObject dice;
  public GameObject diePrefab;

  public Slot[] slots;

  private void Awake () {
    slots[0].Init(DamageType.Slash);
    slots[1].Init(DamageType.Pierce);
    slots[2].Init(DamageType.Blunt);

    SetBattle(new Battle("Woriar", 10, new List<DieFace[]> {
      new [] { faces[0] },
      new [] { faces[0], faces[1], faces[2] },
      new [] { faces[0], faces[1], faces[2], faces[3] },
    }, enemies[1]));
  }

  public void SetBattle (Battle battle) {
    this.battle = battle;
    player.Init(battle.playerName, playerSprite, battle.playerMaxHp);
    player.SetHp(battle.playerHp);
    enemy.Init(battle.enemy.name, battle.enemy.image, battle.enemy.maxHp);
    enemy.SetHp(battle.enemyHp);
    Roll();
  }

  public void Roll () {
    foreach (var slot in slots) slot.Reset();
    battle.Roll();
    ShowDice(battle.roll);
  }

  private void ShowDice (IEnumerable<DieFace> faces) {
    dice.DestroyChildren();
    foreach (var face in faces) {
      var die = Instantiate(diePrefab, dice.transform);
      die.transform.Find("Image").GetComponent<Image>().sprite = face.image;
    }
  }
}
}
