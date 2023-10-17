using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FiaMedKnuffGrupp4
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Startmenutogame : Page
    {
        public Startmenutogame()
        {
            this.InitializeComponent();
        }

        private void Button_Start(object sender, RoutedEventArgs e)
        {
            //show the GameBoard page
            this.Frame.Navigate(typeof(GameBoard));
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
    }
}
