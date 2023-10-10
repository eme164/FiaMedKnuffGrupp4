using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace FiaMedKnuffGrupp4
{
    public sealed partial class MainPage
    {
        
        public MainPage()
        {
            this.InitializeComponent();
        }


        private void Button_Start(object sender, RoutedEventArgs e)
        {
            //show the GameBoard page
            this.Frame.Navigate(typeof(GameBoard));
        }

        private void Button_Create(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Load(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Score(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Quit(object sender, RoutedEventArgs e)
        {

        }

       
    }
}