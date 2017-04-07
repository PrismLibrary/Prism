using Windows.UI.Xaml.Navigation;

namespace Prism.Windows.Tests.Mocks
{
    public partial class MockPage
    {
        public object PageParameter { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e != null)
            {
                this.PageParameter = e.Parameter;
            }
        }
    }
}
