using Xamarin.Forms;

namespace Prism.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="IPageAware"/> using <see cref="Application.Current.MainPage"/> property.
    /// </summary>
    public class ApplicationPageProvider : IPageAware
    {
        #region Implementation of IPageProvider

        public Page Page
        {
            get { return Application.Current.MainPage; }
            set { /* Does nothing */ }
        }

        #endregion
    }
}
