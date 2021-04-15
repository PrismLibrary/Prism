#if MONOANDROID
using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Prism.Common;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Services.Dialogs;
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
        public static async Task<IBackPressedResult> OnBackPressed(Activity activity = null)
        {
            await semaphore.WaitAsync();
            try
            {
                var container = ContainerLocator.Container;
                var appProvider = container.Resolve<IApplicationProvider>();

                Page topPage = PageUtilities.GetCurrentPage(appProvider.MainPage);
                if (topPage is IDialogContainer dc)
                {
                    var result = await dc.RequestCloseAsync();
                    return result.Exception switch
                    {
                        DialogException de
                            when de.Message == DialogException.CanCloseIsFalse
                          => new BackPressedResult(),
                        _ => new BackPressedResult
                        {
                            Success = result.Exception is null,
                            Exception = result.Exception
                        }
                    };
                }

                var navService = Navigation.Xaml.Navigation.GetNavigationService(topPage);
                var navResult = await navService.GoBackAsync();
                if(navResult.Exception is NavigationException ne
                    && ne.Message == NavigationException.CannotPopApplicationMainPage
                    && activity != null)
                {
                    activity.FinishAffinity();
                    return new BackPressedResult
                    {
                        Success = true
                    };
                }

                return new BackPressedResult
                {
                    Success = navResult.Success,
                    Exception = navResult.Exception
                };
            }
            finally
            {
                semaphore.Release();
            }
        }
    }

    internal class BackPressedResult : IBackPressedResult
    {
        public bool Success { get; set; }

        public Exception Exception { get; set; }
    }

    public interface IBackPressedResult
    {
        bool Success { get; }
        Exception Exception { get; }
    }
}
#endif
