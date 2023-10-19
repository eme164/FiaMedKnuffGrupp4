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
using ColorCode.Common;
using Windows.UI.Xaml.Shapes;

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
        //Randomize who starts the game
        private ActiveTeam currentActiveTeam = (ActiveTeam)new Random().Next(0, 4);

        // Game initialization
        //CREATING TOKENS WITH THEIR STARTING POSITIONS AND PLACING THEM IN A TEAM
        private void InitializeGame()
        {
            // Clear tokens for each team
            teamRed.ClearTokens();
            teamGreen.ClearTokens();
            teamYellow.ClearTokens();
            teamBlue.ClearTokens();
            //Original placement of tokens
            teamRed.AddToken(new Token("Red1", 10, 1, Colors.Red));
            teamRed.AddToken(new Token("Red2", 13, 1, Colors.Red));
            teamRed.AddToken(new Token("Red3", 10, 4, Colors.Red));
            teamRed.AddToken(new Token("Red4", 13, 4, Colors.Red));

            //Only for testing purposes to get tokens to goal faster
            //teamRed.AddToken(new Token("Red1", 10, 7, Colors.Red));
            //teamRed.AddToken(new Token("Red2", 9, 7, Colors.Red));
            //teamRed.AddToken(new Token("Red3", 9, 7, Colors.Red));
            //teamRed.AddToken(new Token("Red4", 10, 7, Colors.Red));

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
        //LIST OF ALL TOKENS TO CHECK FOR COLLISIONS
        private List<Token> AllTokens()
        {
            List<Token> allTokens = new List<Token>();
            foreach (Models.Team team in teams.TeamList)
            {
                foreach (Token token in team.TeamTokens)
                {
                    allTokens.Add(token);
                }
            }
            return allTokens;
        }


        public GameBoard()
        {
            this.InitializeComponent();
            InitializeGame();

            Debug.WriteLine("Current active team: " + currentActiveTeam);
        }

        //DRAWING THE BOARD AND TOKENS
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
                    if (!token.isInsideGoal)
                    {
                        token.DrawToken(drawingSession, cellSize, token == selectedToken, AllTokens());
                    }
                }
            }
            
        }

        private void canvas_Update(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
            
            setCellSize();
            setCanvasMargin();
            setDiceImageAndVictoryImageSize();
            GetActiveTeamColor();
            UpdateRedGoalsOpacity();
            SetButtonAreaSize();

            foreach (Models.Team team in teams.TeamList)
            {
                foreach (Token token in team.TeamTokens)
                {
                    token.UpdateAnimation();
                }
            }
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
                canvas.Margin = new Thickness((boardGrid.ActualWidth - backgroundImage.ActualWidth) / 2 - (buttonArea.ActualWidth / 2),
                                                (boardGrid.ActualHeight - backgroundImage.ActualHeight) / 2,
                                                (boardGrid.ActualWidth - backgroundImage.ActualWidth) / 2 - (buttonArea.ActualWidth / 2),
                                                (boardGrid.ActualHeight - backgroundImage.ActualHeight) / 2);
            });
        }

        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (diceRollResult == 0)
            {
                //TODO: Replace the debug line with some actual in-game indication that the dice needs to be rolled first.
                Debug.WriteLine("Roll the dice first!");
                return;
            }

            Point pointerPosition = e.GetCurrentPoint(canvas).Position;

            Models.Team activeTeam = teams.TeamList.FirstOrDefault(t => t.TeamColor == GetActiveTeamColor());
            if (activeTeam == null)
            {
                // No active team found.
                return;
            }

            bool validMoveFound = false;

            foreach (Token token in activeTeam.TeamTokens)
            {
                if (IsPointerInsideToken(token, pointerPosition))
                {
                    int stepsToGoal = StepsToGoal(token);

                    // Check if the dice roll result would exceed the steps to goal
                    if (diceRollResult <= stepsToGoal)
                    {
                        if ((diceRollResult == 6 && token.isAtBase(grid)) || !token.isAtBase(grid))
                        {
                            selectedToken = token; // Select token
                            validMoveFound = true;
                        }
                    }
                }

                if (validMoveFound)
                {
                    break;
                }
            }

            if (validMoveFound && selectedToken != null)
            {
                selectedToken.MoveToken(selectedToken, diceRollResult, grid, AllTokens());
                canvas.Invalidate();
                if(AllTokensAtGoal())
                {
                    Debug.WriteLine("All tokens at goal");
                }
                SwitchToNextTeam();
                Debug.WriteLine("Current active team: " + currentActiveTeam);
                diceRollResult = 0;
                EnableDiceClick();
            }
            else if (!IsValidRoll())
            {
                // If the roll isn't valid and no valid moves can be made, switch to the next team.
                Debug.WriteLine("No valid moves available for current team. Switching...");
                SwitchToNextTeam();
                diceRollResult = 0;  // Reset the dice roll
                EnableDiceClick();
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
            Models.Team activeTeam = GetCurrentTeam();

            // If any token is in base and roll is 6, it's valid.
            if (diceRollResult == 6 && activeTeam.HasTokensInBase(grid))
                return true;

            // If there are tokens outside the base, check if they can move with the current roll.
            foreach (Token token in activeTeam.TeamTokens)
            {
                if (!token.isAtBase(grid))
                {
                    int stepsToGoal = StepsToGoal(token);
                    if (diceRollResult <= stepsToGoal)
                        return true; // found a valid move for the current roll
                }
            }

            return false;
        }

        private Models.Team GetCurrentTeam()
        {
            switch (currentActiveTeam)
            {
                case ActiveTeam.Red:
                    return teamRed;
                case ActiveTeam.Green:
                    return teamGreen;
                case ActiveTeam.Yellow:
                    return teamYellow;
                case ActiveTeam.Blue:
                    return teamBlue;
                default:
                    throw new Exception("Invalid team."); // You can handle this however you like.
            }
        }

        //Call this method to switch to the next team.
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
        private async void setDiceImageAndVictoryImageSize()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                DiceImage.Width = cellSize * 1.5;
                DiceImage.Height = cellSize * 1.5;

                victoryImage.Height = cellSize * 8;
                victoryImage.Width = cellSize * 8;
            });

        }
        //check if all tokens in current active team are at goal
        
        private bool AllTokensAtGoal()
        {
            foreach(Token token in GetCurrentTeam().TeamTokens)
            {
                if (!token.isInsideGoal)
                {
                    return false;
                }
            }
            PromptUsersForNextAction();
            //Not Sure if we want to use this image after a game is won
            //victoryImage.Visibility = Visibility.Visible;
            return true;
        }
        private void DisableDiceClick()
        {
            DiceImage.IsHitTestVisible = false;
        }
        private void EnableDiceClick()
        {
            DiceImage.IsHitTestVisible = true;
        }
        public int StepsToGoal(Token token)
        {
            int steps = 0;
            int tile = grid.GetTile(token.CurrentPositionRow, token.CurrentPositionCol);
            int tempRow = token.CurrentPositionRow;
            int tempCol = token.CurrentPositionCol;

            while (tile != 15 && steps < 100)
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
        private bool HasCurrentTeamWon()
        {
            Models.Team activeTeam = GetCurrentTeam();

            return activeTeam.TeamTokens.All(token => token.IsAtGoal(grid));
        }
        private async void PromptUsersForNextAction()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Game Over",
                Content = "Your team has won! Would you like to play again or return to the main menu?",
                PrimaryButtonText = "Play Again",
                CloseButtonText = "Main Menu"
            };

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                RestartGame(); 
            }
            else
            {
                ReturnToMainMenu(); 
            }
        }
        private void RestartGame()
        {
            Frame.Navigate(this.GetType());
            //// Reset the positions of all tokens to their starting positions.
            //foreach (var team in teams.TeamList)
            //{
            //    foreach (var token in team.TeamTokens)
            //    {
            //        token.resetToken();
            //    }
            //}

            //// Reset game variables
            //currentActiveTeam = ActiveTeam.Red; //or 0
            //diceRollResult = 0;

            //// Redraw the game board.
            //canvas.Invalidate();
        }

        private void ReturnToMainMenu()
        {
            //RestartGame();

            // Navigate back to the main menu.
            Frame.Navigate(typeof(Startmenutogame));
        }
        private async void UpdateRedGoalsOpacity()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                for(int i = 0; i < 4; i++)
                {
                    if (teamRed.TeamTokens[i].isInsideGoal)
                    {
                        redTokensGoal.Children[i].Opacity = 1;
                    }
                }
            });
        }

        private async void SetButtonAreaSize()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                menuPopup.Width = cellSize * 8;
                //relativePanel.Width = backgroundImage.ActualWidth;
                //relativePanel.Height = backgroundImage.ActualHeight;
                int minFontSize = 12;

                StartButton.Width = cellSize * 2;  

                if (StartButton.Content is TextBlock buttonContent)
                {
                    double desiredFontSize = cellSize / 4;
                    buttonContent.FontSize = desiredFontSize < minFontSize ? minFontSize : desiredFontSize;
                }

                foreach (var stackPanel in new[] { redTokensGoal, greenTokensGoal, yellowTokensGoal, blueTokensGoal })
                {
                    foreach (Ellipse ellipse in stackPanel.Children)
                    {
                        ellipse.Height = cellSize / 2;
                        ellipse.Width = cellSize / 2;
                    }
                }
                foreach (var child in mainStackPanel.Children)
                {
                    if (child is TextBlock textBlock && textBlock.Text == "GOAL:")
                    {
                        textBlock.FontSize = cellSize / 2;
                    }
                }
            });
        }

        private void ResetGameState()
        {
            // Reset the position of all tokens to their original positions
            InitializeGame();

            // Reset the current active team to a random team
            currentActiveTeam = (ActiveTeam)new Random().Next(0, 4);

            // Reset other necessary game state variables
            selectedToken = null;
            diceRollResult = 0;

            // Possibly invalidate the canvas to redraw the initial game state
            canvas.Invalidate();

            Debug.WriteLine("Game state reset. Current active team: " + currentActiveTeam);
        }


        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void restart_Click(object sender, RoutedEventArgs e)
        {
            ResetGameState();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            menuPopup.IsOpen = true;
        }
    }
    

}
