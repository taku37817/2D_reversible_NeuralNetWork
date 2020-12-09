using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// セオリーで攻めるAI
/// </summary>
public class Game_AI_Theory : Game_AI_Base
{
    /// <summary>
    /// 次の手を取得
    /// </summary>
    /// <param name="gameField"></param>
    /// <returns></returns>
    public override CellInfo GetNextMove(Game_Fild gameField)
    {
        var simulateField = GenerateSimulateFieldWithGameField(gameField);
        var cellInfoPoints = new Dictionary<CellInfo, int>();
        foreach (var cellInfo in simulateField.GetPuttableCellInfos(stoneColor))
        {
            //角を取得するための重み付け
            var point = 0;
            if (cellInfo.IsCorner)
            {
                point += 50;
            }
            //過度の斜め隣に打たないように
            if (cellInfo.IsCornerInside && simulateField.Stage != SimulateField.StageEnum.End)
            {
                point += 30;
            }

            //序盤は少なくとるための重み
            var tempField = GenerateSimulateFieldWithGameField(gameField);
            var turneCellInfos = tempField.PutStone(cellInfo.x, cellInfo.y, stoneColor);
            point -= turneCellInfos.Count * 10;

            //返す医師が相手の石に囲まれている場所狙う
            foreach (var cell in turneCellInfos)
            {
                point += turneCellInfos.Count * 5;
            }
            cellInfoPoints.Add(cellInfo, point);
        }
        return cellInfoPoints.OrderByDescending(x => x.Value).First().Key;
    }
}
