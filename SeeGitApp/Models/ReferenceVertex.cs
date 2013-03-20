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
    }
}