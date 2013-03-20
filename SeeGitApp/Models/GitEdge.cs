namespace SeeGit.Models
{
    using QuickGraph;

    public class GitEdge : Edge<GitVertex>
    {
        public GitEdge(GitVertex source, GitVertex target)
            : base(source, target)
        {
            SourceKey = source.Key;
            TargetKey = target.Key;
        }

        public string TargetKey { get; set; }
        public string SourceKey { get; set; }

        public string Key
        {
            get { return GetEdgeKey(SourceKey, TargetKey); }
        }

        public static string GetEdgeKey(string sourceKey, string targetKey)
        {
            return targetKey + ".." + sourceKey;
        }

        public override string ToString()
        {
            return Key;
        }

        protected bool Equals(GitEdge other)
        {
            return string.Equals(TargetKey, other.TargetKey) && string.Equals(SourceKey, other.SourceKey);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GitEdge) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TargetKey != null ? TargetKey.GetHashCode() : 0)*397) ^ (SourceKey != null ? SourceKey.GetHashCode() : 0);
            }
        }
    }
}