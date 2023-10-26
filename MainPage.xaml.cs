using Windows.UI.Xaml;


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
            this.Frame.Navigate(typeof(Startmenutogame));
        }



        private void Button_Quit(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void StartButton_Drop(object sender, DragEventArgs e)
        {

        }
    }
}