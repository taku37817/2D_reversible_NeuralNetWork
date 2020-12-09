using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 完全にランダム
/// </summary>
public class Game_AI_Random : Game_AI_Base
{
   public Game_AI_Random(Game_Fild.StoneColor stoneColor)
    {
        this.stoneColor = stoneColor;
    }
    /// <summary>
    /// 次の手を取得
    /// </summary>
    /// <param name="gameField"></param>
    /// <returns></returns>
    public override CellInfo GetNextMove(Game_Fild gameField)
    {
        var sinmulateField = GenerateSimulateFieldWithGameField(gameField);
        var puttableCellInfos = sinmulateField.GetPuttableCellInfos(stoneColor);
        return puttableCellInfos[UnityEngine.Random.Range(0,puttableCellInfos.Count)];
    }
}
