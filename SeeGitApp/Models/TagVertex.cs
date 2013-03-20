namespace SeeGit.Models
{
    using LibGit2Sharp;

    public class TagVertex : ReferenceVertex
    {
        public TagVertex(Tag tag) : base(tag.CanonicalName, tag.Name)
        {
        }
    }
}