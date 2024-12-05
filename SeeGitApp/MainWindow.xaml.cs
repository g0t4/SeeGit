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
            _viewModel.MonitorRepository("C:\\Users\\wes\\repos\\scratch\\test-small");
            //_viewModel.MonitorRepository("C:\\Users\\wes\\repos\\scratch\\test-new"); // delete and open and then git init after app running to show it detects new repo creation!
            //_viewModel.MonitorRepository("C:\\Users\\wes\\repos\\scratch\\test-mini");

            //_viewModel.MonitorRepository("C:\\Users\\wes\\repos\\github\\g0t4\\pwsh-abbr"); // ~10 commits, real repo (too many files though for content, realistically)
            // TODO can I get to a point where I can better represent bigger repos (like this one with 10 commits, or is this really just for learning in small repos with a few commits/files, which is totally cool if so as that was what I made this for)
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