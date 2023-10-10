using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FiaMedKnuffGrupp4
{
    public sealed partial class MainPage
    {
        private Random random = new Random();
        private int currentPlayerIndex = 0;
        private string[] players = { "Player 1", "Player 2", "Player 3", "Player 4" };

        public MainPage()
        {
            this.InitializeComponent();
        }

        private bool IsValidRoll()
        {
            // Game Turn Management | Ensure that the displayed dice result is updated only when it's the player's turn to roll the dice..
            return true;  // Placeholder logic: always returns true.
        }

        private void NextTurn()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
            PlayerTurnTextBlock.Text = $"{players[currentPlayerIndex]}'s Turn";
        }

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
    }
}
