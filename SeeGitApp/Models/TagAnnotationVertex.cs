namespace SeeGit.Models
{
    using LibGit2Sharp;

    public class TagAnnotationVertex : ObjectVertex
    {
        public TagAnnotationVertex(TagAnnotation tagAnnotation) : base(tagAnnotation)
        {
        }
    }
}