using Xamarin.Forms;

namespace Prism.Common
{
    public class ApplicationProvider : IApplicationProvider
    {
        public Page MainPage
        {
            get { return Application.Current.MainPage; }
            set { Application.Current.MainPage = value; }
        }
    }
}
