namespace SeeGit.Models
{
    using LibGit2Sharp;

    public class ObjectVertex : GitVertex
    {
        public string Sha { get; set; }

        public ObjectVertex(string sha1)
        {
            Sha = sha1;
        }

        public ObjectVertex(GitObject gitObject)
        {
            Sha = gitObject.Sha;
        }

        public override string ToString()
        {
            return Sha.AtMost(8);
        }

        public override string Key
        {
            get { return Sha; }
        }
    }
}