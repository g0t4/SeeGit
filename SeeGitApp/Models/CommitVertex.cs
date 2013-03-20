namespace SeeGit.Models
{
    using LibGit2Sharp;

    public class CommitVertex : ObjectVertex
    {
        public CommitVertex(string sha1, string messageShort) : base(sha1)
        {
            Sha = sha1;
            MessageShort = messageShort;
        }

        public CommitVertex(Commit commit)
            : base(commit)
        {
            MessageShort = commit.MessageShort;
            Message = commit.Message;
        }

        public string MessageShort { get; set; }
        public string Message { get; set; }
    }
}