namespace SeeGit.Models.Vertices
{
    public class ReferenceVertex : GitVertex
    {
        public ReferenceVertex(string canonicalName, string targetId)
        {
            CanonicalName = canonicalName;
            Name = GetShortName(canonicalName);
            TargetId = targetId;
        }

        private string GetShortName(string canonicalName)
        {
            if (canonicalName.StartsWith("refs/heads/"))
            {
                return canonicalName.Replace("refs/heads/", string.Empty);
            }
            if (canonicalName.StartsWith("refs/tags/"))
            {
                return canonicalName.Replace("refs/tags/", string.Empty);
            }
            if (canonicalName.StartsWith("remotes/"))
            {
                return canonicalName.Replace("remotes/", string.Empty);
            }
            return canonicalName;
        }

        public string CanonicalName { get; set; }
        public string Name { get; set; }
        public string TargetId { get; set; }

        public override string Key
        {
            get { return CanonicalName; }
        }

        protected bool Equals(ReferenceVertex other)
        {
            return string.Equals(CanonicalName, other.CanonicalName)
                   && string.Equals(TargetId, other.TargetId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ReferenceVertex) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (CanonicalName != null ? CanonicalName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (TargetId != null ? TargetId.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return Key;
        }
    }
}