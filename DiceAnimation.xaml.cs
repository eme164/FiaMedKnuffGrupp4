using System;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace FiaMedKnuffGrupp4
{
    /// <summary>
    /// Represents a page for simulating dice rolling animation and managing player turns.
    /// </summary>
    public sealed partial class DiceAnimation : Page
    {
        private Random random = new Random();
        private int currentPlayerIndex = 0;
        private string[] players = { "Player 1", "Player 2", "Player 3", "Player 4" };

        public DiceAnimation()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Checks whether the current player's roll is valid.
        /// </summary>
        /// <returns>True if the roll is valid; otherwise, false.</returns>
        private bool IsValidRoll()
        {
            // Placeholder logic: always returns true; you can implement custom logic here.
            return true;
        }

        /// <summary>
        /// Advances to the next player's turn.
        /// </summary>
        private void NextTurn()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
            PlayerTurnTextBlock.Text = $"{players[currentPlayerIndex]}'s Turn";
        }

        /// <summary>
        /// Handles the click event of the Roll Dice button.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments.</param>
        private async void RollDiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidRoll())
            {
                PlayDiceSound();
                await RollDiceAnimation();
                int diceRollResult = random.Next(1, 7);
                DiceImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/dice_" + diceRollResult + ".png"));
                NextTurn();
            }
            else
            {
                // Inform the player that the roll is not valid, if needed.
            }
        }

        private MediaPlayer mediaPlayer = new MediaPlayer();

        /// <summary>
        /// Plays a sound when rolling the dice.
        /// </summary>
        private void PlayDiceSound()
        {
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/dice_roll.mp3"));
            mediaPlayer.Play();
        }

        /// <summary>
        /// Simulates a rolling dice animation.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task RollDiceAnimation()
        {
            string[] diceImages = { "dice_1.png", "dice_2.png", "dice_3.png", "dice_4.png", "dice_5.png", "dice_6.png" };
            for (int i = 0; i < 10; i++)
            {
                DiceImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/" + diceImages[random.Next(0, 6)]));
                await Task.Delay(100);  // Adjust delay as per your needs.
            }
        }
    }
}