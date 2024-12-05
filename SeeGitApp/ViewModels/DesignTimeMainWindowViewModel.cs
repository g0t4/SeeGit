namespace SeeGit
{
    using System.Windows.Threading;
    using Models;
    using Models.Vertices;

	public class DesignTimeMainWindowViewModel : MainWindowViewModel
    {
        public DesignTimeMainWindowViewModel() : base(Dispatcher.CurrentDispatcher, _ => new DesignTimeGraphBuilder())
        {
            Graph = new DesignTimeGraphBuilder().Graph(new GraphParameters());
        }
    }

    public class DesignTimeGraphBuilder : IRepositoryGraphBuilder
    {
        public RepositoryGraph Graph(GraphParameters graphParameters)
        {
            var graph = new RepositoryGraph();
            var commits = new GitVertex[]
                {
                    new CommitVertex("c34173273", "Wrote some code")
                        {Message = "This is a long form description of the commit"},
                    new CommitVertex("b1ae7a123", "Initial commit"),
                    new CommitVertex("aa3823ca1", "Added readme"),
                    new CommitVertex("9e21435fa", "Branching")
                        {Message = "This is a long form description of the commit"},
                    new ReferenceVertex("refs/head/master", "c34173273"),
                    new ReferenceVertex("remotes/origin/master", "c34173273"),
                };

            graph.AddVertexRange(commits);
            graph.AddEdge(new GitEdge(commits[1], commits[2], null));
            graph.AddEdge(new GitEdge(commits[0], commits[1], null));
            graph.AddEdge(new GitEdge(commits[3], commits[2], null));

            graph.LayoutAlgorithmType = StandardLayoutAlgorithms.Sugiyama;

            return graph;
        }
    }
}