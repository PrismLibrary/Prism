using Prism.Common;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks
{
    public class ApplicationProviderMock : IApplicationProvider
    {
        public ApplicationProviderMock()
        {
            MainPage = new ContentPage()
            {
                Title = "MainPage"
            };
        }

        public Page MainPage { get; set; }
    }
}
