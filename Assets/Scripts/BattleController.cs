namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class BattleController : MonoBehaviour {
  private readonly System.Random random = new System.Random();
  private GameController game;
  private Battle battle;
  private bool playerTurn;

  public CombatantController player;
  public CombatantController enemy;

  public GameObject slotsPanel;
  public GameObject slotPrefab;
  public Button attack;

  public GameObject diePrefab;
  public GameObject dice;
  public GameObject playerDice;
  public GameObject enemyDice;

  public GameObject wonPanel;
  public GameObject lostPanel;

  public SlotController[] slots { get; private set; }

  private void Start () {
    attack.onClick.AddListener(Attack);
  }

  public void Init (GameController game, Battle battle, UnityAction onWin, UnityAction onLose) {
    this.game = game;
    this.battle = battle;

    wonPanel.SetActive(false);
    wonPanel.GetComponentInChildren<Button>().onClick.AddListener(onWin);

    lostPanel.SetActive(false);
    lostPanel.GetComponentInChildren<Button>().onClick.AddListener(onLose);

    slotsPanel.SetActive(true);
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
    ShowSlots(playerTurn ? battle.player.Slots : battle.enemy.Slots);
    if (playerTurn) {
      battle.player.Roll(battle.random);
      ShowDice(playerDice, battle.player.roll, true);
    } else {
      battle.enemy.Roll(battle.random);
      ShowDice(enemyDice, battle.enemy.roll, false);
    }
    UpdateAttack();
    attack.gameObject.SetActive(playerTurn);
    if (!playerTurn) this.RunAfter(1, EnemyPlay);
  }

  private void ClearSlots () {
    if (slots != null) for (var ii = slots.Length-1; ii >= 0; ii -= 1) {
      Destroy(slotsPanel.transform.GetChild(ii).gameObject);
    }
  }

  private void ShowSlots (int slotCount) {
    slots = new SlotController[slotCount];
    for (var ii = 0; ii < slotCount; ii += 1) {
      var slot = Instantiate(slotPrefab, slotsPanel.transform).GetComponent<SlotController>();
      slot.Init();
      slots[ii] = slot;
    }
    attack.transform.parent.SetAsLastSibling();
  }

  private void ShowDice (GameObject dice, IEnumerable<FaceData> faces, bool clickable) {
    dice.DestroyChildren();
    foreach (var face in faces) {
      var die = Instantiate(diePrefab, dice.transform).GetComponent<DieController>();
      die.Init(this, face, clickable);
    }
  }

  private void Attack () {
    var slots = this.slots.Where(
      slot => slot.face != null).Select(slot => (slot.face, slot.upgrades));
    var attacker = playerTurn ? (Combatant)battle.player : (Combatant)battle.enemy;
    var defender = playerTurn ? (Combatant)battle.enemy : (Combatant)battle.player;
    battle.Attack(slots, attacker, defender);
    ClearSlots();
    playerDice.DestroyChildren();
    enemyDice.DestroyChildren();
    if (battle.player.hp.current == 0 || battle.enemy.hp.current == 0) this.RunAfter(1, EndGame);
    else Roll(!playerTurn);
  }

  private void EnemyPlay () {
    var dice = new DieController[enemyDice.transform.childCount];
    for (var ii = 0; ii < dice.Length; ii += 1) {
      dice[ii] = enemyDice.transform.GetChild(ii).GetComponent<DieController>();
    }
    battle.enemy.Play(dice, slots);
    this.RunAfter(1, Attack);
  }

  private void EndGame () {
    slotsPanel.SetActive(false);
    if (battle.player.hp.current > 0) {
      wonPanel.SetActive(true);
      var enemy = (EnemyData)battle.enemy.data;
      var startXp = game.player.xp.current;
      var endXp = startXp + enemy.xpAward;
      var maxXp = game.player.nextLevelXp;
      wonPanel.GetComponent<WonController>().AnimateXP(
        game.player.level.current, startXp, endXp, maxXp);
      game.player.Award(enemy.xpAward, enemy.coinAward);
    } else lostPanel.SetActive(true);
  }
}
}
