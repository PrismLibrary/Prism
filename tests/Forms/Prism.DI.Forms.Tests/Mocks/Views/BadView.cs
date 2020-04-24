using Xamarin.Forms;

namespace Prism.DI.Forms.Tests.Mocks.Views
{
    public class BadView : Page
    {
        public BadView()
        {
            throw new Xamarin.Forms.Xaml.XamlParseException("You write bad XAML");
        }
    }
}