﻿namespace SeeGit.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Vertices;

	public class GraphContents
    {
        public IDictionary<string, GitVertex> Vertices = new Dictionary<string, GitVertex>();
        public IDictionary<string, Edge> Edges = new Dictionary<string, Edge>();

        public void AddVertex(GitVertex vertex)
        {
            if (Vertices.ContainsKey(vertex.Key))
            {
                return;
            }
            Vertices[vertex.Key] = vertex;
        }

        public void AddEdge(Edge edge)
        {
            if (Edges.ContainsKey(edge.Key))
            {
                return;
            }
            Edges[edge.Key] = edge;
        }

        public class Edge
        {
            public string Source { get; set; }
            public string Target { get; set; }
            public string Tag { get; set; }

            public string Key
            {
                get { return Source + ".." + Target + ".." + Tag; }
            }
        }

        public IEnumerable<GitEdge> GetEdges()
        {
            var edges = new Dictionary<string, GitEdge>();
            Edges
                .Values
                .ToList() // just for ForEach
                .ForEach(e =>
                    {
                        var source = Vertices.ContainsKey(e.Source) ? Vertices[e.Source] : null;
                        var target = Vertices.ContainsKey(e.Target) ? Vertices[e.Target] : null;
                        if (source == null || target == null)
                        {
                            return;
                        }
                        var edge = new GitEdge(source, target, e.Tag);
                        if (edges.ContainsKey(edge.Key))
                        {
                            var existing = edges[edge.Key];
                            if (!existing.Tag.Contains(e.Tag))
                            {
                                existing.Tag.Add(e.Tag);
                            }
                        }
                        else
                        {
                            edges.Add(edge.Key, edge);
                        }
                    });
            return edges.Values;
        }
    }
}