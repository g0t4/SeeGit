namespace SeeGit.Models
{
    using System;
    using System.Linq;
    using BclExtensionMethods;
    using LibGit2Sharp;

    public class RepositoryGraphBuilder : IRepositoryGraphBuilder
    {
        public string GitRepositoryPath { get; private set; }
        private readonly Repository _repository;
        private RepositoryGraph _graph = new RepositoryGraph();
        private GraphParameters _parameters;

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

        public RepositoryGraph Graph(GraphParameters parameters)
        {
            _parameters = parameters;
            if (_repository == null)
            {
                return _graph;
            }

            _repository.Commits
                       .QueryBy(new Filter {SortBy = GitSortOptions.Topological | GitSortOptions.Time, Since = _repository.Refs})
                       .ForEach(AddCommit);
            AddTagAnnotations();
            AddReferences();
            // todo unreachable commits
            return _graph;
        }

        private void AddTagAnnotations()
        {
            _repository.Tags
                       .Where(t => t.Annotation != null)
                       .ForEach(AddTagAnnotation);
        }

        private void AddTagAnnotation(Tag tag)
        {
            var vertex = new TagAnnotationVertex(tag.Annotation);
            _graph.AddObject(vertex);
            _graph.AddObjectEdge(tag.Annotation.Sha, tag.Annotation.Target.Sha, null);
        }

        private void AddReferences()
        {
            var references = _repository.Refs.Union(new[] {_repository.Refs["HEAD"]})
                                        .Select(b => new ReferenceVertex(b.CanonicalName, b.TargetIdentifier))
                                        .ToArray();
            _graph.RemoveReferencesNotIn(references.Select(r => r.CanonicalName).ToArray());

            references.ForEach(b => _graph.AddReference(b));
            references.ForEach(b => _graph.AddReferenceEdge(b));
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
            commit.Parents.ForEach(p => _graph.AddObjectEdge(commit.Sha, p.Sha, null));
            if (_parameters.IncludeCommitContent)
            {
                AddTree(commit.Tree);
                _graph.AddObjectEdge(commit.Sha, commit.Tree.Sha, "/");
            }
        }

        private void AddTree(Tree tree)
        {
            var treeVertex = new TreeVertex(tree);
            if (_graph.HasObject(treeVertex))
            {
                return;
            }
            _graph.AddObject(treeVertex);
            tree.ForEach(e => AddTreeEntry(tree, e));
        }

        private void AddTreeEntry(Tree tree, TreeEntry entry)
        {
            if (entry.Target is Tree)
            {
                AddTree(entry.Target as Tree);
                _graph.AddObjectEdge(tree.Sha, entry.Target.Sha, entry.Name);
            }
            else if (entry.Target is Blob)
            {
                AddBlob(entry.Target as Blob);
                _graph.AddObjectEdge(tree.Sha, entry.Target.Sha, entry.Name);
            }
            else
            {
                throw new NotSupportedException("Invalid tree entry, not supported: " + entry.Target.GetType());
            }
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