namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour {
  private readonly System.Random random = new System.Random();
  private Battle battle;
  private bool playerTurn;

  public Player[] players;
  public Enemy[] enemies;

  public CombatantController player;
  public CombatantController enemy;

  public GameObject slotsPanel;
  public GameObject slotPrefab;
  public Button attack;

  public GameObject diePrefab;
  public GameObject dice;
  public GameObject playerDice;
  public GameObject enemyDice;

  public SlotController[] slots { get; private set; }

  private void Start () {
    attack.onClick.AddListener(Attack);
    SetBattle(new Battle(players[random.Next(players.Length)],
                         enemies[random.Next(enemies.Length)]));
  }

  public void SetBattle (Battle battle) {
    this.battle = battle;
    player.Init(battle.player);
    enemy.Init(battle.enemy);
    Roll(true);
  }

  public void UpdateAttack () {
    var canAttack = slots.Any(slot => slot.face != null);
    attack.interactable = canAttack;
  }

  public void Roll (bool playerTurn) {
    this.playerTurn = playerTurn;
    ShowSlots(playerTurn ? battle.player.data : battle.enemy.data);
    if (playerTurn) {
      battle.player.Roll(battle.random);
      ShowDice(playerDice, battle.player.roll, true);
      enemyDice.DestroyChildren();
    } else {
      battle.enemy.Roll(battle.random);
      ShowDice(enemyDice, battle.enemy.roll, false);
      playerDice.DestroyChildren();
    }
    UpdateAttack();
    attack.gameObject.SetActive(playerTurn);
  }

  private void ShowSlots (Combatant.Data data) {
    if (slots != null) for (var ii = slots.Length-1; ii >= 0; ii -= 1) {
      Destroy(slotsPanel.transform.GetChild(ii).gameObject);
    }
    var sidx = 0;
    slots = new SlotController[data.Slots.Length];
    foreach (var type in data.Slots) {
      var slot = Instantiate(slotPrefab, slotsPanel.transform).GetComponent<SlotController>();
      slot.Init(type);
      slots[sidx++] = slot;
    }
    attack.transform.parent.SetAsLastSibling();
  }

  private void ShowDice (GameObject dice, IEnumerable<DieFace> faces, bool clickable) {
    dice.DestroyChildren();
    foreach (var face in faces) {
      var die = Instantiate(diePrefab, dice.transform).GetComponent<DieController>();
      die.Init(this, face, clickable);
    }
  }

  private void Attack () {
    var slots = this.slots.Where(slot => slot.face != null).Select(slot => slot.face);
    var attacker = playerTurn ? battle.player : battle.enemy;
    var defender = playerTurn ? battle.enemy : battle.player;
    battle.Attack(slots, attacker, defender);
    player.Update();
    enemy.Update();
    Roll(!playerTurn);
    if (!playerTurn) this.RunAfter(1, EnemyPlay);
  }

  private void EnemyPlay () {
    var dice = new DieController[enemyDice.transform.childCount];
    for (var ii = 0; ii < dice.Length; ii += 1) {
      dice[ii] = enemyDice.transform.GetChild(ii).GetComponent<DieController>();
    }
    battle.enemy.Play(dice, slots);
    this.RunAfter(1, Attack);
  }
}
}
