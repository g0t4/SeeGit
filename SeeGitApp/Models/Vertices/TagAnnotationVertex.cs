namespace SeeGit.Models.Vertices
{
	using LibGit2Sharp;

	public class TagAnnotationVertex : ObjectVertex
    {
        public TagAnnotationVertex(TagAnnotation tagAnnotation) : base(tagAnnotation)
        {
            Message = tagAnnotation.Message;
        }

        public string Message { get; set; }
    }
}