namespace SeeGit
{
    using System;
    using System.IO;
    using System.Reactive.Linq;
    using System.Windows.Threading;
    using BclExtensionMethods;
    using Models;

    public class MainWindowViewModel : NotifyPropertyChanged
    {
        private RepositoryGraph _graph;
        private string _repositoryPath;
        private string _repoDisplayName;

        private GraphParameters _graphParameters = new GraphParameters();

        private IRepositoryGraphBuilder _graphBuilder;

        private readonly Dispatcher _uiDispatcher;
        private readonly Func<string, IRepositoryGraphBuilder> _graphBuilderThunk;

        public MainWindowViewModel(Dispatcher uiDispatcher, Func<string, IRepositoryGraphBuilder> graphBuilderThunk)
        {
            _uiDispatcher = uiDispatcher;
            _graphBuilderThunk = graphBuilderThunk ?? (path => new RepositoryGraphBuilder(path));
        }
        public string LayoutAlgorithmType => StandardLayoutAlgorithms.Sugiyama;
        //  doesn't change for now, just define in real code
        public string OverlapRemovalAlgorithmType => StandardOverlapRemovalAlgorithms.FSA;

        public RepositoryGraph Graph
        {
            get { return _graph; }
            set
            {
                _graph = value;
                RaisePropertyChanged(() => Graph);
            }
        }

        public string RepositoryPath
        {
            get { return _repositoryPath; }
            private set
            {
                _repositoryPath = value;
                RaisePropertyChanged(() => RepositoryPath);
            }
        }

        public string RepoDisplayName
        {
            get { return _repoDisplayName; }
            private set
            {
                _repoDisplayName = value;
                RaisePropertyChanged(() => RepoDisplayName);
            }
        }

        public GraphParameters GraphParameters
        {
            get { return _graphParameters; }
            private set
            {
                _graphParameters = value;
                RaisePropertyChanged(() => GraphParameters);
            }
        }

        public void MonitorRepository(string repositoryWorkingPath)
        {
            if (repositoryWorkingPath == null) return;

            var gitPath = ModelExtensions.GetGitRepositoryPath(repositoryWorkingPath);
            if (!Directory.Exists(gitPath))
            {
                MonitorForRepositoryCreation(repositoryWorkingPath);
                return;
            }

            _graphBuilder = _graphBuilderThunk(gitPath);
            RepositoryPath = Directory.GetParent(gitPath).FullName;
            RepoDisplayName = Directory.GetParent(gitPath).Name;

            Refresh();

            MonitorForRepositoryChanges(gitPath);
        }

        private void MonitorForRepositoryCreation(string repositoryWorkingPath)
        {
            ModelExtensions.CreateGitRepositoryCreationObservable(repositoryWorkingPath)
                           .Subscribe(_ => _uiDispatcher.Invoke(new Action(() => MonitorRepository(repositoryWorkingPath))));
        }

        private void MonitorForRepositoryChanges(string gitRepositoryPath)
        {
            ModelExtensions.CreateGitRepositoryChangesObservable(gitRepositoryPath)
                           .Subscribe(_ => _uiDispatcher.Invoke(new Action(Refresh)));
        }

        public void Refresh()
        {
            // todo can't we split updating the UI from reading the repo and run this on a background thread?
            Graph = _graphBuilder.Graph(GraphParameters);
        }
    }

    public static class ModelExtensions
    {
        private const string GitDirectoryName = ".git";

        public static string GetGitRepositoryPath(string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            //If we are passed a .git directory, just return it straightaway
            var pathDirectoryInfo = new DirectoryInfo(path);
            if (pathDirectoryInfo.Name == GitDirectoryName)
            {
                return path;
            }

            if (!pathDirectoryInfo.Exists) return Path.Combine(path, GitDirectoryName);

            var checkIn = pathDirectoryInfo;

            while (checkIn != null)
            {
                var pathToTest = Path.Combine(checkIn.FullName, GitDirectoryName);
                if (Directory.Exists(pathToTest))
                {
                    return pathToTest;
                }
                else
                {
                    checkIn = checkIn.Parent;
                }
            }

            // This is not good, it relies on the rest of the code being ok
            // with getting a non-git repo dir
            return Path.Combine(path, GitDirectoryName);
        }

        public static IObservable<FileSystemEventArgs> ObserveFileSystemCreateEvents(this FileSystemWatcher watcher)
        {
            return Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                h => watcher.Created += h,
                h => watcher.Created -= h)
                .Select(e => e.EventArgs);
        }

        public static IObservable<FileSystemEventArgs> ObserveFileSystemChangeEvents(this FileSystemWatcher watcher)
        {
            return Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                h => watcher.Changed += h,
                h => watcher.Changed -= h)
                .Select(e => e.EventArgs);
        }

        /// <summary>
        /// Creates an observable that will fire when the git repo is created (if it doesn't yet exist), mostly to show what happens instantly when a new repo is created.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IObservable<FileSystemEventArgs> CreateGitRepositoryCreationObservable(string path)
        {
            var expectedGitDirectory = Path.Combine(path, GitDirectoryName);
            return new FileSystemWatcher(path)
            {
                IncludeSubdirectories = false,
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.DirectoryName
            }.ObserveFileSystemCreateEvents()
                 .Where(
                     e =>
                     e.ChangeType.In(WatcherChangeTypes.Created, WatcherChangeTypes.Deleted) &&
                     e.FullPath.Equals(expectedGitDirectory, StringComparison.OrdinalIgnoreCase))
                 .Throttle(TimeSpan.FromMilliseconds(250));
            // todo perhaps we want a small throttle window and reset the window each time we get a change notification, like a BufferUntilCalm
        }

        /// <summary>
        /// Observable for fs change events in the git repository
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IObservable<FileSystemEventArgs> CreateGitRepositoryChangesObservable(string path)
        {
            return new FileSystemWatcher(path)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
            }.ObserveFileSystemChangeEvents()
                 .Throttle(TimeSpan.FromMilliseconds(250));
            // todo perhaps we want a small throttle window and reset the window each time we get a change notification, like a BufferUntilCalm
        }
    }

}