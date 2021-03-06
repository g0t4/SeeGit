﻿namespace SeeGit.Models
{
    using System.Linq;
    using BclExtensionMethods;
    using QuickGraph;
    using Vertices;

	public class RepositoryGraph : BidirectionalGraph<GitVertex, GitEdge>
    {
        public RepositoryGraph()
        {
            LayoutAlgorithmType = "Tree";
        }

        public string LayoutAlgorithmType { get; set; }

        private void SetLayoutType()
        {
            if (Vertices.Count() > 1)
            {
                LayoutAlgorithmType = "EfficientSugiyama";
            }
        }

        public void Set(GraphContents contents)
        {
            contents.Vertices
                    .Values
                    .ForEach(v => AddVertex(v));
            contents.GetEdges()
                    .ForEach(e => AddEdge(e));
            SetLayoutType();
        }
    }
}