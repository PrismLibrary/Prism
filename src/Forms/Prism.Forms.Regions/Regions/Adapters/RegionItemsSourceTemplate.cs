using Xamarin.Forms;

namespace Prism.Regions.Adapters
{
    // Implementation Note:
    // In discussing with PureWeen, it is best to provide a ContentView and provide a
    // simple Binding of the BindingContext to the Content property. The Xamarin.Forms
    // Layout engine may reuse the template provided and change out change out the Binding
    // Context for memory optimization.
    internal class RegionItemsSourceTemplate : DataTemplate
    {
        public RegionItemsSourceTemplate()
            : base(ViewTemplate)
        {
        }

        private static View ViewTemplate()
        {
            var view = new ContentView();
            view.SetBinding(ContentView.ContentProperty, new Binding("."));
            return view;
        }
    }
}
