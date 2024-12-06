namespace SeeGit.Models
{
    using QuikGraph;
    using Vertices;

    public class RepositoryGraph : BidirectionalGraph<GitVertex, GitEdge>
    {
        public RepositoryGraph()
        {
        }

        public void Set(GraphContents contents)
        {
            contents.Vertices
                    .Values
                    .ForEach(v => AddVertex(v));
            contents.GetEdges()
                    .ForEach(e => AddEdge(e));
        }
    }
}