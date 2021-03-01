#if MONOANDROID
using System.Threading;
using Prism.Common;
using Prism.Ioc;
using Xamarin.Forms;

namespace Prism
{
    /// <summary>
    /// Native helper class
    /// </summary>
    public static class PrismPlatform
    {
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        /// <summary>
        /// Called when the Activity has detected the user's press of the back key
        /// </summary>
        public static async void OnBackPressed()
        {
            await semaphore.WaitAsync();
            try
            {
                var container = ContainerLocator.Container;
                var appProvider = container.Resolve<IApplicationProvider>();

                Page topPage = PageUtilities.GetCurrentPage(appProvider.MainPage);
                var navService = Navigation.Xaml.Navigation.GetNavigationService(topPage);
                await navService.GoBackAsync();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
#endif
