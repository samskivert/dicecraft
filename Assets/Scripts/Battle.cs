namespace dicecraft {

using System;

using React;

public class Battle {

  public readonly Random random = new Random();
  public readonly Player player;
  public readonly Enemy enemy;
  public readonly int oldPos;

  // flings are (delay, slot idx, player/enemy)
  public readonly Emitter<(float, int, bool)> flings = new Emitter<(float, int, bool)>();

  public readonly Emitter<Battle> barriers = new Emitter<Battle>();

  public Battle (Player player, EnemyData enemyData, int oldPos) {
    this.player = player;
    this.enemy = new Enemy(enemyData);
    this.oldPos = oldPos;
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

  public void Attack (FaceData face, int slot, Combatant attacker, Combatant defender) {
    switch (face.dieType) {
    case Die.Type.Slash:
    case Die.Type.Pierce:
    case Die.Type.Blunt:
    case Die.Type.Magic:
      flings.Emit((0, slot, attacker == enemy));
      barriers.Emit(this);
      if (defender.ApplyDamage(face.dieType, face.amount)) return; // dead
      // TODO: just 1 effect?
      if (face.effectType != Effect.Type.None) defender.AddEffect(face.effectType, 1);
      break;
    case Die.Type.SelfEffect:
      flings.Emit((0, slot, attacker == player));
      barriers.Emit(this);
      attacker.AddEffect(face.effectType, face.amount);
      break;
    case Die.Type.OtherEffect:
      flings.Emit((0, slot, attacker != player));
      barriers.Emit(this);
      defender.AddEffect(face.effectType, face.amount);
      break;
    case Die.Type.Heal:
      attacker.Heal(face.amount);
      break;
    }
  }
}

}
