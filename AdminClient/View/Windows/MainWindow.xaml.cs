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
            InitializeComponent();

            // Styling
            Color color = Colors.MediumVioletRed;
            Wpf.Ui.Appearance.Accent.Apply(color);

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            loginBtn.IsSelected = true;
            var loginView = new LoginView();
            pageContent.Content = loginView;

            // Subscribe/bind to the SuccessfulLogin event
            loginView.ViewModel.SuccessfulLogin += OnSuccessfulLogin;

            // Bind mouse click to btn
            userAdminBtn.PreviewMouseLeftButtonDown += UserAdmin_BtnClick;
        }

        private void OnSuccessfulLogin(object sender, EventArgs e)
        {
            pageContent.Content = new HomeView();
            loginBtn.IsSelected = false;
            loginBtn.IsEnabled = false;
            homeBtn.IsSelected = true;
            homeBtn.IsEnabled = true;
            userAdminBtn.IsEnabled = true;
        }

        private void UserAdmin_BtnClick(object sender, RoutedEventArgs e)
        {
            var userAdminView = new UserAdminView();
            pageContent.Content = userAdminView;
        }
    }
}