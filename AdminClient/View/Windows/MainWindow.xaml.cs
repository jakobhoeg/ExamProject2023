using AdminClient.View.Pages;
using AdminClient.ViewModel.Pages;
using System.Windows;
using System.Windows.Media;

namespace AdminClient.View.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            Color color = Colors.MediumVioletRed;
            Wpf.Ui.Appearance.Accent.Apply(color);

            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            loginBtn.IsSelected = true;
            var loginView = new LoginView();
            pageContent.Content = loginView;

            loginView.ViewModel.SuccessfulLogin += OnSuccessfulLogin;

        }

        private void OnSuccessfulLogin(object sender, EventArgs e)
        {
            pageContent.Content = new HomeView();
        }
    }
}