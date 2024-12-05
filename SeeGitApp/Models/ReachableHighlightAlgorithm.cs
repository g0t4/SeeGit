using GraphShape.Algorithms.Highlight;
using QuikGraph;
using System.Linq;

namespace SeeGit.Models
{
    public class ReachableHighlightAlgorithm<TVertex, TEdge, TGraph> :
            HighlightAlgorithmBase<TVertex, TEdge, TGraph, IHighlightParameters> where TVertex : class
                                                                                 where TEdge : IEdge<TVertex>
                                                                                 where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        public ReachableHighlightAlgorithm(IHighlightController<TVertex, TEdge, TGraph> controller,
                                           IHighlightParameters parameters) : base(controller, parameters)
        {
        }

        private void ClearAllHighlights()
        {
            //   IMPROVEMENT? take off black highlight of arrow if hover over arrow (edge)
            // TODO how can I style the highlighted colors / etc... is blue just a default on the edges? 
            //  TODO style the selected commit vertices, and others?

            // FTR, added ToList() to avoid InvalidOperationException after first remove which breaks removing highlights and thus highlights break, ToList() isn't lazy enumerated, it's a cached list then and then enumeration is over the cached list so when the underlying collection is altered enumeration isn't Fubar'd

            // *** semi
            foreach (var vertex in Controller.SemiHighlightedVertices.ToList())
            {
                Controller.RemoveSemiHighlightFromVertex(vertex);
            }
            foreach (var edge in Controller.SemiHighlightedEdges.ToList())
            {
                Controller.RemoveSemiHighlightFromEdge(edge);
            }

            // *** full on
            foreach (var vertex in Controller.HighlightedVertices.ToList())
            {
                Controller.RemoveHighlightFromVertex(vertex);
            }
            foreach (var edge in Controller.HighlightedEdges.ToList())
            {
                Controller.RemoveHighlightFromEdge(edge);
            }
        }

        public override bool OnEdgeHighlighting(TEdge edge)
        {
            ClearAllHighlights();
            if (Equals(edge, default) || !Controller.Graph.ContainsEdge(edge))
            {
                return false;
            }
            Controller.HighlightEdge(edge, null);
            Controller.SemiHighlightVertex(edge.Source, "Source");
            Controller.SemiHighlightVertex(edge.Target, "Target");
            return true;
        }

        public override bool OnEdgeHighlightRemoving(TEdge edge)
        {
            ClearAllHighlights();
            return true;
        }

        public override bool OnVertexHighlighting(TVertex vertex)
        {
            if (!Controller.Graph.IsDirected) return false;
            ClearAllHighlights();

            if (vertex == null || !Controller.Graph.ContainsVertex(vertex))
                return false;

            Controller.HighlightVertex(vertex, "Source");
            return HighlightChildrenRecursively(vertex);
        }

        private bool HighlightChildrenRecursively(TVertex vertex)
        {
            if (vertex == null) return false;

            foreach (var outEdge in Controller.Graph.OutEdges(vertex))
            {
                Controller.SemiHighlightEdge(outEdge, "OutEdge");
                if (Controller.IsHighlightedVertex(outEdge.Target)) continue;
                Controller.SemiHighlightVertex(outEdge.Target, "Target");
                HighlightChildrenRecursively(outEdge.Target);
            }
            return true;
        }

        public override bool OnVertexHighlightRemoving(TVertex vertex)
        {
            ClearAllHighlights();
            return true;
        }

        public override void ResetHighlight()
        {
            ClearAllHighlights();
        }
    }
}