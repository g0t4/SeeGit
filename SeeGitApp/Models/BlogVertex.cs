namespace SeeGit.Models
{
    using LibGit2Sharp;

    public class BlogVertex : ObjectVertex
    {
        public BlogVertex(Blob blob) : base(blob)
        {
        }
    }
}