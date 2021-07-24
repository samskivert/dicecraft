namespace dicecraft {

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using React;

public class Board {

  public const int Width = 7;
  public const int Height = 5;
  public const int Spots = 2 * (Width + Height - 2);

  public static (int, int) Coord (int pos) {
    if (pos < Width-1) return (pos, 0);
    pos -= Width-1;
    if (pos < Height-1) return (Width-1, pos);
    pos -= Height-1;
    if (pos < Width-1) return (Width-1-pos, Height-1);
    pos -= Width-1;
    return (0, Height-1-pos);
  }

  public readonly BoardData data;
  public readonly System.Random random = new System.Random();

  public int dieCount = 2;

  public IMutable<int> playerPos = Values.Mutable(0);
  public IMutable<int[]> roll = Values.Mutable(new int[0]);

  public Board (BoardData data) {
    this.data = data;
  }

  public void Roll () {
    var dice = new int[dieCount];
    for (var ii = 0; ii < dieCount; ii += 1) dice[ii] = random.Next(6)+1;
    roll.Update(dice);
  }

  public void UseDie (int index) {
    var dice = roll.current;
    if (index >= dice.Length) {
      Debug.Log($"Invalid die index {index} (have {dice.Length}).");
      return;
    }
    if (dice[index] < 1) {
      Debug.Log($"Using invalid die ({index}, value {dice[index]}.");
      return;
    }

    dice[index] = -1;
    roll.ForceUpdate(dice);

    var newPos = (playerPos.current + dice[index]) % Spots;
    playerPos.Update(newPos);
    // TODO: actions based on the space they landed on
  }
}
}
