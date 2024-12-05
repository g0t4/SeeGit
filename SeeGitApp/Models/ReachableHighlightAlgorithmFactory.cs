using System.Collections.Generic;
using GraphShape.Algorithms.Highlight;
using QuikGraph;

namespace SeeGit.Models
{
    public class ReachableHighlightAlgorithmFactory<TVertex, TEdge, TGraph> :
        IHighlightAlgorithmFactory<TVertex, TEdge, TGraph> where TVertex : class
                                                           where TEdge : IEdge<TVertex>
                                                           where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        private const string HighlightModeName = "Reachable";

        public IHighlightAlgorithm<TVertex, TEdge> CreateAlgorithm(string highlightMode,
                                                                       IHighlightContext<TVertex, TEdge, TGraph> context,
                                                                       IHighlightController<TVertex, TEdge, TGraph> controller,
                                                                       IHighlightParameters parameters)
        {
            if (string.IsNullOrEmpty(highlightMode) || highlightMode == HighlightModeName)
            {
                return new ReachableHighlightAlgorithm<TVertex, TEdge, TGraph>(controller, parameters);
            }
            return null;
        }

        public IHighlightParameters CreateParameters(string highlightMode, IHighlightParameters oldParameters)
        {
            // TODO what are all the unhandled exceptions for collections modified in algorithm?
            // TODO does this work?!
            // FYI notes about layout issues => seems like its may a vertical offset problem? zoom out and refresh and they all line up in a row but no vertical differences
            // 
            //    this is one thing I don't know if it matters: was some HighlightParametersBase() previously in QuickShape
            return new HighlightParameters();
        }

        public string GetHighlightMode(IHighlightAlgorithm<TVertex, TEdge> algorithm)
        {
            if (algorithm is ReachableHighlightAlgorithm<TVertex, TEdge, TGraph>)
            {
                return HighlightModeName;
            }
            return null;
        }

        public bool IsValidMode(string mode)
        {
            return string.IsNullOrEmpty(mode) || mode == HighlightModeName;
        }

        // Properties
        public IEnumerable<string> HighlightModes
        {
            get { yield return HighlightModeName; }
        }
    }
}