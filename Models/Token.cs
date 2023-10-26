using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace FiaMedKnuffGrupp4.Models
{   /// <summary>
    /// Represents a token.
    /// </summary>
    public class Token
    {
        public string TokenID { get; set; }
        public int CurrentPositionRow { get; set; }
        public int CurrentPositionCol { get; set; }
        public Color TokenColor { get; set; }
        public float AnimatedX { get; private set; }
        public float AnimatedY { get; private set; }
        private float targetX, targetY;
        private float startX, startY;
        private DateTime startTime;
        public bool IsAnimating { get; private set; } = false;
        private const float Duration = 1f; // half a second per tile
        public float Scale { get; set; } = 1.0f;
        public int StartPositionRow;
        public int StartPositionCol;
        public bool isInsideGoal { get; set; }

        /// <summary>
        /// Constructor for Token
        /// </summary>
        /// <param name="tokenID"></param>
        /// <param name="currentPositionRow"></param>
        /// <param name="currentPositionCol"></param>
        /// <param name="tokenColor"></param>
        public Token(string tokenID, int currentPositionRow, int currentPositionCol, Color tokenColor)
        {
            TokenID = tokenID;
            CurrentPositionCol = currentPositionCol;
            CurrentPositionRow = currentPositionRow;
            TokenColor = tokenColor;
            AnimatedX = currentPositionCol;
            AnimatedY = currentPositionRow;
            StartPositionCol = currentPositionCol;
            StartPositionRow = currentPositionRow;
            isInsideGoal = false;
        }

        /// <summary>
        /// This method moves the token based on the dice roll result.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="diceRollResult"></param>
        /// <param name="grid"></param>
        /// <param name="allTokens"></param>
        public void MoveToken(Token token, int diceRollResult, Grid grid, List<Token> allTokens)
        {
            bool canContinueMoving = true;
            int tile = grid.GetTile(CurrentPositionRow, CurrentPositionCol);
            int stepsRequired = StepsToGoal(token, grid);

            if (diceRollResult == 6 && tile == 0)
            {
                if (token.TokenColor == Colors.Red)
                {
                    CurrentPositionCol = 6;
                    CurrentPositionRow = 13;
                }
                else if (token.TokenColor == Colors.Green)
                {
                    CurrentPositionCol = 1;
                    CurrentPositionRow = 6;
                }
                else if (token.TokenColor == Colors.Blue)
                {
                    CurrentPositionCol = 13;
                    CurrentPositionRow = 8;
                }
                else if (token.TokenColor == Colors.Yellow)
                {
                    CurrentPositionCol = 8;
                    CurrentPositionRow = 1;
                }
                diceRollResult = 0;
            }
            // 1 = Up, 2 = Right,3 = up&right 4 = Down, 5 = down/right 6 = left/up 8 = Left 9 = left/down 11 = greencolumn, 12 = yellowcolumn, 13 = bluecolumn, 14 = redcolumn 15 = goal
            //Only move as many cells as the dice roll result.
            int tempRow = CurrentPositionRow;
            int tempCol = CurrentPositionCol;

            for (int i = 0; i < diceRollResult; i++)
            {
                switch (tile)
                {
                    case 1:
                        tempRow--;
                        break;
                    case 2:
                        tempCol++;
                        break;
                    case 3:
                        tempCol++;
                        tempRow--;
                        break;
                    case 4:
                        tempRow++;
                        break;
                    case 5:
                        tempCol++;
                        tempRow++;
                        break;
                    case 6:
                        tempCol--;
                        tempRow--;
                        break;
                    case 8:
                        tempCol--;
                        break;
                    case 9:
                        tempCol--;
                        tempRow++;
                        break;
                    case 11:
                        if(token.TokenColor == Colors.Green)
                        {
                            tempCol++;
                            break;
                        }
                        else
                        {
                            tempRow--;
                        break;
                        }
                    case 12:
                        if(token.TokenColor == Colors.Yellow)
                        {
                            tempRow++;
                            break;
                        }
                        else
                        {
                            tempCol++;
                            break;
                        }
                    case 13:
                        if(token.TokenColor == Colors.Blue)
                        {
                            tempCol--;
                            break;
                        }
                        else
                        {
                            tempRow++;
                            break;
                        }
                    case 14:
                        if(token.TokenColor == Colors.Red)
                        {
                            tempRow--;
                            break;
                        }
                        else
                        {
                            tempCol--;
                            break;
                        }
                    case 15: //TODO: Decide what should happen to the token who reached the goal.
                        canContinueMoving = false;
                        break;

                }

                tile = grid.GetTile(tempRow, tempCol);
                if(tile == 15)
                {
                    isInsideGoal = true;
                    Debug.WriteLine("You have reached the goal!");
                }

                var tokensAtNextPosition = allTokens.Where(t =>
                   t.CurrentPositionRow == tempRow &&
                   t.CurrentPositionCol == tempCol).ToList();

                // Check if two tokens of a different color block the path
                if (tokensAtNextPosition.Count >= 2 && tokensAtNextPosition.Any(t => t.TokenColor != token.TokenColor))
                {
                    canContinueMoving = false;
                    break;
                }
                // If can continue, update the token's current position.
                if (canContinueMoving)
                {
                    CurrentPositionRow = tempRow;
                    CurrentPositionCol = tempCol;
                }
            
            }


            foreach (var otherToken in allTokens)
            {
                if (this.CollidesWith(otherToken))
                {
                    otherToken.resetToken();
                }
            }


            startX = AnimatedX;
            startY = AnimatedY;
            targetX = CurrentPositionCol; // this assumes one unit per tile
            targetY = CurrentPositionRow; // this assumes one unit per tile
            startTime = DateTime.Now;
            IsAnimating = true;

        }
        /// <summary>
        /// Checks if the token collides with another token
        /// Used to make the other token return to base
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CollidesWith(Token other)
        {
            // Check if they are at the same position and have different colors.
            return this.CurrentPositionRow == other.CurrentPositionRow &&
                   this.CurrentPositionCol == other.CurrentPositionCol &&
                   this.TokenColor != other.TokenColor;
        }

        /// <summary>
        /// This method updates the animation of the token.
        /// EaseInOutQuad is used to make the animation smoother.
        /// Scale is used to make the token grow and shrink during the animation.
        /// </summary>
        public void UpdateAnimation()
        {
            if (!IsAnimating) return;

            var elapsed = (float)(DateTime.Now - startTime).TotalSeconds;
            var t = elapsed / Duration;

            // Use LERP to determine the current animated position
            AnimatedX = startX + t * (targetX - startX);
            AnimatedY = startY + t * (targetY - startY);

            // Quadratic in-out easing function
            float EaseInOutQuad(float time)
            {
                return time < 0.5 ? 2 * time * time : -1 + (4 - 2 * time) * time;
            }

            float scaleChange = EaseInOutQuad(t);

            // Use the eased value to adjust the scale over the animation
            if (t <= 0.5)
            {
                Scale = 1.0f + scaleChange * 0.9f;
            }
            else
            {
                Scale = 1.0f + (1 - scaleChange) * 0.9f;
            }

            if (t >= 1)
            {
                IsAnimating = false;
                AnimatedX = targetX;
                AnimatedY = targetY;
                Scale = 1.0f;
            }
        }

        /// <summary>
        /// This method draws the token.
        /// if there are two tokens of the same color on the same tile, one of them will be drawn slightly offset.
        /// cellSize is used to determine the size of the token.
        /// tokenAtSamePosition is used to determine if there are two tokens of the same color on the same tile.
        /// </summary>
        /// <param name="drawingSession"></param>
        /// <param name="cellSize"></param>
        /// <param name="isSelected"></param>
        /// <param name="allTokens"></param>
        public void DrawToken(CanvasDrawingSession drawingSession, float cellSize, bool isSelected, List<Token> allTokens)
        {
            // Calculate the token's position based on its current animated position.
            float tokenX = AnimatedX * cellSize + cellSize / 2;
            float tokenY = AnimatedY * cellSize + cellSize / 2;

            var tokensAtSamePosition = allTokens.Where(t =>
            t.CurrentPositionRow == this.CurrentPositionRow &&
            t.CurrentPositionCol == this.CurrentPositionCol &&
            t.TokenColor == this.TokenColor).ToList();

            if (tokensAtSamePosition.Count > 1)
            {
                if (tokensAtSamePosition[0] == this)
                {
                    tokenX += cellSize * 0.1f;
                    tokenY += cellSize * 0.1f;
                }
                else
                {
                    tokenX -= cellSize * 0.1f;
                    tokenY -= cellSize * 0.1f;
                }
            }
            // Define the token's radius and color.
            Color tokenColor = TokenColor;

            // Calculate the adjusted size which scales the size of the token based on its animation.
            float adjustedSize = cellSize * Scale;

            // Draw the outline of the token.
            float outlineThickness = 4.0f;  // Adjust thickness as needed
            drawingSession.DrawCircle(tokenX, tokenY, adjustedSize / 3, Colors.Black, outlineThickness);

            // Draw the token with the adjusted size and animated position.
            drawingSession.FillCircle(tokenX, tokenY, adjustedSize / 3, tokenColor);
        }

        /// <summary>
        /// This method returns the current column the token is positioned in.
        /// </summary>
        /// <returns>Current column</returns>
        public int getCurrentPositionCol()
        {
            return CurrentPositionCol;
        }
        /// <summary>
        /// This method returns the current row the token is positioned in.
        /// </summary>
        /// <returns>Current Row</returns>
        public int getCurrentPositionRow()
        {
            return CurrentPositionRow;
        }
        /// <summary>
        /// Uses the grid to check if the token is at the base
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>True if the cell/tile is 0</returns>
        public bool isAtBase(Grid grid)
        {
            int tile = grid.GetTile(CurrentPositionRow, CurrentPositionCol);
            if (tile == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the token is at the goal
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>True if the tile/cell is of value 15</returns>
        public bool IsAtGoal(Grid grid)
        {
            int currentTile = grid.GetTile(CurrentPositionRow, CurrentPositionCol);

            return currentTile == 15;
        }

        /// <summary>
        /// Resets the token to its starting position
        /// </summary>
        public void resetToken()
        {
            CurrentPositionCol = StartPositionCol;
            CurrentPositionRow = StartPositionRow;
            startX = AnimatedX;
            startY = AnimatedY;
            targetX = StartPositionCol;
            targetY = StartPositionRow;
            startTime = DateTime.Now;
            IsAnimating = true;
        }
        /// <summary>
        /// Checks how many steps the token has to take to reach the goal
        /// Used to determine if the token can move or not
        /// </summary>
        /// <param name="token"></param>
        /// <param name="grid"></param>
        /// <returns>Number of steps(int) away from the goal</returns>
        public int StepsToGoal(Token token, Grid grid)
        {
            int steps = 0;
            int tile = grid.GetTile(token.CurrentPositionRow, token.CurrentPositionCol);
            int tempRow = token.CurrentPositionRow;
            int tempCol = token.CurrentPositionCol;

            while (tile != 15 && steps < 100) // we add a maximum of 100 steps to prevent infinite loops in case of errors
            {
                switch (tile)
                {
                    case 1:
                        tempRow--;
                        break;
                    case 2:
                        tempCol++;
                        break;
                    case 4:
                        tempRow++;
                        break;
                    case 8:
                        tempCol--;
                        break;
                }
                tile = grid.GetTile(tempRow, tempCol);
                steps++;
            }

            return steps;
        }

    }
}
