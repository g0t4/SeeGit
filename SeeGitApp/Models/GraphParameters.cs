namespace SeeGit.Models
{
    public class GraphParameters
    {
        public bool IncludeCommitContent { get; set; }
        public bool IncludeUnreachableCommits { get; set; }
        public bool IncludeStaged { get; set; }
        public bool IncludeWorkTrees { get; set; }
        // TODO stashes?
        public bool IncludeStashes { get; set; }

        // TODO reflog mode?!

        // TODO others?
    }
}