using Xamarin.Forms;

namespace Prism.Common
{
    /// <summary>
    /// Provides Application components.
    /// </summary>
    public class ApplicationProvider : IApplicationProvider
    {
        /// <inheritdoc/>
        public Page MainPage
        {
            get { return Application.Current.MainPage; }
            set { Application.Current.MainPage = value; }
        }
    }
}
