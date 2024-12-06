namespace SeeGit.Models.Vertices
{
    using LibGit2Sharp;
    using System.Windows.Media;

    public class StagedEntryVertex : ObjectVertex
    {
        public StagedEntryVertex(StatusEntry entry, FileStatus status, string id) : base(id)
        {
            State = status;
            Path = entry.FilePath;
        }

        public string Path { get; set; }
        public FileStatus State { get; set; }

        public string AbbreviatedStatus
        {
            get
            {
                if (State == FileStatus.NewInIndex)
                    return "New";
                if (State == FileStatus.RenamedInIndex)
                    return "Renamed";
                if (State == FileStatus.ModifiedInIndex)
                    return "Modified";
                if (State == FileStatus.DeletedFromIndex)
                    return "Deleted";
                // TODO more?
                return State.ToString();
            }
        }

        public string TextColor
        {
            get
            {
                if (State == FileStatus.DeletedFromIndex)
                    return Colors.PaleVioletRed.ToString();
                return Colors.LightBlue.ToString();
            }
        }
    }

}