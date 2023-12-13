using AdminClient.ViewModel.Pages;
using System.Windows.Controls;

namespace AdminClient.View.Pages
{
    /// <summary>
    /// Interaction logic for UserAdminView.xaml
    /// </summary>
    public partial class UserAdminView : UserControl
    {
        public UserAdminViewModel ViewModel { get; }

        public UserAdminView()
        {

            InitializeComponent();
            ViewModel = new UserAdminViewModel();
            DataContext = ViewModel;
        }
    }
}
