namespace SeeGit.Models
{
    using System;
    using System.Linq;
    using BclExtensionMethods;
    using LibGit2Sharp;
    using Vertices;

    public class RepositoryGraphBuilder : IRepositoryGraphBuilder
    {
        public string GitRepositoryPath { get; private set; }
        private readonly Repository _repository;
        private GraphParameters _parameters;
        private GraphContents _contents;

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
            var graph = new RepositoryGraph();
            if (_repository == null)
            {
                return graph;
            }
            _contents = new GraphContents();
            _repository.Commits
                       .QueryBy(new Filter {SortBy = GitSortOptions.Topological | GitSortOptions.Time, Since = _repository.Refs})
                       .ForEach(AddCommit);
            AddTagAnnotations();
            AddReferences();
            AddUnreachableCommits();
            // todo add notes?
            AddIndex();
            graph.Set(_contents);
            return graph;
        }

        private void AddIndex()
        {
            if (!_parameters.IncludeIndex)
            {
                return;
            }

            var index = new IndexVertex();
            _contents.AddVertex(index);
            _repository.Index.ForEach(e => AddIndexEntry(e, index));
        }

        private void AddIndexEntry(IndexEntry entry, IndexVertex index)
        {
            var entryVertex = new IndexEntryVertex(entry);
            _contents.AddVertex(entryVertex);
            _contents.AddEdge(new GraphContents.Edge {Source = index.Key, Target = entryVertex.Key});
        }

        private void AddUnreachableCommits()
        {
            if (!_parameters.IncludeUnreachableCommits)
            {
                return;
            }

            GitExtensions.GetUnreachableCommitShas(GitRepositoryPath)
                         .ForEach(AddCommit);
        }

        private void AddCommit(string commitSha)
        {
            var commit = _repository.Lookup(commitSha, GitObjectType.Commit) as Commit;
            AddCommit(commit);
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
            _contents.AddVertex(vertex);
            var edge = new GraphContents.Edge {Source = tag.Annotation.Sha, Target = tag.Annotation.Target.Sha};
            _contents.AddEdge(edge);
        }

        private void AddReferences()
        {
            var references = _repository.Refs.Union(new[] {_repository.Refs["HEAD"]})
                                        .Select(b => new ReferenceVertex(b.CanonicalName, b.TargetIdentifier))
                                        .ToArray();

            references.ForEach(b => _contents.AddVertex(b));
            references
                .Select(r => new GraphContents.Edge {Source = r.Key, Target = r.TargetId})
                .ForEach(e => _contents.AddEdge(e));
        }

        private void AddCommit(Commit commit)
        {
            var commitVertex = new CommitVertex(commit);
            _contents.AddVertex(commitVertex);
            commit.Parents.ForEach(AddCommit);
            commit.Parents
                  .Select(p => new GraphContents.Edge {Source = commit.Sha, Target = p.Sha})
                  .ForEach(edge => _contents.AddEdge(edge));
            if (_parameters.IncludeCommitContent)
            {
                AddTree(commit.Tree);
                var edge = new GraphContents.Edge {Source = commit.Sha, Target = commit.Tree.Sha, Tag = "/"};
                _contents.AddEdge(edge);
            }
        }

        private void AddTree(Tree tree)
        {
            var treeVertex = new TreeVertex(tree);
            _contents.AddVertex(treeVertex);
            tree.ForEach(e => AddTreeEntry(tree, e));
        }

        private void AddTreeEntry(Tree tree, TreeEntry entry)
        {
            if (entry.Target is Tree)
            {
                AddTree(entry.Target as Tree);
            }
            else if (entry.Target is Blob)
            {
                AddBlob(entry.Target as Blob);
            }
            else
            {
                throw new NotSupportedException("Invalid tree entry, not supported: " + entry.Target.GetType());
            }
            var edge = new GraphContents.Edge {Source = tree.Sha, Target = entry.Target.Sha, Tag = entry.Name};
            _contents.AddEdge(edge);
        }

        private void AddBlob(Blob blob)
        {
            var blobVertex = new BlobVertex(blob);
            _contents.AddVertex(blobVertex);
        }
    }
}