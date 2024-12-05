namespace SeeGit.Models.Vertices
{
    using LibGit2Sharp;

    public class StagedEntryVertex : ObjectVertex
    {
        public StagedEntryVertex(IndexEntry entry, FileStatus status) : base(
                status == FileStatus.DeletedFromIndex ? "deleted" + entry.Id.Sha : entry.Id.Sha)
        {
            State = status;
            Path = entry.Path;
        }

        public string Path { get; set; }
        public FileStatus State { get; set; }
    }

}