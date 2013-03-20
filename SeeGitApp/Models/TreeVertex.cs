namespace SeeGit.Models
{
    using LibGit2Sharp;

    public class TreeVertex : ObjectVertex
    {
        public TreeVertex(Tree tree) : base(tree)
        {
        }
    }
}