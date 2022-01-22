namespace dicecraft {

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Util;

public class BattleController : MonoBehaviour {
  private readonly System.Random random = new System.Random();
  private GameController game;
  private Battle battle;
  private Level level;
  private bool playerTurn;
  private UnityAction onWin;

  public CombatantController player;
  public CombatantController enemy;

  public GameObject slotsPanel;
  public GameObject slotPrefab;
  public Button endTurnButton;

  public GameObject diePrefab;
  public GameObject dice;
  public GameObject playerDice;
  public GameObject enemyDice;

  public float actionDelay = 1f;

  public GameObject wonPopupPrefab;

  public SlotController[] slots { get; private set; }

  public void Init (GameController game, Level level, Battle battle, UnityAction onWin) {
    this.game = game;
    this.level = level;
    this.battle = battle;
    this.onWin = onWin;

    battle.flings.OnEmit(trip => {
      var delay = trip.Item1;
      var slot = slots[trip.Item2];
      var player = trip.Item3;
      game.floater.Fling(slot.gameObject, (player ? this.player : this.enemy).gameObject,
                         slot.face);
    });
    battle.barriers.OnEmit(_ => game.anim.AddBarrier());

    slotsPanel.SetActive(true);
    endTurnButton.onClick.AddListener(EndTurn);
    player.Init(game, battle.player);
    enemy.Init(game, battle.enemy);
    StartTurn(true);
  }

  public void StartTurn (bool playerTurn) {
    this.playerTurn = playerTurn;
    ShowSlots(playerTurn ? battle.player.Slots : battle.enemy.Slots);
    battle.StartTurn(playerTurn);
    endTurnButton.gameObject.SetActive(playerTurn);
    if (playerTurn) ShowDice(battle.player, playerDice, battle.player.roll, true);
    else ShowDice(battle.enemy, enemyDice, battle.enemy.roll, false);
    // starting the turn may have ended the game due to poison
    if (CheckGameOver()) return;
    if (!playerTurn) EnemyPlay();
  }

  public bool CheckGameOver () {
    if (battle.player.hp.current > 0 && battle.enemy.hp.current > 0) return false;
    this.RunAfter(actionDelay, EndGame);
    return true;
  }

  public void PlayDie (SlotController slot, FaceData face) {
    slot.Show(face);
    var attacker = playerTurn ? (Combatant)battle.player : (Combatant)battle.enemy;
    var defender = playerTurn ? (Combatant)battle.enemy : (Combatant)battle.player;
    battle.Attack(face, slot.index, attacker, defender);

    // on the player's turn: if all the slots are full or all the dice are used, auto-end the turn
    if (playerTurn) {
      if (CheckGameOver()) ClearButtons();
      else {
        var filledSlots = slots.Count(slot => slot.face != null);
        if (!slots.Any(slot => slot.face == null) ||
            filledSlots == level.player.dice.Count) EndTurn();
      }
    }
  }

  private void ShowSlots (int slotCount) {
    if (slots != null) foreach (var slot in slots) Destroy(slot.gameObject);
    slots = new SlotController[slotCount];
    for (var ii = 0; ii < slotCount; ii += 1) {
      var slot = Instantiate(slotPrefab, slotsPanel.transform).GetComponent<SlotController>();
      slot.Init(ii);
      slots[ii] = slot;
    }
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

  private void ClearButtons () {
    playerDice.DestroyChildren();
    enemyDice.DestroyChildren();
    endTurnButton.gameObject.SetActive(false);
  }

  private void EndTurn () {
    ClearButtons();
    game.anim.Add(Anim.Action(() => {
      if (!CheckGameOver()) this.RunAfter(actionDelay/2, () => StartTurn(!playerTurn));
    }));
  }

  private void EnemyPlay () {
    var dice = new DieController[enemyDice.transform.childCount];
    for (var ii = 0; ii < dice.Length; ii += 1) {
      dice[ii] = enemyDice.transform.GetChild(ii).GetComponent<DieController>();
    }
    void PlayNext (int index) {
      if (index >= dice.Length) {
        EndTurn();
        return;
      }
      var die = dice[index];
      if (!die.CanPlay) PlayNext(index+1);
      else if (die.Clear(battle.enemy)) {
        if (CheckGameOver()) return;
        else this.RunAfter(actionDelay, () => PlayNext(index));
      } else {
        die.Play(battle.enemy);
        this.RunAfter(actionDelay, () => PlayNext(index+1));
      }
    }
    this.RunAfter(actionDelay, () => PlayNext(0));
  }

  private void EndGame () {
    slotsPanel.SetActive(false);
    endTurnButton.gameObject.SetActive(false);
    if (battle.player.hp.current <= 0) game.ShowLost(level);
    else {
      var enemy = (EnemyData)battle.enemy.data;
      game.ShowPopup<WonBattlePopup>(wonPopupPrefab).Show(enemy.coinAward, onWin);
      level.WonBattle(enemy.coinAward);
    }
  }
}
}
