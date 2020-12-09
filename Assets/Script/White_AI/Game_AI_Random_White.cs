using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;

public class Game_AI_Random_White : Game_AI_Base
{
    public Game_AI_Random_White(Game_Fild.StoneColor stoneColor)
    {
        this.stoneColor = stoneColor;
    }
    public override CellInfo GetNextMove(Game_Fild gameField)
    {
        var simulateField = GenerateSimulateFieldWithGameField(gameField);
        var puttableCellInfo = simulateField.GetPuttableCellInfos(stoneColor);
        return puttableCellInfo[UnityEngine.Random.Range(0, puttableCellInfo.Count)];
    }
     
}
