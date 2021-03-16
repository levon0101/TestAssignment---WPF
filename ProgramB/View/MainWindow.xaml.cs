using System;
using System.Windows;
using ProgramB.ViewModel;

namespace ProgramB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            ContentRendered += MainWindowContentRendered;
        }

        private void MainWindowContentRendered(object sender, EventArgs e)
        {
            _viewModel.Load();
        }
         
    }
}
