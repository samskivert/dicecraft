namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

using Util;

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

  public SlotController[] slots { get; private set; }

  private void Start () {
    attack.onClick.AddListener(Attack);
  }

  public void Init (GameController game, Battle battle, UnityAction onWin) {
    this.game = game;
    this.battle = battle;

    wonPanel.SetActive(false);
    wonPanel.GetComponentInChildren<Button>().onClick.AddListener(onWin);

    battle.flings.OnEmit(trip => {
      var delay = trip.Item1;
      var slot = slots[trip.Item2];
      var player = trip.Item3;
      game.floater.Fling(slot.gameObject, (player ? this.player : this.enemy).gameObject,
                         slot.face.image, slot.face.amount);
    });
    battle.barriers.OnEmit(_ => game.anim.AddBarrier());

    slotsPanel.SetActive(true);
    player.Init(game, battle.player);
    enemy.Init(game, battle.enemy);
    StartTurn(true);
  }

  public void UpdateAttack () {
    var canAttack = slots.Any(slot => slot.face != null);
    attack.interactable = canAttack;
  }

  public void StartTurn (bool playerTurn) {
    this.playerTurn = playerTurn;
    ShowSlots(playerTurn ? battle.player.Slots : battle.enemy.Slots);
    battle.StartTurn(playerTurn);
    if (playerTurn) ShowDice(battle.player, playerDice, battle.player.roll, true);
    else ShowDice(battle.enemy, enemyDice, battle.enemy.roll, false);
    // starting the turn may have ended the game due to poison
    if (CheckGameOver()) return;
    UpdateAttack();
    attack.gameObject.SetActive(playerTurn);
    if (!playerTurn) this.RunAfter(1, EnemyPlay);
  }

  public bool CheckGameOver () {
    if (battle.player.hp.current > 0 && battle.enemy.hp.current > 0) return false;
    this.RunAfter(1, EndGame);
    return true;
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
      slot.Init(ii);
      slots[ii] = slot;
    }
    attack.transform.parent.SetAsLastSibling();
  }

  private void ShowDice (
    Combatant comb, GameObject dice, IEnumerable<FaceData> faces, bool clickable
  ) {
    dice.DestroyChildren();
    var added = new List<DieController>();
    foreach (var face in faces) {
      var die = Instantiate(diePrefab, dice.transform).GetComponent<DieController>();
      added.Add(die);
      die.Init(this, face);
    }

    var delay = 0f;
    comb.effects.TryGetValue(Effect.Type.Burn, out var burns);
    for (var ii = 0; ii < burns; ii += 1) {
      if (ii >= added.Count) break;
      var die = added[ii];
      game.anim.Add(Anim.DelayedAction(delay, () => {
        die.SetBurning(true);
        comb.effects.Update(Effect.Type.Burn, c => c-1);
      }));
      delay += 0.5f;
    }
    comb.effects.TryGetValue(Effect.Type.Freeze, out var freezes);
    for (var ii = 0; ii < freezes; ii += 1) {
      if (ii >= added.Count) break;
      var die = added[added.Count-ii-1];
      game.anim.Add(Anim.DelayedAction(delay, () => {
        die.SetFrozen();
        comb.effects.Update(Effect.Type.Freeze, c => c-1);
      }));
      delay += 0.5f;
    }

    game.anim.Add(Anim.DelayedAction(delay, () => {
      if (clickable) foreach (var die in added) die.EnableClick(comb);
      // any leftover burn or freeze are lost
      comb.effects[Effect.Type.Burn] = 0;
      comb.effects[Effect.Type.Freeze] = 0;
    }));
  }

  private void Attack () {
    var slots = this.slots.Where(
      slot => slot.face != null).Select(slot => (slot.face, slot.index, slot.upgrades));
    var attacker = playerTurn ? (Combatant)battle.player : (Combatant)battle.enemy;
    var defender = playerTurn ? (Combatant)battle.enemy : (Combatant)battle.player;
    battle.Attack(slots, attacker, defender);
    ClearSlots();
    playerDice.DestroyChildren();
    enemyDice.DestroyChildren();
    if (!CheckGameOver()) this.RunAfter(2, () => StartTurn(!playerTurn));
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
      var startXp = battle.player.xp.current;
      var endXp = startXp + enemy.xpAward;
      var maxXp = battle.player.nextLevelXp;
      var wonCtrl = wonPanel.GetComponent<WonController>();
      if (enemy.coinAward > 0) wonCtrl.coinLabel.text = $"+{enemy.coinAward}";
      else wonCtrl.coinLabel.gameObject.transform.parent.gameObject.SetActive(false);
      wonCtrl.AnimateXP(battle.player, startXp, endXp, maxXp);
      battle.player.Award(enemy.xpAward);
      game.Award(enemy.coinAward);
    } else game.ShowLost();
  }
}
}
