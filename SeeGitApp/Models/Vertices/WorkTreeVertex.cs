using LibGit2Sharp;

namespace SeeGit.Models.Vertices
{
    public class WorkTreeVertex : GitVertex
    {

        public WorkTreeVertex()
        {
        }

        public override string Key => "work-tree";
    }
}