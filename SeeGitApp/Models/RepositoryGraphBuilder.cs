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
            catch (LibGit2SharpException)
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
                       .QueryBy(new CommitFilter { SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time | CommitSortStrategies.Reverse, IncludeReachableFrom = _repository.Refs })
                       .ForEach(AddCommit);
            AddTagAnnotations();
            AddReferences();
            AddUnreachableCommits();
            // todo add notes?
            AddStagedPreCommitCommit();
            AddWorkingTree();
            graph.Set(_contents);
            return graph;
        }

        private void AddWorkingTree()
        {
            if (!_parameters.IncludeWorkTrees)
            {
                return;
            }
            // PRN _repository.Worktrees (is empty but should have a list like `git worktree list`)
            //  for now just assume one worktree at repo path

            var files = _repository.RetrieveStatus(new StatusOptions { IncludeUntracked = true, DetectRenamesInWorkDir = true });
            // TODO other StatusOptions?

            var workTreeVertex = new WorkTreeVertex();
            _contents.AddVertex(workTreeVertex);
            foreach (var file in files)
            {
                if (file.State == FileStatus.NewInIndex || file.State == FileStatus.ModifiedInIndex || file.State == FileStatus.RenamedInIndex || file.State == FileStatus.DeletedFromIndex)
                {
                    // don't add index only file statuses... TODO hrm if something is in index and in work tree with changes that differ. Is it returned twice?
                    continue;
                }

                // TODO if deleted in worktree... skip object id or?!
                // TODO later on spend some time on edge cases here... I dont wanna do this all day, gotta stop soon

                // treat renamed the same as new/changed, just use contents to lookup id each time
                // get sha for the new/changed file:
                using (var stream = System.IO.File.OpenRead(System.IO.Path.Combine(_repository.Info.WorkingDirectory, file.FilePath)))
                {
                    // FYI this is expensive but should happen rarely? I COULD add caching based on file path + timestamp of last edit? would that even help in most cases? remember this is for demo purposes... ONLY OPTIMIZE if I NOTICE THE ISSUE
                    // TODO can I just pass a path to ObjectDatabase to read the file for me too?
                    var blob = _repository.ObjectDatabase.CreateBlob(stream);
                    var objectId = blob.Sha;
                    var vertex = new WorkTreeEntryVertex(objectId, file.FilePath, file.State);
                    _contents.AddVertex(vertex);
                    _contents.AddEdge(new GraphContents.Edge { Source = workTreeVertex.Key, Target = vertex.Key });
                }
            }

            // TODO, link WorkTree vertex with an edge to Staged to show chain of changes?
        }

        private void AddStagedPreCommitCommit()
        {
            if (!_parameters.IncludeStaged)
            {
                return;
            }

            var staged = new StagedVertex();
            _contents.AddVertex(staged);
            _repository.Index.ForEach(e => AddStagedEntry(e, staged));

            // resolve ref chain (i.e. HEAD => master => commit, or detached HEAD => commit, etc. ):
            var headCommitId = _repository.Head.Reference.ResolveToDirectReference().TargetIdentifier; // this is the commit sha
            var toFutureParentCommit = new GraphContents.Edge { Source = staged.Key, Target = headCommitId };
            _contents.AddEdge(toFutureParentCommit);
        }

        private void AddStagedEntry(IndexEntry entry, StagedVertex staged)
        {
            var status = _repository.RetrieveStatus(entry.Path);
            var entryVertex = new StagedEntryVertex(entry, status);
            _contents.AddVertex(entryVertex);
            _contents.AddEdge(new GraphContents.Edge { Source = staged.Key, Target = entryVertex.Key });
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
            var commit = _repository.Lookup(commitSha, ObjectType.Commit) as Commit;
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
            var edge = new GraphContents.Edge { Source = tag.Annotation.Sha, Target = tag.Annotation.Target.Sha };
            _contents.AddEdge(edge);
        }

        private void AddReferences()
        {
            var references = _repository.Refs.Union(new[] { _repository.Refs["HEAD"] })
                                        .Select(b => new ReferenceVertex(b.CanonicalName, b.TargetIdentifier))
                                        .ToArray();

            references.ForEach(b => _contents.AddVertex(b));
            references
                .Select(r => new GraphContents.Edge { Source = r.Key, Target = r.TargetId })
                .ForEach(e => _contents.AddEdge(e));
        }

        private void AddCommit(Commit commit)
        {
            var commitVertex = new CommitVertex(commit);
            _contents.AddVertex(commitVertex);
            commit.Parents.ForEach(AddCommit);
            commit.Parents
                  .Select(p => new GraphContents.Edge { Source = commit.Sha, Target = p.Sha })
                  .ForEach(edge => _contents.AddEdge(edge));
            if (_parameters.IncludeCommitContent)
            {
                AddTree(commit.Tree);
                var edge = new GraphContents.Edge { Source = commit.Sha, Target = commit.Tree.Sha, Tag = "/" };
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
            var edge = new GraphContents.Edge { Source = tree.Sha, Target = entry.Target.Sha, Tag = entry.Name };
            _contents.AddEdge(edge);
        }

        private void AddBlob(Blob blob)
        {
            var blobVertex = new BlobVertex(blob);
            _contents.AddVertex(blobVertex);
        }
    }
}