namespace SeeGit.Models.Vertices
{
    using LibGit2Sharp;

    public class WorkTreeEntryVertex : ObjectVertex
    {
        public WorkTreeEntryVertex(string path, FileStatus state) : base(path)
        {
            // TODO can't I get an object id for new files, its just a sha! is there a method to get that, if so use it here to pass to base(sha) instead of base(path)
            Path = path;
            State = state;
        }

        public string Path { get; set; }
        public FileStatus State { get; set; }
        public string StatusColor => "LightBlue"; // TODO colors per status
    }

}