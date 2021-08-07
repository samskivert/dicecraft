namespace dicecraft.Util {

using System;
using System.Collections.Generic;
using Remover = System.Action;
using UnityEngine;
using React;

/// <summary>Plays animations.</summary>
public class AnimPlayer {

  private readonly IMutable<int> _anims = Values.Mutable(0);
  private readonly List<List<AnimFn>> _batches = new List<List<AnimFn>>();
  private List<AnimFn> _active = new List<AnimFn>();
  private int _nextBatchId = 1;
  private int _currentBatchId = 0;

  /// <summary>The current number of animations running.</summary>
  public IValue<int> anims => _anims;

  /// <summary>Emits when the player transitions from >0 running animations to 0.</summary>
  public event Action<AnimPlayer> clear;

  /// <summary>Emits an event when a particular animation batch is finished.</summary>
  public event Action<int> finished;

  /// <summary>Adds an animation to be played.</summary>
  /// Either immediately if there is no active batch, or when the currently accumulating batch is
  /// started.
  /// <returns>A thunk that can be invoked to cancel the animation and remove it from the animator
  /// (whether or not it has already started).</returns>
  public Remover Add (AnimFn anim) {
    var batch = _batches.Count is var bc && bc > 0 ? _batches[bc - 1] : _active;
    batch.Add(anim);
    return () => {
      batch.Remove(anim);
    };
  }

  /// <summary>Starts accumulating new animations to a postponed batch.</summary>
  /// The batch will not start until all currently executing animations are completed and any
  /// previously accumulated batches are also run to completion.
  /// <returns>An id for the batch which will be emitted by `finished` when all the animations in
  /// this batch have completed (or been canceled).</returns>
  public int AddBarrier () {
    if (_batches.Count is var bc && bc > 0 && _batches[bc - 1].Count == 0) return _nextBatchId - 1;
    _batches.Add(new List<AnimFn>());
    return _nextBatchId++;
  }

  /// <summary>Updates the animator every frame.</summary>
  /// This must be called to drive the animation process.
  public void Update (float dt) {
    var anims = _active;
    var count = anims.Count;
    if (count > 0) {
      // TODO: handle removals in the middle of update()
      for (int ii = 0, ll = count; ii < ll; ii += 1) {
        bool remove;
        try {
          remove = anims[ii](dt) <= 0;
        } catch (Exception e) {
          Debug.LogException(e);
          remove = true;
        }
        if (remove) {
          anims.RemoveAt(ii);
          ii -= 1;
          ll -= 1;
        }
      }
      _anims.Update(anims.Count);
      if (anims.Count > 0) return;
      else finished?.Invoke(_currentBatchId);
    }
    if (_batches.Count == 0) clear?.Invoke(this);
    else {
      _active = _batches[0];
      _batches.RemoveAt(0);
      _currentBatchId += 1;
    }
  }
}

}
