using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace FiaMedKnuffGrupp4.Models
{
    public class Token
    {
        public string TokenID { get; set; }
        public int CurrentPositionRow { get; set; }
        public int CurrentPositionCol { get; set; }
        public Color TokenColor { get; set; }

        public Token(string tokenID, int currentPositionRow, int currentPositionCol, Color tokenColor)
        {
            TokenID = tokenID;
            CurrentPositionCol = currentPositionCol;
            CurrentPositionRow = currentPositionRow;
            TokenColor = tokenColor;
        }

        public void MoveToken(Token token, int diceRollResult, Grid grid)
        {
            int tile = grid.GetTile(CurrentPositionRow, CurrentPositionCol);
            if(diceRollResult == 6 && tile == 0) 
            { 
                if(token.TokenColor == Colors.Red)
                {
                    CurrentPositionCol = 6;
                    CurrentPositionRow = 13;
                }
                else if(token.TokenColor == Colors.Green)
                {
                    CurrentPositionCol = 1;
                    CurrentPositionRow = 6;
                }
                else if(token.TokenColor == Colors.Blue)
                {
                    CurrentPositionCol = 13;
                    CurrentPositionRow = 8;
                }
                else if(token.TokenColor == Colors.Yellow)
                {
                    CurrentPositionCol = 8;
                    CurrentPositionRow = 1;
                }
                diceRollResult = 0;
            }
            //1 = Up, 2 = Right,3 = up & right 4 = Down, 5 = down / right 6 = left / up 8 = Left 9 = left / down 11
            //Only move as many cells as the dice roll result.
            for(int i = 0; i < diceRollResult; i++)
            {
                switch(tile)
                {
                    case 1:
                        CurrentPositionRow--;
                        break;
                    case 2:
                        CurrentPositionCol++;
                        break;
                    case 3:
                        CurrentPositionCol++;
                        CurrentPositionRow--;
                        break;
                    case 4:
                        CurrentPositionRow++;
                        break;
                    case 5:
                        CurrentPositionCol++;
                        CurrentPositionRow++;
                        break;
                    case 6:
                        CurrentPositionCol--;
                        CurrentPositionRow--;
                        break;
                    case 8:
                        CurrentPositionCol--;
                        break;
                    case 9:
                        CurrentPositionCol--;
                        CurrentPositionRow++;
                        break;
                }
                tile = grid.GetTile(CurrentPositionRow, CurrentPositionCol);
            }
        }

        //draw token using win2d DrawingSession
        public void DrawToken(CanvasDrawingSession drawingSession, float cellSize, bool isSelected)
        {
            // Calculate the token's position based on row and column, as you did before.
            float tokenX = CurrentPositionCol * cellSize + cellSize / 2;
            float tokenY = CurrentPositionRow * cellSize + cellSize / 2;

            // Define the token's radius and color.
            float tokenRadius = cellSize / 3;
            Color tokenColor = TokenColor;

            // If the token is selected, change its color to indicate selection.
            if (isSelected)
            {
                tokenColor = Colors.PaleGreen; // Change the color to blue (or any color of your choice).
            }

            // Draw the token.
            drawingSession.FillCircle(tokenX, tokenY, tokenRadius, tokenColor);
        }
        public int getCurrentPositionCol()
        {
            return CurrentPositionCol;
        }
        public int getCurrentPositionRow()
        {
            return CurrentPositionRow;
        }
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
    }
}
