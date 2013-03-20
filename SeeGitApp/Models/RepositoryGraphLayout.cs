namespace SeeGit.Models
{
    using GraphSharp.Controls;

    public class RepositoryGraphLayout : GraphLayout<GitVertex, GitEdge, RepositoryGraph>
    {
        public RepositoryGraphLayout()
        {
            HighlightAlgorithmFactory = new ReachableHighlightAlgorithmFactory<GitVertex, GitEdge, RepositoryGraph>();
        }
    }
}