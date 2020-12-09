using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


abstract public class Game_AI_Base
{
    /// <summary>
    /// AIの石の色
    /// </summary>
    protected Game_Fild.StoneColor stoneColor;

    /// <summary>
    /// 次の手を取得
    /// </summary>
    /// <returns>The next move.</returns>
    /// <param name="gameField">Game field.</param>
    abstract public Game_AI_Base.CellInfo GetNextMove(Game_Fild gameField);

    /// <summary>
    /// Game_Fieldオブジェクトを元に盤面シミュレータを作成
    /// </summary>
    /// <returns>The simulate field with game field.</returns>
    /// <param name="gameField">Game field.</param>
    protected SimulateField GenerateSimulateFieldWithGameField(Game_Fild gameField)
    {
        var cells = new Game_Fild.StoneColor[Game_Fild.SIZE_X, Game_Fild.SIZE_Y];
        foreach (var cell in gameField.cells)
        {
            cells[cell.X, cell.Y] = cell.stoneColor;
        }
        return new SimulateField(cells);
    }

    /// <summary>
    /// マス情報クラス
    /// </summary>
    public class CellInfo
    {
        /// <summary>
        /// 角かな？
        /// </summary>
        /// <value><c>true</c> if this instance is corner; otherwise, <c>false</c>.</value>
        public bool IsCorner
        {
            get
            {
                return x == 0 && y == 0 ||
                x == 0 && y == (Game_Fild.SIZE_Y - 1) ||
                x == (Game_Fild.SIZE_X - 1) && y == 0 ||
                x == (Game_Fild.SIZE_X - 1) && y == (Game_Fild.SIZE_Y - 1);
            }
        }

        /// <summary>
        /// 角の内側のマスかどうか
        /// </summary>
        /// <value><c>true</c> if this instance is corner inside; otherwise, <c>false</c>.</value>
        public bool IsCornerInside
        {
            get
            {
                return x == 1 && y == 1 ||
                x == 1 && y == (Game_Fild.SIZE_Y - 2) ||
                x == (Game_Fild.SIZE_X - 2) && y == 1 ||
                x == (Game_Fild.SIZE_X - 2) && y == (Game_Fild.SIZE_Y - 2);
            }
        }

        public readonly int x;
        public readonly int y;
        public readonly Game_Fild.StoneColor stoneColor;
        private Game_Fild.StoneColor stoneColor1;

        public CellInfo(int x, int y, Game_Fild.StoneColor stoneColor)
        {
            this.x = x;
            this.y = y;
            this.stoneColor = stoneColor;
        }
    }

    /// <summary>
    /// 盤面シミュレータクラス
    /// </summary>
    protected class SimulateField
    {
        /// <summary>
        /// ゲーム状況の定義
        /// </summary>
        public enum StageEnum
        {
            Opening,
            Middle,
            End
        }

        /// <summary>
        /// ゲーム状況
        /// </summary>
        /// <value>The stage.</value>
        public StageEnum Stage
        {
            get
            {
                var emptyCellCount = CountStone(Game_Fild.StoneColor.NONE);
                var stagePunctuation = (float)cells.Length / 3f;
                if (emptyCellCount < stagePunctuation)
                {
                    return StageEnum.End;
                }
                else if (emptyCellCount < stagePunctuation * 2)
                {
                    return StageEnum.Middle;
                }
                else
                {
                    return StageEnum.Opening;
                }
            }
        }

        Game_Fild.StoneColor[,] cells;

        public SimulateField(Game_Fild.StoneColor[,] cells)
        {
            this.cells = cells;
        }

        /// <summary>
        /// 石を置けるマスを返す
        /// </summary>
        /// <returns>The puttable cell infos.</returns>
        /// <param name="stoneColor">Stone color.</param>
        public List<CellInfo> GetPuttableCellInfos(Game_Fild.StoneColor stoneColor)
        {
            var infos = new List<CellInfo>();
            for (var x = 0; x < Game_Fild.SIZE_X; x++)
            {
                for (var y = 0; y < Game_Fild.SIZE_Y; y++)
                {
                    if (IsStonePuttable(x, y, stoneColor))
                    {
                        infos.Add(new CellInfo(x, y, stoneColor));
                    }
                }
            }
            return infos;

        }

        /// <summary>
        /// 指定した色の石を数える
        /// </summary>
        /// <returns>The stone.</returns>
        /// <param name="stoneColor">Stone color.</param>
        public int CountStone(Game_Fild.StoneColor stoneColor)
        {
            return cells.Cast<Game_Fild.StoneColor>().Count(x => x == stoneColor);
        }

        /// <summary>
        /// 指定マスの周り8マスに指定した色の石が何個あるか数える
        /// </summary>
        /// <returns>The stone around cell.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="stoneColor">Stone color.</param>
        public int CountStoneAroundCell(int x, int y, Game_Fild.StoneColor stoneColor)
        {
            var count = 0;
            for (var xDiff = -1; xDiff <= 1; xDiff++)
            {
                for (var yDiff = -1; yDiff <= 1; yDiff++)
                {
                    var nextX = x + xDiff;
                    var nextY = y + yDiff;
                    if (!IsCorrectCoordinate(nextX, nextY))
                    {
                        continue;
                    }
                    if ((xDiff != 0 || yDiff != 0) && cells[nextX, nextY] == stoneColor)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// 石を置く
        /// </summary>
        /// <returns>The stone.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="stoneColor">Stone color.</param>
        public List<CellInfo> PutStone(int x, int y, Game_Fild.StoneColor stoneColor)
        {
            if (!IsStonePuttable(x, y, stoneColor))
            {
                throw new Exception(string.Format("Cannot put {0} stone to [{1},{2}].", stoneColor.ToString(), x, y));
            }
            cells[x, y] = stoneColor;
            return TurnOverStoneIfPossible(x, y);
        }

        /// <summary>
        /// 指定のマスに石が置けるかどうかをチェック
        /// </summary>
        /// <returns><c>true</c> if this instance is stone puttable the specified x y stoneColor; otherwise, <c>false</c>.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="stoneColor">Stone color.</param>
        bool IsStonePuttable(int x, int y, Game_Fild.StoneColor stoneColor)
        {
            if (IsCorrectCoordinate(x, y) && cells[x, y] == Game_Fild.StoneColor.NONE)
            {
                return ExistsOwnStoneAtTheOtherSideOfEnemyStoneForDirection(x, y, stoneColor, -1, -1) ||
                ExistsOwnStoneAtTheOtherSideOfEnemyStoneForDirection(x, y, stoneColor, -1, 0) ||
                ExistsOwnStoneAtTheOtherSideOfEnemyStoneForDirection(x, y, stoneColor, -1, 1) ||
                ExistsOwnStoneAtTheOtherSideOfEnemyStoneForDirection(x, y, stoneColor, 0, -1) ||
                ExistsOwnStoneAtTheOtherSideOfEnemyStoneForDirection(x, y, stoneColor, 0, 1) ||
                ExistsOwnStoneAtTheOtherSideOfEnemyStoneForDirection(x, y, stoneColor, 1, -1) ||
                ExistsOwnStoneAtTheOtherSideOfEnemyStoneForDirection(x, y, stoneColor, 1, 0) ||
                ExistsOwnStoneAtTheOtherSideOfEnemyStoneForDirection(x, y, stoneColor, 1, 1);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 自分（指定色）の石が相手の石を挟める場所にあるかチェック
        /// </summary>
        /// <returns><c>true</c>, if own stone at the other side of enemy stone for direction was existsed, <c>false</c> otherwise.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="stoneColor">Stone color.</param>
        /// <param name="xDirection">X direction.</param>
        /// <param name="yDirection">Y direction.</param>
        bool ExistsOwnStoneAtTheOtherSideOfEnemyStoneForDirection(int x, int y, Game_Fild.StoneColor stoneColor, int xDirection, int yDirection)
        {
            var existsEnemyStone = false;
            while (true)
            {
                x += xDirection;
                y += yDirection;
                if (!IsCorrectCoordinate(x, y) || cells[x, y] == Game_Fild.StoneColor.NONE)
                {
                    return false;
                }
                else if (cells[x, y] == stoneColor)
                {
                    return existsEnemyStone;
                }
                else
                {
                    existsEnemyStone = true;
                }
            }
        }

        /// <summary>
        /// 石をひっくり返す
        /// </summary>
        /// <returns>The over stone if possible.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        List<CellInfo> TurnOverStoneIfPossible(int x, int y)
        {
            var infos = new List<CellInfo>();
            // 8方向それぞれに対し、ひっくり返す処理を実行
            infos.AddRange(TurnStoneForDirectionIfPossible(x, y, -1, -1));
            infos.AddRange(TurnStoneForDirectionIfPossible(x, y, -1, 0));
            infos.AddRange(TurnStoneForDirectionIfPossible(x, y, -1, 1));
            infos.AddRange(TurnStoneForDirectionIfPossible(x, y, 0, -1));
            infos.AddRange(TurnStoneForDirectionIfPossible(x, y, 0, 1));
            infos.AddRange(TurnStoneForDirectionIfPossible(x, y, 1, -1));
            infos.AddRange(TurnStoneForDirectionIfPossible(x, y, 1, 0));
            infos.AddRange(TurnStoneForDirectionIfPossible(x, y, 1, 1));
            return infos;
        }

        /// <summary>
        /// 指定方向にある石をひっくり返す
        /// </summary>
        /// <returns>The stone for direction if possible.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="xDirection">X direction.</param>
        /// <param name="yDirection">Y direction.</param>
        List<CellInfo> TurnStoneForDirectionIfPossible(int x, int y, int xDirection, int yDirection)
        {
            var infos = new List<CellInfo>();
            if (!ExistsOwnStoneAtTheOtherSideOfEnemyStoneForDirection(x, y, cells[x, y], xDirection, yDirection))
            {
                return infos;
            }

            var targetStoneColor = cells[x, y];

            while (true)
            {
                x += xDirection;
                y += yDirection;
                if (!IsCorrectCoordinate(x, y) || cells[x, y] == targetStoneColor)
                {
                    return infos;
                }
                else
                {
                    cells[x, y] = targetStoneColor;
                    infos.Add(new CellInfo(x, y, targetStoneColor));
                }
            }
        }

        /// <summary>
        /// 盤面に存在する座標かどうかチェック
        /// </summary>
        /// <returns><c>true</c> if this instance is correct coordinate the specified x y; otherwise, <c>false</c>.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        bool IsCorrectCoordinate(int x, int y)
        {
            return 0 <= x && x < Game_Fild.SIZE_X && 0 <= y && y < Game_Fild.SIZE_Y;
        }
    }
}