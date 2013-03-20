namespace SeeGit.Models.Vertices
{
	using LibGit2Sharp;

	public class ObjectVertex : GitVertex
    {
        public string Sha { get; set; }

        public string ShortSha
        {
            get { return Sha.AtMost(6); }
        }

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
            return ShortSha;
        }

        protected bool Equals(ObjectVertex other)
        {
            return string.Equals(Sha, other.Sha);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ObjectVertex) obj);
        }

        public override int GetHashCode()
        {
            return (Sha != null ? Sha.GetHashCode() : 0);
        }

        public override string Key
        {
            get { return Sha; }
        }
    }
}