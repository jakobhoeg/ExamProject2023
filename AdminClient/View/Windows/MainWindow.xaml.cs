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
            Color color = Colors.HotPink;
            Wpf.Ui.Appearance.Accent.Apply(color);

            InitializeComponent();
        }

    }
}