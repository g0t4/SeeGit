namespace SeeGit.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using BclExtensionMethods;
    using QuickGraph;

    public class RepositoryGraph : BidirectionalGraph<GitVertex, GitEdge>
    {
        public RepositoryGraph()
        {
            LayoutAlgorithmType = "Tree";
        }

        public string LayoutAlgorithmType { get; set; }

        private IDictionary<string, ObjectVertex> _Objects = new Dictionary<string, ObjectVertex>();

        public void AddObject(ObjectVertex @object)
        {
            if (_Objects.ContainsKey(@object.Sha))
            {
                return;
            }
            AddVertex(@object);
            _Objects.Add(@object.Sha, @object);
            SetLayoutType();
        }

        private void SetLayoutType()
        {
            if (Vertices.Count() > 1)
            {
                LayoutAlgorithmType = "EfficientSugiyama";
            }
        }

        public bool HasObject(ObjectVertex @object)
        {
            return _Objects.ContainsKey(@object.Sha);
        }

        private IDictionary<string, GitEdge> _ObjectEdges = new Dictionary<string, GitEdge>();

        public void AddObjectEdge(string sourceKey, string targetKey, string tag)
        {
            var edgeKey = GitEdge.GetEdgeKey(sourceKey, targetKey);
            var edge = _ObjectEdges.GetValueOrDefault(edgeKey);
            if (edge != null)
            {
                if (edge.Tag.Contains(tag))
                {
                    return;
                }
                edge.Tag.Add(tag);
                return;
            }
            var source = _Objects.GetValueOrDefault(sourceKey);
            var target = _Objects.GetValueOrDefault(targetKey);
            edge = new GitEdge(source, target, tag);
            _ObjectEdges.Add(edge.Key, edge);
            AddEdge(edge);
        }

        private IDictionary<string, ReferenceVertex> _References = new Dictionary<string, ReferenceVertex>();
        private IDictionary<string, GitEdge> _ReferenceEdges = new Dictionary<string, GitEdge>();

        public void RemoveReferencesNotIn(IEnumerable<string> canonicalNames)
        {
            _ReferenceEdges
                .ToArray()
                .ForEach(e =>
                    {
                        _ReferenceEdges.Remove(e.Key);
                        RemoveEdge(e.Value);
                    });
            _References
                .Where(e => !canonicalNames.Contains(e.Key))
                .ToArray()
                .ForEach(r =>
                    {
                        _References.Remove(r.Key);
                        RemoveVertex(r.Value);
                    });
        }

        public void AddReference(ReferenceVertex reference)
        {
            if (_References.ContainsKey(reference.CanonicalName))
            {
                var existing = _References[reference.CanonicalName];
                if (existing.TargetId == reference.TargetId)
                {
                    return;
                }
                RemoveVertex(existing);
            }
            _References[reference.CanonicalName] = reference;
            AddVertex(reference);
        }

        public void AddReferenceEdge(ReferenceVertex reference)
        {
            var target = ((GitVertex) _Objects.GetValueOrDefault(reference.TargetId))
                         ?? _References.GetValueOrDefault(reference.TargetId);
            var edge = new GitEdge(reference, target, null);
            _ReferenceEdges[reference.CanonicalName] = edge;
            AddEdge(edge);
        }
    }
}