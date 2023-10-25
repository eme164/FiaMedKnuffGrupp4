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
using Windows.System;

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
        private bool drawGrid = false; //For debugging purposes
        private float cellSize;
        public Token selectedToken;
        private int diceRollResult;
        private bool loadGame = false;
        CanvasDrawingSession drawingSession;
       
        public enum ActiveTeam
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
        /// <summary>
        /// This method initializes the game by creating tokens and placing them in their starting positions.
        /// </summary>
        private void InitializeGame()
        {
            //Original placement of tokens
            teamRed.AddToken(new Token("Red1", 10, 1, Colors.Red));
            teamRed.AddToken(new Token("Red2", 13, 1, Colors.Red));
            teamRed.AddToken(new Token("Red3", 10, 4, Colors.Red));
            teamRed.AddToken(new Token("Red4", 13, 4, Colors.Red));

            //Only for testing purposes to get tokens to goal faster
            //teamRed.AddToken(new Token("Red1", 10, 1, Colors.Red));
            //teamRed.AddToken(new Token("Red2", 13, 1, Colors.Red));
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

            if (loadGame)
            {
                LoadGameState();
            }

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
            //InitializeGame();
            
            Debug.WriteLine("Current active team: " + currentActiveTeam);

            PlayBackgroundMusic();
        }

        //Return user type from startmenutogame.xaml
        private bool IsUserTypeCpu()
        {
            return (bool)Application.ReferenceEquals(typeof(Startmenutogame), "Cpu");
        }

        //DRAWING THE BOARD AND TOKENS
        /// <summary>
        /// Used to draw the game board and tokens.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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

        /// <summary>
        /// Use this method to update the game state and perform any necessary calculations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void canvas_Update(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
            
            setCellSize();
            setCanvasMargin();
            setDiceImageSize();
            GetActiveTeamColor();
            UpdateTokensGoalsOpacity();
            SetTokenGoalRowSizes();

            foreach (Models.Team team in teams.TeamList)
            {
                foreach (Token token in team.TeamTokens)
                {
                    token.UpdateAnimation();
                }
            }

        }

        /// <summary>
        /// Use this method to create any resources that will be used in the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void canvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            
        }

        /// <summary>
        /// Set the cell size based on the height of the background image and the number of columns in the grid
        /// </summary>
        private async void setCellSize()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                cellSize = (float)backgroundImage.ActualHeight / numberOfColumnsInGrid;
            });
        }

        /// <summary>
        /// This method sets the margin of the canvas so that centers the grid over the background image(game board).
        /// </summary>
        private async void setCanvasMargin()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                canvas.Margin = new Thickness((boardGrid.ActualWidth - backgroundImage.ActualWidth) / 2,
                                                (boardGrid.ActualHeight - backgroundImage.ActualHeight) / 2 - (mainStackPanel.ActualHeight / 2),
                                                (boardGrid.ActualWidth - backgroundImage.ActualWidth) / 2,
                                                (boardGrid.ActualHeight - backgroundImage.ActualHeight) / 2 - (mainStackPanel.ActualHeight / 2));
            });
        }

        /// <summary>
        /// Checks if the pointer is inside a token and if the token can be moved.
        /// Also checks if the token is at base and if the dice roll result is 6.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Returns true if the pointer is inside the bounds of the token, otherwise returns false.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="pointerPosition"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns true if the current roll is valid, otherwise returns false.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the current active team.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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
        /// <summary>
        /// Switches to the next team based on the current active team.
        /// </summary>
        private void SwitchToNextTeam()
        {
            if(diceRollResult != 6)
            {
                switch (currentActiveTeam)
                {
                    case ActiveTeam.Red:
                        currentActiveTeam = ActiveTeam.Green;
                        if(teamGreen.AI)
                        {
                            CpuPlayerRollDice();
                        }
                        break;
                    case ActiveTeam.Green:
                        currentActiveTeam = ActiveTeam.Yellow;
                        if (teamYellow.AI)
                        {
                            CpuPlayerRollDice();
                        }
                        break;
                    case ActiveTeam.Yellow:
                        currentActiveTeam = ActiveTeam.Blue;
                        if (teamBlue.AI)
                        {
                            CpuPlayerRollDice();
                        }
                        break;
                    case ActiveTeam.Blue:
                        currentActiveTeam = ActiveTeam.Red;
                        if (teamRed.AI)
                        {
                            CpuPlayerRollDice();
                        }
                        break;
                }
            }
                UpdateTurnIndicator();
        }

        /// <summary>
        /// Gets the color of the current active team.
        /// </summary>
        /// <returns>The color of the current active team</returns>
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


        /// <summary>
        /// Button event handler for the roll dice button.
        /// Disable the dice button when it's clicked and enable it again after a token has been moved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RollDiceButton_Click(object sender, RoutedEventArgs e)
        {
            selectedToken = null;
            PlayDiceSound();
            await RollDiceAnimation();
            diceRollResult = random.Next(1, 7);
            Debug.WriteLine("Dice roll result: " + diceRollResult);
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
                    if(GetCurrentTeam().AI)
                    {
                        CpuPlayerPickToken();
                        CpuPlayerRollDice();
                    }
                    
                    
                }
                else
                {
                    if(GetCurrentTeam().AI)
                    {
                        CpuPlayerPickToken();
                    }
                }
            }
            Debug.WriteLine("Current active team: " + currentActiveTeam);
        }

        /// <summary>
        /// Notifies the player that they rolled a six.
        /// </summary>
        private async void NotifyPlayerForSix()
        {
            if (!GetCurrentTeam().AI)
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
        }

        private MediaPlayer mediaPlayer = new MediaPlayer();
        private void PlayDiceSound()
        {
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/dice_roll.mp3"));
            mediaPlayer.Play();
        }

        /// <summary>
        /// Changes the dice image at a set interval to create an animation.
        /// </summary>
        private async Task RollDiceAnimation()
        {
            string[] diceImages = { "dice_1.png", "dice_2.png", "dice_3.png", "dice_4.png", "dice_5.png", "dice_6.png" };
            for (int i = 0; i < 10; i++)
            {
                DiceImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/" + diceImages[random.Next(0, 6)]));
                await Task.Delay(100);  // Adjust delay as per your needs.
            }
        }

        /// <summary>
        /// Sets the size of the dice image and victory image.
        /// </summary>
        private async void setDiceImageSize()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                DiceImage.Width = cellSize * 1.5;
                DiceImage.Height = cellSize * 1.5;
            });

        }
        //check if all tokens in current active team are at goal
        /// <summary>
        /// Check to see if all tokens in the current active team are at goal.
        /// </summary>
        /// <returns>True if all tokens of the current team is at goal, otherwise returns False</returns>
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

        /// <summary>
        /// Cheks how many steps a token has to take to reach the goal to determine if a move towards the goal is valid.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>amount of steps left to reach the goal</returns>
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

        /// <summary>
        /// Checks if the current team has won the game.
        /// </summary>
        private bool HasCurrentTeamWon()
        {
            Models.Team activeTeam = GetCurrentTeam();

            return activeTeam.TeamTokens.All(token => token.IsAtGoal(grid));
        }

        /// <summary>
        /// Prompts the user to choose whether they want to play again or return to the main menu.
        /// </summary>
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
                ResetGameState(); 
            }
            else
            {
                ReturnToMainMenu(); 
            }
        }

        /// <summary>
        /// Navigate back to the main menu.
        /// </summary>
        private void ReturnToMainMenu()
        {
            //RestartGame();

            // Navigate back to the main menu.
            Frame.Navigate(typeof(Startmenutogame));
        }

        /// <summary>
        /// Changes the opacity of the Tokens in the goal to 1 if they are inside the goal.
        /// </summary>
        private async void UpdateTokensGoalsOpacity()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                for(int i = 0; i < 4; i++)
                {
                    if (teamRed.TeamTokens[i].isInsideGoal)
                    {
                        redTokensGoal.Children[i].Opacity = 1;
                    }

                    if (teamGreen.TeamTokens[i].isInsideGoal)
                    {
                        greenTokensGoal.Children[i].Opacity = 1;
                    }

                    if (teamYellow.TeamTokens[i].isInsideGoal)
                    {
                        yellowTokensGoal.Children[i].Opacity = 1;
                    }

                    if (teamBlue.TeamTokens[i].isInsideGoal)
                    {
                        blueTokensGoal.Children[i].Opacity = 1;
                    }

                }
            });
        }

        /// <summary>
        /// Set the sizes for elements inside the Goal row below the game board.
        /// </summary>
        private async void SetTokenGoalRowSizes()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                int minFontSize = 12;
                mainStackPanel.Width = backgroundImage.ActualWidth;
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

        /// <summary>
        /// Reset the game state to the initial state.
        /// </summary>
        private void ResetGameState()
        {
            // Reset the position of all tokens to their original positions
            foreach(Token token in AllTokens())
            {
                token.resetToken();
            }

            menuPopup.IsOpen = false;
            // Reset the current active team to a random team
            currentActiveTeam = (ActiveTeam)new Random().Next(0, 4);

            // Reset other necessary game state variables
            selectedToken = null;
            diceRollResult = 0;
            EnableDiceClick();

            // Possibly invalidate the canvas to redraw the initial game state
            canvas.Invalidate();

            Debug.WriteLine("Game state reset. Current active team: " + currentActiveTeam);
        }

        MediaPlayer backgroundMusicPlayer = new MediaPlayer();
        public void PlayBackgroundMusic()
        {
            backgroundMusicPlayer.AutoPlay = true;
            backgroundMusicPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/background_music.mp3"));
            backgroundMusicPlayer.IsLoopingEnabled = true;  // If you want the music to loop
            backgroundMusicPlayer.Volume = 0.5;  // Adjust volume as needed
            backgroundMusicPlayer.Play();
        }

        // Field to track mute state
        private bool isMuted = false;

        // Event handler for mute/unmute button click
        private void MuteUnmuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (isMuted)
            {
                // Currently muted, so unmute
                backgroundMusicPlayer.IsMuted = false;
                MuteUnmuteButton.Content = new TextBlock { Text = "Mute", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }; // Change button text to "Mute"
                isMuted = false;
            }
            else
            {
                // Currently unmuted, so mute
                backgroundMusicPlayer.IsMuted = true;
                MuteUnmuteButton.Content = new TextBlock { Text = "Unmute", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }; // Change button text to "Unmute"
                isMuted = true;
            }
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            menuPopup.IsOpen = true;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            menuPopup.IsOpen = true;
        }

        private void RulesButton_Click(object sender, RoutedEventArgs e)
        {
            rulesPopup.IsOpen = true;

         

            /*Popup rulesPopup = new Popup();
 
            Image photo = new Image();
            photo.Source = new BitmapImage(new Uri("ms-appx:///Assets/Mask group (3).png"));

            rulesPopup.Child = photo;

            rulesPopup.IsOpen = true;

            await Task.Delay(10000); 
            rulesPopup.IsOpen = false;*/


        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the pop-up
            rulesPopup.IsOpen = false;
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            menuPopup.IsOpen = false;
        }
        private void SaveQuitButton_Click(object sender, RoutedEventArgs e)
        {
            SaveGameState();
            Frame.Navigate(typeof(MainPage));
        }
        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        //This does not work for some reason....
        private void menuPopup_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
            {
                if (menuPopup.IsOpen)
                {
                    menuPopup.IsOpen = false;
                }
                else
                {
                    menuPopup.IsOpen = true;
                }
            }
        }

        /// <summary>
        /// This method is used to simulate a CPU player rolling the dice.
        /// </summary>
        private async void CpuPlayerRollDice()
        {
            await Task.Delay(2000);
            Debug.WriteLine("CPU turn");
            RollDiceButton_Click(this,null);
        }

        /// <summary>
        /// This method is used to simulate a CPU player picking a token and moving it.
        /// </summary>
        /// TODO: Check so that you can't walk into goal unless you rolled the exact amount of steps needed to reach goal.
        private async void CpuPlayerPickToken() 
        {   
            await Task.Delay(2000);
            if(diceRollResult == 6)
            {
                selectedToken = GetCurrentTeam().TeamTokens[new Random().Next(0, 4)];
            }
            else
            {
                List<Token> tokensNotInBase = new List<Token>();
                foreach (Token token in GetCurrentTeam().TeamTokens)
                {
                    int stepsToGoal = StepsToGoal(token);
                    if (!token.isAtBase(grid) && diceRollResult <= stepsToGoal)
                    {
                        tokensNotInBase.Add(token);
                    }
                }
                selectedToken = tokensNotInBase[new Random().Next(0, tokensNotInBase.Count)];
            }
            selectedToken.MoveToken(selectedToken, diceRollResult, grid, AllTokens());
            SwitchToNextTeam();
            Debug.WriteLine("Current active team: " + currentActiveTeam);
            EnableDiceClick();
        }

        /// <summary>
        /// Gathers the user selections from the start menu and sets the AI property of the teams.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Dictionary<string, string> userSelections)
            {
                teamGreen.AI = userSelections["Green"] == "Ai";
                teamRed.AI = userSelections["Red"] == "Ai";
                teamYellow.AI = userSelections["Yellow"] == "Ai";
                teamBlue.AI = userSelections["Blue"] == "Ai";
                
                Debug.WriteLine("Green: " + teamGreen.AI);
                Debug.WriteLine("Red: " + teamRed.AI);
                Debug.WriteLine("Yellow: " + teamYellow.AI);
                Debug.WriteLine("Blue: " + teamBlue.AI);

                //if (GetCurrentTeam().AI)
                //{
                //    CpuPlayerRollDice();
                //}
                //Debug.WriteLine("Current active team: " + currentActiveTeam);
            }

            if(e.Parameter is bool loadGameState)
            {
                loadGame = loadGameState;
            }

            InitializeGame();
            UpdateTurnIndicator();

            if (GetCurrentTeam().AI)
            {
                CpuPlayerRollDice();
            }
            Debug.WriteLine("Current active team: " + currentActiveTeam);
        }

        /// <summary>
        /// Manly used to pause the canvas when the user navigates away from the game.
        /// This prevents the game from having unfinished methods running in the background
        /// Causing the game to Freeze.
        /// </summary>
        /// <param name="e"></param>
        /// TODO: Fix so that the AI does not roll the dice after the user navigates away from the game.
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Turn off the game
            StopGame();
        }

        /// <summary>
        /// Pauses Canvas, stops background music and resets the game state.
        /// </summary>
        private void StopGame()
        {
            // 1. Stopping animations or updates
            if (canvas != null)
            {
                canvas.Paused = true;
            }

            StopBackgroundMusic();

            foreach(Team team in teams.TeamList)
            {
                team.AI = false;
            }
            ResetGameState();
        }

        /// <summary>
        /// Pauses the background music
        /// </summary>
        private void StopBackgroundMusic()
        {
            backgroundMusicPlayer.Pause();
        }

        /// <summary>
        /// Save the current game state to the database.
        /// </summary>
        private void SaveGameState()
        {
            // Create a list of teams to save
            List<Team> teamsToSave = new List<Team> { teamRed, teamGreen, teamYellow, teamBlue };

            // Create a new GameState object with current game data
            var gameState = new Models.GameState(teamsToSave, currentActiveTeam);

            // Serialize the game state to a JSON string
            string serializedState = gameState.SerializeGameState();

            // Save the serialized state to the database
            DataAccess.SetGameState(serializedState);
        }

        /// <summary>
        /// Set the game state to the saved state.
        /// </summary>
        private void LoadGameState()
        {
            // Get the serialized game state from the database
            string serializedState = DataAccess.GetGameState();

            if (string.IsNullOrEmpty(serializedState))
                return; // No saved state available

            // Create a new empty GameState object
            var gameState = new Models.GameState(null, ActiveTeam.Red);

            // Deserialize the saved state into the gameState object
            gameState.DeserializeGameState(serializedState);

            // Restore game properties from the gameState object
            teamRed = gameState.Teams.FirstOrDefault(t => t.TeamColor == Colors.Red);
            teamGreen = gameState.Teams.FirstOrDefault(t => t.TeamColor == Colors.Green);
            teamYellow = gameState.Teams.FirstOrDefault(t => t.TeamColor == Colors.Yellow);
            teamBlue = gameState.Teams.FirstOrDefault(t => t.TeamColor == Colors.Blue);
            currentActiveTeam = gameState.CurrentActiveTeam;
        }

        private async void UpdateTurnIndicator()
        {
            var currentTeam = currentActiveTeam;
            turnIndicatorTextBlock.Text = $"Turn: Team {currentTeam}";

            Color teamColor;
            switch (currentTeam)
            {
                case ActiveTeam.Red:
                    teamColor = Colors.Red;
                    break;
                case ActiveTeam.Green:
                    teamColor = Colors.Green;
                    break;
                case ActiveTeam.Blue:
                    teamColor = Colors.Blue;
                    break;
                case ActiveTeam.Yellow:
                    teamColor = Colors.Yellow;
                    break;
                default:
                    teamColor = Colors.Black; // Default color.
                    break;
            }

            turnIndicatorTextBlock.Foreground = new SolidColorBrush(teamColor);

        }


    }


}
