namespace SeeGit.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using LibGit2Sharp;

    public class RepositoryGraphBuilder : IRepositoryGraphBuilder
    {
        public string GitRepositoryPath { get; private set; }
        private readonly RepositoryGraph _graph = new RepositoryGraph();
        private readonly Repository _repository;
        private readonly Dictionary<string, ObjectVertex> _vertices = new Dictionary<string, ObjectVertex>();
        private readonly Dictionary<GitEdge, GitEdge> _edges = new Dictionary<GitEdge, GitEdge>();

        public RepositoryGraphBuilder(string gitRepositoryPath)
        {
            GitRepositoryPath = gitRepositoryPath;
            try
            {
                _repository = new Repository(GitRepositoryPath);
            }
            catch (LibGit2Exception)
            {
            }
        }

        public RepositoryGraph Graph()
        {
            if (_repository == null) return new RepositoryGraph();

            var commits =
                _repository.Commits.QueryBy(new Filter {SortBy = GitSortOptions.Topological | GitSortOptions.Time});

            AddCommitsToGraph(commits.First(), null);

            if (_vertices.Count > 1)
            {
                _graph.LayoutAlgorithmType = "EfficientSugiyama";
            }

            return _graph;
        }

        private void AddCommitsToGraph(Commit commit, ObjectVertex child)
        {
            var commitVertex = GetCommitVertex(commit);
            _graph.AddVertex(commitVertex);
            if (child != null)
            {
                var edge = new GitEdge(child, commitVertex);
                if (_edges.ContainsKey(edge)) return;
                _graph.AddEdge(edge);
                _edges.Add(edge, edge);
            }

            foreach (var parent in commit.Parents)
            {
                AddCommitsToGraph(parent, commitVertex);
            }
        }

        private ObjectVertex GetCommitVertex(Commit commit)
        {
            ObjectVertex commitVertex;
            if (!_vertices.TryGetValue(commit.Sha, out commitVertex))
            {
                commitVertex = new CommitVertex(commit);
                _vertices.Add(commit.Sha, commitVertex);
            }
            return commitVertex;
        }
    }
}