﻿namespace SeeGit
{
    using System;
    using System.IO;
    using System.Windows.Threading;
    using Models;

    public class MainWindowViewModel : NotifyPropertyChanged
    {
        private RepositoryGraph _graph;
        private string _repositoryPath;
        private GraphParameters _graphParameters = new GraphParameters();

        private IRepositoryGraphBuilder _graphBuilder;
        // Tree, LinLog, KK, ISOM, EfficientSugiyama, FR, CompoundFDP, BoundedFR, Circular

        private string _layoutAlgorithmType = "Tree";
        private readonly Dispatcher _uiDispatcher;
        private readonly Func<string, IRepositoryGraphBuilder> _graphBuilderThunk;

        public MainWindowViewModel(Dispatcher uiDispatcher, Func<string, IRepositoryGraphBuilder> graphBuilderThunk)
        {
            _uiDispatcher = uiDispatcher;
            _graphBuilderThunk = graphBuilderThunk ?? (path => new RepositoryGraphBuilder(path));
        }

        public string LayoutAlgorithmType
        {
            get { return _layoutAlgorithmType; }
            private set
            {
                _layoutAlgorithmType = value;
                RaisePropertyChanged(() => LayoutAlgorithmType);
            }
        }

        public RepositoryGraph Graph
        {
            get { return _graph; }
            set
            {
                _graph = value;
                LayoutAlgorithmType = _graph.LayoutAlgorithmType;
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

            RepositoryPath = repositoryWorkingPath;

            _graphBuilder = _graphBuilderThunk(gitPath);
            RepositoryPath = Directory.GetParent(gitPath).Name;
            LayoutAlgorithmType = "Tree";

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
}