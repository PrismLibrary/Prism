using Xamarin.Forms;

namespace Prism.Regions.Adapters
{
    internal class RegionItemsSourceTemplate : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return new DataTemplate(() => (View)item);
        }
    }
}
