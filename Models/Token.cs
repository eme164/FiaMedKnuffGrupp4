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
        string TokenID { get; set; }
        int StartingPositionRow { get; set; }
        int StartingPositionCol { get; set; }
        int CurrentPositionRow { get; set; }
        int CurrentPositionCol { get; set; }
        int TargetPositionRow { get; set; }
        int TargetPositionCol { get; set; }
        Color TokenColor { get; set; }

        public Token(string tokenID, int startingPositionRow, int startingPositionCol, Color tokenColor)
        {
            TokenID = tokenID;
            StartingPositionCol = startingPositionCol;
            StartingPositionRow = startingPositionRow;
            TokenColor = tokenColor;
        }

        public void MoveToken()
        {

        }

        //draw token using win2d DrawingSession
        public void DrawToken(CanvasDrawingSession drawingSession, float cellSize, bool isSelected)
        {
            // Calculate the token's position based on row and column, as you did before.
            float tokenX = StartingPositionCol * cellSize + cellSize / 2;
            float tokenY = StartingPositionRow * cellSize + cellSize / 2;

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


        public int getStartingPositionRow()
        {
            return StartingPositionRow;
        }
        public int getStartingPositionCol()
        {
            return StartingPositionCol;
        }
    }
}
