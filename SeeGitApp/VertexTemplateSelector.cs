namespace SeeGit
{
    using System.Windows;
    using System.Windows.Controls;
    using Models;
    using Models.Vertices;

	public class VertexTemplateSelector : DataTemplateSelector
    {
        public VertexTemplateSelector()
        {
            Resources = new ResourceDictionary();
        }

        public ResourceDictionary Resources { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ReferenceVertex)
            {
                var reference = item as ReferenceVertex;
                if (reference.Name == "HEAD")
                {
                    return Resources["HeadVertexTemplate"] as DataTemplate;
                }
                if (reference.CanonicalName.Contains("/tags/"))
                {
                    return Resources["TagVertexTemplate"] as DataTemplate;
                }
            }
            return Resources[item.GetType().Name + "Template"] as DataTemplate;
        }
    }
}