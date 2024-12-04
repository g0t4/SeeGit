namespace SeeGit
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using Models;

    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel = new MainWindowViewModel(Dispatcher, path => new RepositoryGraphBuilder(path));

            // FYI load a dir when testing, comment out when done (so dont have to browse to one each time):
            _viewModel.MonitorRepository("C:\\Users\\wes\\repos\\github\\g0t4\\SeeGit");
        }

        private void OnChooseRepository(object sender, RoutedEventArgs args)
        {
            _viewModel.MonitorRepository(WindowsExtensions.BrowseForFolder(Environment.GetFolderPath(Environment.SpecialFolder.Personal)));
        }

        private void OnRefresh(object sender, ExecutedRoutedEventArgs e)
        {
            _viewModel.MonitorRepository(_viewModel.RepositoryPath);
        }
    }
}