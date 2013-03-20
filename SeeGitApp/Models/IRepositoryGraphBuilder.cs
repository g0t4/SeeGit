namespace SeeGit
{
    using Models;

    public interface IRepositoryGraphBuilder
    {
        RepositoryGraph Graph(GraphParameters graphParameters);
    }
}