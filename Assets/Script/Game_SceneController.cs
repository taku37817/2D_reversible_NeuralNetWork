using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// ゲームシーンクラス
/// </summary>
public class Game_SceneController : MonoBehaviour
{
    static Game_SceneController instance;
    [SerializeField]
    Game_Fild fild;
    [SerializeField]
    Game_Message message;
    int turnNumber;
    Dictionary<Game_Fild.StoneColor,Game_AI_Base> ais = new Dictionary<Game_Fild.StoneColor, Game_AI_Base>();
    public static Game_SceneController Instance
    {
        get { return instance; }
    }

    public bool IsAIturn
    {
        get
        {
            return ais.ContainsKey(CurrentPlayerStoneColor);
        }
    }
    /// <summary>
    /// 今の手番石の色
    /// </summary>
    Game_Fild.StoneColor CurrentPlayerStoneColor
    {
        get { return (Game_Fild.StoneColor)((turnNumber + 1) % 2 + 1); }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameStart();
    }

    public void GameStart()
    {
        turnNumber = 0;

        // AI設定
        ais.Clear();
        ais.Add(Game_Fild.StoneColor.BLACK, new Game_AI_Random(Game_Fild.StoneColor.BLACK));
        //ais.Add(Game_Field.StoneColor.White, new Game_AI_Theory(Game_Field.StoneColor.White));

        fild.Initialize();
        StartCoroutine(NextTurnCoroutine());
    }

    /// <summary>
    /// マスがクリックされた時の処理です
    /// </summary>
    /// <param name="cell">Cell.</param>
    public void OnCellClick(Game_Cell cell)
    {
        fild.Lock();
        cell.stoneColor = Instance.CurrentPlayerStoneColor;
        Game_SoundManeger.Instance.put.Play();
        fild.TurnOverStoneIfPossible(cell);
    }

    /// <summary>
    /// 石をひっくり返し終わった後の処理です
    /// </summary>
    public void OnTurnStoneFinished()
    {
        StartCoroutine(NextTurnCoroutine());
    }

    /// <summary>
    /// 次の手番に移るコルーチンです
    /// </summary>
    /// <returns>The turn coroutine.</returns>
    IEnumerator NextTurnCoroutine()
    {
        if (fild.CountStone(Game_Fild.StoneColor.NONE) == 0)
        {
            // マスが全て埋まったならゲーム終了
            yield return message.Show("GAME FINISHED");
            StartCoroutine(GameFinishedCoroutine());
            yield break;
        }
        else
        {
            IncrementTurnNumber();
            if (fild.CountPuttableCells() == 0)
            {
                // 石を置ける場所が無いならパス
                yield return message.Show(string.Format("{0} cannot put stone.\n TURN SKIPPED", CurrentPlayerStoneColor.ToString()));
                IncrementTurnNumber();
                if (fild.CountPuttableCells() == 0)
                {
                    // もう一方も石を置ける場所が無いならゲーム終了
                    yield return message.Show(string.Format("{0} cannot put stone too. GAME FINISHED", CurrentPlayerStoneColor.ToString()));
                    StartCoroutine(GameFinishedCoroutine());
                    yield break;
                }
            }
        }
        // AIの手番の場合は、1秒待ってから処理を実行
        if (IsAIturn)
        {
            yield return new WaitForSeconds(1f);
            var resultCell = ais[CurrentPlayerStoneColor].GetNextMove(fild);
            OnCellClick(fild.cells.First(x => x.X == resultCell.x && x.Y == resultCell.y));
        }
    }

    /// <summary>
    /// 手番番号をインクリメントし、盤面を更新します
    /// </summary>
    void IncrementTurnNumber()
    {
        turnNumber++;
        fild.UpdateCellsClickable(CurrentPlayerStoneColor);
    }

    /// <summary>
    /// ゲーム終了時のコルーチンです
    /// </summary>
    /// <returns>The finished coroutine.</returns>
    IEnumerator GameFinishedCoroutine()
    {
        var blackCount = fild.CountStone(Game_Fild.StoneColor.BLACK);
        var whiteCount = fild.CountStone(Game_Fild.StoneColor.WHITE);

        // 結果表示
        yield return message.Show(string.Format("{0}\nBlack[{1}] : White[{2}]",
                blackCount > whiteCount ? "Black WIN!!" : (blackCount < whiteCount ? "White WIN!!" : "DRAW"),
                blackCount,
                whiteCount));

        // 表示終了後、次のゲーム開始
        GameStart();
    }

}
