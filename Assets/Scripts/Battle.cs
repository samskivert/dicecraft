namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;

using React;

public class Battle {

  public readonly Random random = new Random();
  public readonly Player player;
  public readonly Enemy enemy;

  // flings are (delay, slot idx, player/enemy)
  public readonly Emitter<(float, int, bool)> flings = new Emitter<(float, int, bool)>();

  public readonly Emitter<Battle> barriers = new Emitter<Battle>();

  public Battle (Player player, EnemyData enemyData) {
    this.player = player;
    this.enemy = new Enemy(enemyData);
  }

  public void StartTurn (bool playerTurn) {
    var actor = playerTurn ? (Combatant)player : (Combatant)enemy;
    actor.Roll(random);
    actor.effects.TryGetValue(Effect.Type.Poison, out var poison);
    if (poison > 0) {
      actor.effects[Effect.Type.Poison] = poison-1;
      actor.hp.UpdateVia(hp => Math.Max(0, hp-poison));
    }
  }

  public void Attack (
    IEnumerable<(FaceData, int, int)> dice, Combatant attacker, Combatant defender
  ) {
    var delay = 0f;
    foreach (var pair in dice) {
      var face = pair.Item1;
      var slot = pair.Item2;
      var upgrades = pair.Item3;

      switch (face.dieType) {
      case Die.Type.Slash:
      case Die.Type.Pierce:
      case Die.Type.Blunt:
      case Die.Type.Magic:
        flings.Emit((delay, slot, attacker == enemy));
        barriers.Emit(this);
        if (defender.ApplyDamage(face.dieType, face.amount)) return; // dead
        delay += 1;
        break;
      case Die.Type.Shield:
        flings.Emit((delay, slot, attacker == player));
        barriers.Emit(this);
        delay += 1;
        attacker.AddEffect(Effect.Type.Shield, face.amount);
        break;
      case Die.Type.Evade:
        flings.Emit((delay, slot, attacker == player));
        barriers.Emit(this);
        attacker.AddEffect(Effect.Type.Evade, face.amount);
        break;
      case Die.Type.Heal:
        attacker.Heal(face.amount);
        break;
      }

      // TODO: other effects?
      switch (face.effectType) {
      case Effect.Type.Burn:
      case Effect.Type.Freeze:
      case Effect.Type.Poison:
        defender.AddEffect(face.effectType, 1);
        break;
      }
    }
  }
}

}
