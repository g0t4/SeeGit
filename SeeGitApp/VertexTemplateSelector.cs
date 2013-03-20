namespace SeeGit
{
    using System.Windows;
    using System.Windows.Controls;

    public class VertexTemplateSelector : DataTemplateSelector
    {
        public VertexTemplateSelector()
        {
            Resources = new ResourceDictionary();
        }

        public ResourceDictionary Resources { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return Resources[item.GetType().Name + "Template"] as DataTemplate;
        }
    }
}