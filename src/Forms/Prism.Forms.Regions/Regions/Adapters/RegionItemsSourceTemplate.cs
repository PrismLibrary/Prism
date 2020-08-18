using Xamarin.Forms;

namespace Prism.Regions.Adapters
{
    internal class RegionItemsSourceTemplate : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            // Implementation Note:
            // The view returned will get the BindingContext set to the item provided here.
            // In our case this means we lose our ViewModel and the View gets a BindingContext
            // of itself. To prevent anything from getting messed up we return the view as
            // the content of a new ContentView.
            var view = (View)item;
            return new DataTemplate(() => new ContentView { Content = view });
        }
    }
}
