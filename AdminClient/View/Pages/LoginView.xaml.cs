using AdminClient.ViewModel.Pages;
using System.Windows.Controls;

namespace AdminClient.View.Pages
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginViewModel ViewModel { get; }

        public LoginView()
        {

            InitializeComponent();
            ViewModel = new LoginViewModel();
            DataContext = ViewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.PasswordBox passwordBox)
            {
                ViewModel.Password = passwordBox.Password;
            }
        }
    }
}
