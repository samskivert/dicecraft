namespace dicecraft {

using UnityEngine;

public class BoardController : MonoBehaviour {
  // private event Action onDestroy;
  // private GameController game;

  // public SpaceController[] spaces;
  // public TMP_Text coinsLabel;

  // public GameObject dicePanel;
  // public GameObject pipDiePrefab;
  // public GameObject gotDiePrefab;
  // public GameObject showDiePrefab;

  // public GameObject diceBagPanel;
  // public GameObject bagDiePrefab;

  // public CombatantController player;
  // public Image xpMeter;
  // public TMP_Text xpMeterLabel;
  // public TMP_Text levelLabel;

  // public GameObject wonPanel;

  // public IMutable<bool> moving = Values.Mutable(false);
  // public Board board { get; private set; }

  // public void Init (GameController game) {
  //   this.game = game;
  //   this.board = game.board;
  //   var idx = 0;
  //   foreach (var space in spaces) space.Init(board, idx++);

  //   player.Init(game, board.player);

  //   onDestroy += game.coins.OnValue(coins => {
  //     coinsLabel.text = coins.ToString();
  //   });

  //   onDestroy += board.roll.OnValue(dice => {
  //     var dtx = dicePanel.transform;
  //     while (dtx.childCount < dice.Length) Instantiate(pipDiePrefab, dtx).
  //       GetComponent<PipDieController>().Init(this, dtx.childCount-1);
  //     while (dtx.childCount > dice.Length) Destroy(dtx.GetChild(dtx.childCount-1));
  //   });

  //   void AddBagDie (DieData die) {
  //     var dieObj = Instantiate(bagDiePrefab, diceBagPanel.transform);
  //     dieObj.GetComponent<DieController>().Show(die.faces[0]);
  //     dieObj.SetActive(true);
  //     var button = dieObj.AddComponent<Button>();
  //     button.onClick.AddListener(() => {
  //       Instantiate(showDiePrefab, transform.parent).GetComponent<GotDieController>().Show(die);
  //     });
  //   }

  //   onDestroy += board.gotDie.OnEmit(die => {
  //     Instantiate(gotDiePrefab, transform.parent).GetComponent<GotDieController>().Show(die);
  //     AddBagDie(die);
  //   });

  //   onDestroy += board.player.xp.OnValue(xp => {
  //     xpMeterLabel.text = $"XP: {xp}";
  //     xpMeter.fillAmount = xp / (float)board.player.nextLevelXp;
  //   });
  //   onDestroy += board.player.level.OnValue(level => levelLabel.text = $"Level: {level+1}");

  //   board.onDied += () => game.ShowLost(board);
  //   foreach (var die in board.player.dice) AddBagDie(die);
  // }

  // public void UseDie (int index) {
  //   IEnumerator MovePlayer () {
  //     var moves = board.UseDie(index);
  //     var ppos = board.playerPos.current;
  //     moving.Update(true);
  //     for (var ii = 0; ii < moves; ii += 1) {
  //       ppos = (ppos + 1) % Board.Spots;
  //       board.playerPos.Update(ppos);
  //       yield return new WaitForSeconds(0.5f);
  //     }
  //     board.ProcessSpace(ppos);
  //     moving.Update(false);
  //   }
  //   StartCoroutine(MovePlayer());
  // }

  // public void ShowCompleted (UnityAction onClick) {
  //   wonPanel.SetActive(true);
  //   wonPanel.GetComponentInChildren<Button>().onClick.AddListener(onClick);
  //   wonPanel.GetComponentInChildren<EarnedCoinsController>().Init(board.earnedCoins);
  // }

  // private void OnDestroy () => onDestroy();
}
}
