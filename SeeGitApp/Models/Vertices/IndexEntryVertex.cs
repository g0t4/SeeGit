namespace SeeGit.Models.Vertices
{
    using LibGit2Sharp;

    public class IndexEntryVertex : ObjectVertex
    {
        public IndexEntryVertex(IndexEntry entry, FileStatus status) : base(entry.Id.Sha)
        {
            State = status;
            Path = entry.Path;
        }

        public string Path { get; set; }
        public FileStatus State { get; set; }
    }
}