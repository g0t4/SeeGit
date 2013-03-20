namespace SeeGit.Models
{
    using LibGit2Sharp;

    public class BranchVertex : ReferenceVertex
    {
        public BranchVertex(string canonicalName, string name) : base(canonicalName, name)
        {
        }

        public BranchVertex(Branch branch) : base(branch.CanonicalName, branch.Name)
        {
        }
    }
}