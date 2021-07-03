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
  private Slot[] slots;

  public Player[] players;
  public Enemy[] enemies;
  public DieFace[] faces;

  public Combatant player;
  public Combatant enemy;
  public GameObject dice;
  public GameObject diePrefab;

  public GameObject slotsPanel;
  public GameObject slotPrefab;
  public Button attack;

  private void Start () {
    attack.onClick.AddListener(Attack);
    SetBattle(new Battle(players[random.Next(players.Length)],
                         enemies[random.Next(enemies.Length)]));
  }

  public void SetBattle (Battle battle) {
    this.battle = battle;
    player.Init(battle.player.name, battle.player.image, battle.player.maxHp);
    player.SetHp(battle.playerHp);

    var sidx = 0;
    slots = new Slot[battle.player.slots.Length];
    foreach (var type in battle.player.slots) {
      var slot = Instantiate(slotPrefab, slotsPanel.transform).GetComponent<Slot>();
      slot.Init(type);
      slots[sidx++] = slot;
    }
    attack.transform.parent.SetAsLastSibling();

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
