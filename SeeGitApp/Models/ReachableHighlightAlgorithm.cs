using GraphShape.Algorithms.Highlight;
using QuikGraph;

namespace SeeGit.Models
{
    public class ReachableHighlightAlgorithm<TVertex, TEdge, TGraph> :
        HighlightAlgorithmBase<TVertex, TEdge, TGraph, IHighlightParameters> where TVertex : class
                                                                             where TEdge : IEdge<TVertex>
                                                                             where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        // Methods
        public ReachableHighlightAlgorithm(IHighlightController<TVertex, TEdge, TGraph> controller,
                                           IHighlightParameters parameters) : base(controller, parameters)
        {
        }

        private void ClearAllHighlights()
        {
            // FYI reachable highlights are not quite working yet though not that big of a deal IMO.. just the blue line when hovering over the graph, so lets focus on why layout isn't working quite right first
            //   TODO blue reachables are pointed at wrong shapes and not always updating as they would in v1.0.1/v1.1
            //   ALSO, not removing blue highlight when hover off of vertex... unlike original v1.0.1 which snapped off right away
            //   IMPROVEMENT? take off black highlight of arrow if hover over arrow (edge)

            // OMG do I need to lock before clearing and removing items?
            ClearSemiHighlights();
            // TODO what are all the warnings here about modified collections and cannot finish enumeration, add those back (user unhandled), right now I ignored them just to test app out
            foreach (TVertex local in Controller.HighlightedVertices)
            {
                Controller.RemoveHighlightFromVertex(local);
            }
            foreach (TEdge local2 in Controller.HighlightedEdges)
            {
                Controller.RemoveHighlightFromEdge(local2);
            }
        }

        private void ClearSemiHighlights()
        {
            // TODO what are all the warnings here about modified collections and cannot finish enumeration, add those back (user unhandled), right now I ignored them just to test app out
            //   FYI definitely happened often on SemiHighlightedVertices:
            // IIRC it was InvalidOperationException
            foreach (var vertex in Controller.SemiHighlightedVertices) // HOW DO I add back the user unhandled exception breaking here (not breakpoint)
            {
                Controller.RemoveSemiHighlightFromVertex(vertex);
            }
            foreach (var edge in Controller.SemiHighlightedEdges)
            {
                Controller.RemoveSemiHighlightFromEdge(edge);
            }
        }

        public override bool OnEdgeHighlighting(TEdge edge)
        {
            this.ClearAllHighlights();
            if (!(!object.Equals(edge, default(TEdge)) && base.Controller.Graph.ContainsEdge(edge)))
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

            if (vertex == null || !Controller.Graph.ContainsVertex(vertex)) return false;

            Controller.HighlightVertex(vertex, "Source");
            return HighlightChildren(vertex);
        }

        private bool HighlightChildren(TVertex vertex)
        {
            if (vertex == null) return false;

            foreach (var outEdge in Controller.Graph.OutEdges(vertex))
            {
                Controller.SemiHighlightEdge(outEdge, "OutEdge");
                if (Controller.IsHighlightedVertex(outEdge.Target)) continue;
                Controller.SemiHighlightVertex(outEdge.Target, "Target");
                HighlightChildren(outEdge.Target);
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