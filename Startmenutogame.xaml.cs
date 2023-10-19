using FiaMedKnuffGrupp4.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static FiaMedKnuffGrupp4.Models.Team;
using static FiaMedKnuffGrupp4.Startmenutogame;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FiaMedKnuffGrupp4
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Startmenutogame : Page
    {
        public class PlayerTypeData
        {
            public Team.PlayerTypeEnum GreenPlayerType { get; set; }
            public Team.PlayerTypeEnum RedPlayerType { get; set; }
            public Team.PlayerTypeEnum YellowPlayerType { get; set; }
            public Team.PlayerTypeEnum BluePlayerType { get; set; }
        }

        public Team.PlayerTypeEnum SelectedGreenPlayerType { get; set; } = Team.PlayerTypeEnum.User;
        public Team.PlayerTypeEnum SelectedRedPlayerType { get; set; } = Team.PlayerTypeEnum.User;
        public Team.PlayerTypeEnum SelectedYellowPlayerType { get; set; } = Team.PlayerTypeEnum.User;
        public Team.PlayerTypeEnum SelectedBluePlayerType { get; set; } = Team.PlayerTypeEnum.User;

        public Startmenutogame()
        {
            this.InitializeComponent();
            InitializeFlipViews();
        }

        private void InitializeFlipViews()
        {
            greenFlipView.ItemsSource = Enum.GetNames(typeof(Team.PlayerTypeEnum)).ToList();
            redFlipView.ItemsSource = Enum.GetNames(typeof(Team.PlayerTypeEnum)).ToList();
            yellowFlipView.ItemsSource = Enum.GetNames(typeof(Team.PlayerTypeEnum)).ToList();
            blueFlipView.ItemsSource = Enum.GetNames(typeof(Team.PlayerTypeEnum)).ToList();
        }

        /// <summary>
        /// Handles the button click event to start the game with selected player types.
        /// </summary>
        /// <param name="sender">The sender of the event (the button).</param>
        /// <param name="e">The event arguments.</param>
        private void Button_Start(object sender, RoutedEventArgs e)
        {
            // Create a PlayerTypeData object with the selected player types.
            PlayerTypeData playerTypeData = new PlayerTypeData
            {
                GreenPlayerType = SelectedGreenPlayerType,
                RedPlayerType = SelectedRedPlayerType,
                YellowPlayerType = SelectedYellowPlayerType,
                BluePlayerType = SelectedBluePlayerType
            };

            // Serialize the PlayerTypeData object to JSON.
            string param = JsonConvert.SerializeObject(playerTypeData);

            // Output the serialized PlayerTypeData for debugging purposes.
            Debug.WriteLine("Serialized PlayerTypeData: " + param);

            // Navigate to the GameBoard page with the serialized player type data as a parameter.
            this.Frame.Navigate(typeof(GameBoard), param);
        }



        private void Button_Continue(object sender, RoutedEventArgs e)
        {

        }

        private void StartButton_Drop(object sender, DragEventArgs e)
        {

        }

        private void ContinueButton_Drop(Object sender, DragEventArgs e)
        {

        }

        //stuff

        /// <summary>
        /// Event handler for the click event of the Green Player button.
        /// This method is called when the Green Player button is clicked.
        /// It toggles the selected player type for the Green team between "User" and "AI".
        /// </summary>
        /// <param name="sender">The object that triggered the event (Green Player button).</param>
        /// <param name="e">Event arguments, not used in this method.</param>
        private void GreenPlayer_Click(object sender, RoutedEventArgs e)
        {
            // Output the current selected player type for debugging purposes.
            Debug.WriteLine($"Before toggle: {SelectedGreenPlayerType}");

            // Toggle the selected player type for the Green team.
            SelectedGreenPlayerType = TogglePlayerType(SelectedGreenPlayerType);

            // Output the updated selected player type for debugging purposes.
            Debug.WriteLine($"After toggle: {SelectedGreenPlayerType}");
        }

        /// <summary>
        /// Event handler for the click event of the Yellow Player button.
        /// This method is called when the Yellow Player button is clicked.
        /// It toggles the selected player type for the Yellow team between "User" and "AI".
        /// </summary>
        /// <param name="sender">The object that triggered the event (Yellow Player button).</param>
        /// <param name="e">Event arguments, not used in this method.</param>
        private void YellowPlayer_Click(object sender, RoutedEventArgs e)
        {
            // Output the current selected player type for debugging purposes.
            Debug.WriteLine($"Before toggle: {SelectedYellowPlayerType}");

            // Toggle the selected player type for the Yellow team.
            SelectedYellowPlayerType = TogglePlayerType(SelectedYellowPlayerType);

            // Output the updated selected player type for debugging purposes.
            Debug.WriteLine($"After toggle: {SelectedYellowPlayerType}");
        }

        /// <summary>
        /// Event handler for the click event of the Red Player button.
        /// This method is called when the Red Player button is clicked.
        /// It toggles the selected player type for the Red team between "User" and "AI".
        /// </summary>
        /// <param name="sender">The object that triggered the event (Red Player button).</param>
        /// <param name="e">Event arguments, not used in this method.</param>
        private void RedPlayer_Click(object sender, RoutedEventArgs e)
        {
            // Output the current selected player type for debugging purposes.
            Debug.WriteLine($"Before toggle: {SelectedRedPlayerType}");

            // Toggle the selected player type for the Red team.
            SelectedRedPlayerType = TogglePlayerType(SelectedRedPlayerType);

            // Output the updated selected player type for debugging purposes.
            Debug.WriteLine($"After toggle: {SelectedRedPlayerType}");
        }

        /// <summary>
        /// Event handler for the click event of the Blue Player button.
        /// This method is called when the Blue Player button is clicked.
        /// It toggles the selected player type for the Blue team between "User" and "AI".
        /// </summary>
        /// <param name="sender">The object that triggered the event (Blue Player button).</param>
        /// <param name="e">Event arguments, not used in this method.</param>
        private void BluePlayer_Click(object sender, RoutedEventArgs e)
        {
            // Output the current selected player type for debugging purposes.
            Debug.WriteLine($"Before toggle: {SelectedBluePlayerType}");

            // Toggle the selected player type for the Blue team.
            SelectedBluePlayerType = TogglePlayerType(SelectedBluePlayerType);

            // Output the updated selected player type for debugging purposes.
            Debug.WriteLine($"After toggle: {SelectedBluePlayerType}");
        }

        /// <summary>
        /// Toggles the player type between "User" and "AI".
        /// If the current player type is "User", it returns "AI", and vice versa.
        /// </summary>
        /// <param name="currentPlayerType">The current player type to toggle.</param>
        /// <returns>The toggled player type ("User" if it was "AI", or "AI" if it was "User").</returns>
        private Team.PlayerTypeEnum TogglePlayerType(Team.PlayerTypeEnum currentPlayerType)
        {
            // Check if the current player type is "User".
            if (currentPlayerType == Team.PlayerTypeEnum.User)
            {
                // If it is, toggle it to "AI" and return the updated player type.
                return Team.PlayerTypeEnum.Ai;
            }
            else
            {
                // If it's not "User" (i.e., it's "AI"), toggle it to "User" and return the updated player type.
                return Team.PlayerTypeEnum.User;
            }
        }
    }
}
