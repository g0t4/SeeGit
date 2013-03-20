namespace SeeGit.Models
{
    public abstract class ReferenceVertex : GitVertex
    {
        public ReferenceVertex(string canonicalName, string name)
        {
            CanonicalName = canonicalName;
            Name = name;
        }

        public string CanonicalName { get; set; }
        public string Name { get; set; }

        public override string Key
        {
            get { return CanonicalName; }
        }

        protected bool Equals(ReferenceVertex other)
        {
            return string.Equals(CanonicalName, other.CanonicalName);
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
            return (CanonicalName != null ? CanonicalName.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return Key;
        }
    }
}