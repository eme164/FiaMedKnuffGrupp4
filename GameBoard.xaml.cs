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
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FiaMedKnuffGrupp4
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameBoard : Page
    {
        private Random random = new Random();
        private readonly Models.Grid grid = new Models.Grid();
        private Team teamRed = new Models.Team(Colors.Red);
        private Team teamGreen = new Models.Team(Colors.Green);
        private Team teamYellow = new Models.Team(Colors.Yellow);
        private Team teamBlue = new Models.Team(Colors.Blue);
        private Teams teams = new Models.Teams();
        private int numberOfColumnsInGrid = 15;
        private bool drawGrid = true; //For debugging purposes
        private float cellSize;
        public Token selectedToken;
        private int diceRollResult;
        CanvasDrawingSession drawingSession;
        private enum ActiveTeam
        {
            Red,
            Green,
            Yellow,
            Blue
        }
        private ActiveTeam currentActiveTeam = ActiveTeam.Red;

        // Game initialization
        private void InitializeGame()
        {
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
            setDiceImageSize();
            GetActiveTeamColor();
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
            if (IsValidRoll())
            {
                Point pointerPosition = e.GetCurrentPoint(canvas).Position;

                // Check if the pointer is inside the bounds of any token in the active team.
                foreach (Models.Team team in teams.TeamList)
                {
                    if (team.TeamColor == GetActiveTeamColor())
                    {
                        foreach (Token token in team.TeamTokens)
                        {
                            if (IsPointerInsideToken(token, pointerPosition))
                            {
                                if (diceRollResult == 6 && token.isAtBase(grid))
                                {
                                    selectedToken = token; // Select
                                }else if (!token.isAtBase(grid))
                                {
                                    selectedToken = token; // Select
                                }
                                

                                if (selectedToken != null)
                                {
                                    selectedToken.MoveToken(selectedToken, diceRollResult, grid);

                                canvas.Invalidate();
                                SwitchToNextTeam();
                                Debug.WriteLine("Current active team: " + currentActiveTeam);

                                diceRollResult = 0;
                                    EnableDiceClick();
                                }


                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                //TODO: Inform the player that the roll is not valid, if needed.
            }
        }

        private bool IsPointerInsideToken(Token token, Point pointerPosition)
        {
            // Implement logic to check if the pointer is inside the bounds of the token.
            // You can use token properties (position, size) to perform this check.
            // Return true if the pointer is inside the token, otherwise return false.
            if (pointerPosition.X > token.getCurrentPositionCol() * cellSize &&
                pointerPosition.X < (token.getCurrentPositionCol() + 1) * cellSize &&
                pointerPosition.Y > token.getCurrentPositionRow() * cellSize &&
                pointerPosition.Y < (token.getCurrentPositionRow() + 1) * cellSize)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsValidRoll()
        {
            switch (currentActiveTeam)
            {
                case ActiveTeam.Red:
                    return teamRed.HasTokensLeftToMove() && (diceRollResult == 6 || teamRed.HasTokensOnNonZeroCells(grid));
                case ActiveTeam.Green:
                    return teamGreen.HasTokensLeftToMove() && (diceRollResult == 6 || teamGreen.HasTokensOnNonZeroCells(grid));
                case ActiveTeam.Yellow:
                    return teamYellow.HasTokensLeftToMove() && (diceRollResult == 6 || teamYellow.HasTokensOnNonZeroCells(grid));
                case ActiveTeam.Blue:
                    return teamBlue.HasTokensLeftToMove() && (diceRollResult == 6 || teamBlue.HasTokensOnNonZeroCells(grid));
                default:
                    SwitchToNextTeam();
                    return false;
            }
        }
        private void SwitchToNextTeam()
        {
            if(diceRollResult != 6)
            {
                switch (currentActiveTeam)
                {
                    case ActiveTeam.Red:
                        currentActiveTeam = ActiveTeam.Green;
                        break;
                    case ActiveTeam.Green:
                        currentActiveTeam = ActiveTeam.Yellow;
                        break;
                    case ActiveTeam.Yellow:
                        currentActiveTeam = ActiveTeam.Blue;
                        break;
                    case ActiveTeam.Blue:
                        currentActiveTeam = ActiveTeam.Red;
                        break;
                }

            }
        }
        private Color GetActiveTeamColor()
        {
            switch (currentActiveTeam)
            {
                case ActiveTeam.Red:
                    return Colors.Red;
                case ActiveTeam.Green:
                    return Colors.Green;
                case ActiveTeam.Yellow:
                    return Colors.Yellow;
                case ActiveTeam.Blue:
                    return Colors.Blue;
                default:
                    return Colors.Black; // Default color or handle error.
            }
        }



        private async void RollDiceButton_Click(object sender, RoutedEventArgs e)
        {
            selectedToken = null;
            PlayDiceSound();
            await RollDiceAnimation();
            diceRollResult = random.Next(1, 7);
            DiceImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/dice_" + diceRollResult + ".png"));
            DisableDiceClick();
            if(!IsValidRoll())
            {
                diceRollResult = 0;
                SwitchToNextTeam();
                EnableDiceClick();
            }

            else
            {
                if (diceRollResult == 6)
                {
                    // If a six was rolled, notify the player with your new method.
                    NotifyPlayerForSix();
                }
            }
            Debug.WriteLine("Current active team: " + currentActiveTeam);
        }

        private async void NotifyPlayerForSix()
        {
            // Play the sound for rolling a six
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/noftication_roll6.mp3")); 
            mediaPlayer.Play();

            // Notify the player with a dialog (or another UI element)
            ContentDialog rollSixDialog = new ContentDialog
            {
                Title = "Great Roll" + " " + currentActiveTeam + "!",
                Content = "You rolled a six! You get another turn.",
                CloseButtonText = "Awesome!"
            };

            await rollSixDialog.ShowAsync();
        }


        private MediaPlayer mediaPlayer = new MediaPlayer();
        private void PlayDiceSound()
        {
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/dice_roll.mp3"));
            mediaPlayer.Play();
        }


        private async Task RollDiceAnimation()
        {
            string[] diceImages = { "dice_1.png", "dice_2.png", "dice_3.png", "dice_4.png", "dice_5.png", "dice_6.png" };
            for (int i = 0; i < 10; i++)
            {
                DiceImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/" + diceImages[random.Next(0, 6)]));
                await Task.Delay(100);  // Adjust delay as per your needs.
            }
        }
        private async void setDiceImageSize()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                DiceImage.Width = cellSize * 1.5;
                DiceImage.Height = cellSize * 1.5;
            });
        }
        private void DisableDiceClick()
        {
            DiceImage.IsHitTestVisible = false;
        }
        private void EnableDiceClick()
        {
            DiceImage.IsHitTestVisible = true;
        }
    }
}
