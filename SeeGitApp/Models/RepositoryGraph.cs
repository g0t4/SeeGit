namespace SeeGit.Models
{
    using System.Collections.Generic;
    using BclExtensionMethods;
    using QuickGraph;

    public class RepositoryGraph : BidirectionalGraph<GitVertex, GitEdge>
    {
        public RepositoryGraph()
        {
            LayoutAlgorithmType = "EfficientSugiyama";
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
        }

        public bool HasObject(ObjectVertex @object)
        {
            return _Objects.ContainsKey(@object.Sha);
        }

        private IDictionary<string, GitEdge> _ObjectEdges = new Dictionary<string, GitEdge>();

        public void AddObjectEdge(string sourceKey, string targetKey)
        {
            var edgeKey = GitEdge.GetEdgeKey(sourceKey, targetKey);
            if (_ObjectEdges.ContainsKey(edgeKey))
            {
                return;
            }
            var source = _Objects.GetValueOrDefault(sourceKey);
            var target = _Objects.GetValueOrDefault(targetKey);
            var edge = new GitEdge(source, target);
            _ObjectEdges.Add(edge.Key, edge);
            AddEdge(edge);
        }
    }
}