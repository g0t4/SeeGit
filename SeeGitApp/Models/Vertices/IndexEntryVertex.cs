namespace SeeGit.Models.Vertices
{
    using LibGit2Sharp;

    public class IndexEntryVertex : ObjectVertex
    {
        public IndexEntryVertex(IndexEntry entry) : base(entry.Id.Sha)
        {
            State = entry.State;
            Path = entry.Path;
        }

        public string Path { get; set; }
        public FileStatus State { get; set; }
    }
}