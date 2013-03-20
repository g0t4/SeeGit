namespace SeeGit.Models
{
    using LibGit2Sharp;

    public class BlobVertex : ObjectVertex
    {
        public BlobVertex(Blob blob) : base(blob)
        {
        }
    }
}