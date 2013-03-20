namespace SeeGit.Models
{
    using QuickGraph;

    public class RepositoryGraph : BidirectionalGraph<GitVertex, GitEdge>
    {
        public string LayoutAlgorithmType { get; set; }
    }
}