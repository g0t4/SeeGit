using LibGit2Sharp;
using SeeGit.Models.Vertices;

namespace SeeGit.Models
{
    public class WorkTreeVertex : GitVertex
    {

        public WorkTreeVertex()
        {
        }

        public override string Key => "work-tree";
    }
}