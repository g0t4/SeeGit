namespace UnitTests
{
    using System.Linq;
    using NUnit.Framework;
    using SeeGit.Models;

    public class RepositoryGraphTests
    {
        public class AddObject : AssertionHelper
        {
            [Test]
            public void Adds_NewObject()
            {
                var repository = new RepositoryGraph();
                var commit = new CommitVertex("sha1", string.Empty);
                repository.AddObject(commit);

                Expect(repository.Vertices.Single(), Is.EqualTo(commit));
            }

            [Test]
            public void Ignore_ExistingObject()
            {
                var repository = new RepositoryGraph();
                var commit = new CommitVertex("sha1", string.Empty);
                repository.AddObject(commit);
                var commitSame = new CommitVertex("sha1", string.Empty);

                repository.AddObject(commitSame);

                Expect(repository.Vertices.Single(), Is.EqualTo(commit));
            }
        }

        public class AddObjectEdge : AssertionHelper
        {
            [Test]
            public void Adds_NewEdge()
            {
                var repository = new RepositoryGraph();
                var commitA = new CommitVertex("shaA", string.Empty);
                var commitB = new CommitVertex("shaB", string.Empty);
                repository.AddObject(commitA);
                repository.AddObject(commitB);

                repository.AddObjectEdge(commitA.Sha, commitB.Sha, null);

                var edge = repository.Edges.Single();
                Expect(edge.Source, Is.EqualTo(commitA));
                Expect(edge.Target, Is.EqualTo(commitB));
            }

            [Test]
            public void Ignores_ExistingEdge()
            {
                var repository = new RepositoryGraph();
                var commitA = new CommitVertex("shaA", string.Empty);
                var commitB = new CommitVertex("shaB", string.Empty);
                repository.AddObject(commitA);
                repository.AddObject(commitB);
                repository.AddObjectEdge(commitA.Sha, commitB.Sha, null);

                repository.AddObjectEdge(commitA.Sha, commitB.Sha, null);

                Expect(repository.Edges.Count(), Is.EqualTo(1));
            }

            [Test]
            public void Ignores_SameEdgeSameTag()
            {
                var repository = new RepositoryGraph();
                var commitA = new CommitVertex("shaA", string.Empty);
                var commitB = new CommitVertex("shaB", string.Empty);
                repository.AddObject(commitA);
                repository.AddObject(commitB);
                repository.AddObjectEdge(commitA.Sha, commitB.Sha, "tag1");

                repository.AddObjectEdge(commitA.Sha, commitB.Sha, "tag1");

                Expect(repository.Edges.Single().Tag.Count(), Is.EqualTo(1));
            }

            [Test]
            public void AddsSecondTag_SameEdgeDifferentTag()
            {
                var repository = new RepositoryGraph();
                var commitA = new CommitVertex("shaA", string.Empty);
                var commitB = new CommitVertex("shaB", string.Empty);
                repository.AddObject(commitA);
                repository.AddObject(commitB);
                repository.AddObjectEdge(commitA.Sha, commitB.Sha, "tag1");

                repository.AddObjectEdge(commitA.Sha, commitB.Sha, "tag2");

                var tag = repository.Edges.Single().Tag;
                Expect(tag.First(), Is.EqualTo("tag1"));
                Expect(tag.Last(), Is.EqualTo("tag2"));
            }
        }

        public class AddReference : AssertionHelper
        {
            [Test]
            public void Adds_NewReference()
            {
                var repository = new RepositoryGraph();
                var reference = new ReferenceVertex("canonicalName", "targetId");

                repository.AddReference(reference);

                Expect(repository.Vertices.Single(), Is.EqualTo(reference));
            }

            [Test]
            public void Updates_ChangedReference()
            {
                var repository = new RepositoryGraph();
                var reference = new ReferenceVertex("canonicalName", "targetId");
                var updatedReference = new ReferenceVertex("canonicalName", "targetId2");
                repository.AddReference(reference);

                repository.AddReference(updatedReference);

                Expect(repository.Vertices.Single(), Is.EqualTo(updatedReference));
            }

            [Test]
            public void Ignores_UnchangedReference()
            {
                var repository = new RepositoryGraph();
                var reference = new ReferenceVertex("canonicalName", "targetId");
                var updatedReference = new ReferenceVertex("canonicalName", "targetId");
                repository.AddReference(reference);

                repository.AddReference(updatedReference);

                Expect(ReferenceEquals(repository.Vertices.Single(), reference));
            }
        }

        // Objects
        // Tag Annotation -> points at one thing (any type)

        // Refs
        // Tag -> points at one thing (any type)
        // Branch -> points at one Tip commit

        public class RemoveReferencesNotIn : AssertionHelper
        {
            [Test]
            public void RemovesReferenceAndEdge()
            {
                var repository = new RepositoryGraph();
                var commit = new CommitVertex("sha1", string.Empty);
                repository.AddObject(commit);
                var branch = new ReferenceVertex("canonicalName", "sha1");
                repository.AddReference(branch);

                repository.RemoveReferencesNotIn(new string[] {});

                Expect(repository.Edges, Is.Empty);
                Expect(repository.Vertices.Single(), Is.EqualTo(commit));
            }

            [Test]
            public void Leaves_Reference()
            {
                var repository = new RepositoryGraph();
                var commit = new CommitVertex("sha1", string.Empty);
                repository.AddObject(commit);
                var branch = new ReferenceVertex("canonicalName", "sha1");
                repository.AddReference(branch);

                repository.RemoveReferencesNotIn(new[] {"canonicalName"});

                Expect(repository.Vertices.Contains(branch));
            }
        }
    }
}