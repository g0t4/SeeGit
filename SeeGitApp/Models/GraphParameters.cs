namespace SeeGit.Models
{
    public class GraphParameters
    {
        public bool IncludeCommitContent { get; set; }
        public bool IncludeUnreachableCommits { get; set; }
        public bool IncludeIndex { get; set; }
        public bool IncludeWorkTree { get; set; }
    }
}