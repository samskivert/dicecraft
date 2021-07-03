namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour {
  private readonly System.Random random = new System.Random();
  private Battle battle;

  public Sprite playerSprite;
  public Enemy[] enemies;
  public DieFace[] faces;

  public Combatant player;
  public Combatant enemy;
  public GameObject dice;
  public GameObject diePrefab;

  public Slot[] slots;
  public Button attack;

  private void Start () {
    slots[0].Init(DamageType.Slash);
    slots[1].Init(DamageType.Pierce);
    slots[2].Init(DamageType.Blunt);

    attack.onClick.AddListener(Attack);

    SetBattle(new Battle("Woriar", 10, new List<DieFace[]> {
      new [] { faces[0] },
      new [] { faces[0], faces[1], faces[2] },
      new [] { faces[0], faces[1], faces[2], faces[3] },
    }, enemies[random.Next(enemies.Length)]));
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
    UpdateAttack();
  }

  private void ShowDice (IEnumerable<DieFace> faces) {
    dice.DestroyChildren();
    foreach (var face in faces) {
      var die = Instantiate(diePrefab, dice.transform);
      var image = die.transform.Find("Image").GetComponent<Image>();
      image.sprite = face.image;
      die.GetComponent<Button>().onClick.AddListener(() => PlayDie(face, image));
    }
  }

  private void PlayDie (DieFace face, Image image) {
    foreach (var slot in slots) {
      if (slot.CanPlay(face)) {
        image.sprite = null;
        slot.PlayDie(face, () => {
          image.sprite = face.image;
          UpdateAttack();
        });
        UpdateAttack();
      }
    }
  }

  private void UpdateAttack () {
    var canAttack = slots.Any(slot => slot.face != null);
    attack.interactable = canAttack;
  }

  private void Attack () {
    battle.Attack(slots.Where(slot => slot.face != null).Select(slot => slot.face));
    enemy.SetHp(battle.enemyHp);
    Roll();
  }
}
}
