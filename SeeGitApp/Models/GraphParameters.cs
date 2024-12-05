namespace SeeGit.Models
{
    public class GraphParameters
    {
        public bool IncludeCommitContent { get; set; }
        public bool IncludeUnreachableCommits { get; set; }
        // TODO rename to Staged in all places its called index still (makes more sense IMO)
        public bool IncludeStaged { get; set; }
        // TODO impl work tree like index (staged)
        public bool IncludeWorkTree { get; set; }
    }
}