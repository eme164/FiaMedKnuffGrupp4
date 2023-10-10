using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas;
using FiaMedKnuffGrupp4.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FiaMedKnuffGrupp4
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameBoard : Page
    {
        private readonly Models.Grid grid = new Models.Grid();
        private Models.Team teamRed = new Models.Team(Colors.Red);
        private Models.Team teamGreen = new Models.Team(Colors.Green);
        private Models.Team teamYellow = new Models.Team(Colors.Yellow);
        private Models.Team teamBlue = new Models.Team(Colors.Blue);
        private Models.Teams teams = new Models.Teams();
        private int numberOfColumnsInGrid = 15;
        private bool drawGrid = true; //For debugging purposes
        private float cellSize;
        public Token selectedToken;
        CanvasDrawingSession drawingSession;

        // Game initialization
        private void InitializeGame()
        {

            var teamRed = new Models.Team(Colors.Red);
            var teamGreen = new Models.Team(Colors.Green);
            var teamYellow = new Models.Team(Colors.Yellow);
            var teamBlue = new Models.Team(Colors.Blue);


            teamRed.AddToken(new Token("Red1", 10, 1, Colors.Red));
            teamRed.AddToken(new Token("Red2", 13, 1, Colors.Red));
            teamRed.AddToken(new Token("Red3", 10, 4, Colors.Red));
            teamRed.AddToken(new Token("Red4", 13, 4, Colors.Red));

            teamGreen.AddToken(new Token("Green1", 1, 1, Colors.Green));
            teamGreen.AddToken(new Token("Green2", 1, 4, Colors.Green));
            teamGreen.AddToken(new Token("Green3", 4, 1, Colors.Green));
            teamGreen.AddToken(new Token("Green4", 4, 4, Colors.Green));

            teamYellow.AddToken(new Token("Yellow1", 1, 10, Colors.Yellow));
            teamYellow.AddToken(new Token("Yellow2", 4, 10, Colors.Yellow));
            teamYellow.AddToken(new Token("Yellow3", 1, 13, Colors.Yellow));
            teamYellow.AddToken(new Token("Yellow4", 4, 13, Colors.Yellow));

            teamBlue.AddToken(new Token("Blue1", 10, 10, Colors.Blue));
            teamBlue.AddToken(new Token("Blue2", 10, 13, Colors.Blue));
            teamBlue.AddToken(new Token("Blue3", 13, 10, Colors.Blue));
            teamBlue.AddToken(new Token("Blue4", 13, 13, Colors.Blue));

            // Add teams to the Teams collection
            teams.AddTeam(teamRed);
            teams.AddTeam(teamGreen);
            teams.AddTeam(teamYellow);
            teams.AddTeam(teamBlue);
        }



        public GameBoard()
        {
            this.InitializeComponent();
            InitializeGame();
        }
        private void canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            drawingSession = args.DrawingSession;
            float fontSize = 12;
            // Create a CanvasTextFormat with the desired font size
            var textFormat = new CanvasTextFormat
            {
                FontSize = fontSize
            };

            // Loop through all cells and draw a black outline with coordinates inside each cell.
            if (drawGrid)
            {
                for (int y = 0; y < 15; y++)
                {
                    for (int x = 0; x < 15; x++)
                    {
                        float cellX = x * cellSize;
                        float cellY = y * cellSize;

                        drawingSession.DrawRectangle(cellX, cellY, cellSize, cellSize, Colors.Black, 1);

                        string coordinates = $"{y}, {x}";
                        drawingSession.DrawText(coordinates, cellX + cellSize / 4, cellY + cellSize / 4, Colors.Black, textFormat);
                    }
                }
            }

            // Draw all tokens in all the teams
            foreach (Models.Team team in teams.TeamList)
            {
                foreach (Token token in team.TeamTokens)
                {
                    token.DrawToken(drawingSession, cellSize, token == selectedToken);
                }
            }
        }

        private void canvas_Update(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
            setCellSize();
            setCanvasMargin();
        }

        private void canvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {

        }

        private async void setCellSize()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                cellSize = (float)backgroundImage.ActualHeight / numberOfColumnsInGrid;
            });
        }

        private async void setCanvasMargin()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                canvas.Margin = new Thickness((boardGrid.ActualWidth - backgroundImage.ActualWidth) / 2, (boardGrid.ActualHeight - backgroundImage.ActualHeight) / 2,
                                                                (boardGrid.ActualWidth - backgroundImage.ActualWidth) / 2, (boardGrid.ActualHeight - backgroundImage.ActualHeight) / 2);
            });
        }
        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Get the position of the pointer relative to the canvas.
            Point pointerPosition = e.GetCurrentPoint(canvas).Position;

            // Check if the pointer is inside the bounds of any token in any team.
            foreach (Models.Team team in teams.TeamList)
            {
                foreach (Token token in team.TeamTokens)
                {
                    if (IsPointerInsideToken(token, pointerPosition))
                    {
                        // If the clicked token is the same as the currently selected token, deselect it.
                        if (selectedToken == token)
                        {
                            selectedToken = null; // Deselect
                        }
                        else
                        {
                            selectedToken = token; // Select
                        }

                        // Redraw the canvas to update the token visuals (e.g., to indicate selection).
                        canvas.Invalidate();

                        // Exit the loop once a token is found within the pointer's bounds.
                        return;
                    }
                }
            }

            // Clicked outside of any token, so deselect any previously selected token.
            selectedToken = null;
            canvas.Invalidate();
        }


        private bool IsPointerInsideToken(Token token, Point pointerPosition)
        {
            // Implement logic to check if the pointer is inside the bounds of the token.
            // You can use token properties (position, size) to perform this check.
            // Return true if the pointer is inside the token, otherwise return false.
            if (pointerPosition.X > token.getStartingPositionCol() * cellSize &&
                pointerPosition.X < (token.getStartingPositionCol() + 1) * cellSize &&
                pointerPosition.Y > token.getStartingPositionRow() * cellSize &&
                pointerPosition.Y < (token.getStartingPositionRow() + 1) * cellSize)
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
