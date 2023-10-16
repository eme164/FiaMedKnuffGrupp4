﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace FiaMedKnuffGrupp4.Models
{
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
        private readonly int StartPositionRow;
        private readonly int StartPositionCol;


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
        }

        public void MoveToken(Token token, int diceRollResult, Grid grid, List<Token> allTokens)
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
        public bool CollidesWith(Token other)
        {
            // Check if they are at the same position and have different colors.
            return this.CurrentPositionRow == other.CurrentPositionRow &&
                   this.CurrentPositionCol == other.CurrentPositionCol &&
                   this.TokenColor != other.TokenColor;
        }
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

        //draw token using win2d DrawingSession
        public void DrawToken(CanvasDrawingSession drawingSession, float cellSize, bool isSelected)
        {
            // Calculate the token's position based on its current animated position.
            float tokenX = AnimatedX * cellSize + cellSize / 2;
            float tokenY = AnimatedY * cellSize + cellSize / 2;

            // Define the token's radius and color.
            Color tokenColor = TokenColor;

            // If the token is selected, change its color to indicate selection.
            if (isSelected)
            {
                tokenColor = Colors.PaleGreen;
            }

            // Calculate the adjusted size which scales the size of the token based on its animation.
            float adjustedSize = cellSize * Scale;

            // Draw the token with the adjusted size and animated position.
            drawingSession.FillCircle(tokenX, tokenY, adjustedSize / 3, tokenColor);
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
    }
}
