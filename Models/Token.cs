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
            if(diceRollResult == 1) 
            { 
                if(token.TokenColor == Colors.Red && grid.GetTile(CurrentPositionCol,CurrentPositionRow) == 0)
                {
                    CurrentPositionCol = 6;
                    CurrentPositionRow = 13;
                }
                else if(token.TokenColor == Colors.Green && grid.GetTile(CurrentPositionCol, CurrentPositionRow) == 0)
                {
                    CurrentPositionCol = 1;
                    CurrentPositionRow = 6;
                }
                else if(token.TokenColor == Colors.Blue && grid.GetTile(CurrentPositionCol, CurrentPositionRow) == 0)
                {
                    CurrentPositionCol = 13;
                    CurrentPositionRow = 8;
                }
                else if(token.TokenColor == Colors.Yellow && grid.GetTile(CurrentPositionCol, CurrentPositionRow) == 0)
                {
                    CurrentPositionCol = 8;
                    CurrentPositionRow = 1;
                }
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
    }
}
