namespace SeeGit.Models
{
    using BclExtensionMethods;
    using LibGit2Sharp;

    public class RepositoryGraphBuilder : IRepositoryGraphBuilder
    {
        public string GitRepositoryPath { get; private set; }
        private readonly Repository _repository;
        private RepositoryGraph _graph = new RepositoryGraph();

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
            if (_repository == null)
            {
                return _graph;
            }

            _repository.Commits
                       .QueryBy(new Filter {SortBy = GitSortOptions.Topological | GitSortOptions.Time})
                       .ForEach(AddCommit);

            return _graph;
        }

        private void AddCommit(Commit commit)
        {
            var commitVertex = new CommitVertex(commit);
            if (_graph.HasObject(commitVertex))
            {
                return;
            }
            _graph.AddObject(commitVertex);
            commit.Parents.ForEach(AddCommit);
            AddTree(commit.Tree);
            commit.Parents.ForEach(p => _graph.AddObjectEdge(commit.Sha, p.Sha));
            _graph.AddObjectEdge(commit.Sha, commit.Tree.Sha);
        }

        private void AddTree(Tree tree)
        {
            var treeVertex = new TreeVertex(tree);
            if (_graph.HasObject(treeVertex))
            {
                return;
            }
            _graph.AddObject(treeVertex);
            tree.Trees.ForEach(AddTree);
            tree.Blobs.ForEach(AddBlob);
            tree.Trees.ForEach(t => _graph.AddObjectEdge(tree.Sha, t.Sha));
            tree.Blobs.ForEach(b => _graph.AddObjectEdge(tree.Sha, b.Sha));
        }

        private void AddBlob(Blob blob)
        {
            var blobVertex = new BlobVertex(blob);
            if (_graph.HasObject(blobVertex))
            {
                return;
            }
            _graph.AddObject(blobVertex);
        }
    }
}