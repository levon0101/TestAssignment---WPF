using System.Windows;
using ProgramA.ViewModel;

namespace ProgramA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        public MainWindow(MainViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = _viewModel;

            InitializeComponent();

            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Load();
        }
    }
}
