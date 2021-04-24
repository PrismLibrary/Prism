using System;
using System.Collections.Generic;
using System.Text;

#if HAS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#elif HAS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Windows.Foundation.Metadata;
#endif

namespace Prism
{
    internal static partial class DependencyObjectExtensions
    {
        /// <summary>
        /// Compatibility method to determine if the current thread can access a <see cref="DependencyObject"/>
        /// </summary>
        /// <param name="instance">The instance to check</param>
        /// <returns><c>true</c> if the current thread has access to the instance, otherwise <c>false</c></returns>
        public static bool CheckAccess(this DependencyObject instance)
#if __WASM__
            // This needs to evolve, once threading is supported
            // See https://github.com/unoplatform/uno/issues/2302
            => System.Threading.Thread.CurrentThread.ManagedThreadId == 1;
#elif HAS_WINUI
#if NETCOREAPP
            => instance.DispatcherQueue.HasThreadAccess;
#else
            {
                // Dispatcher queue HasThreadAccess is not yet implemented in Uno, we can fall back on CoreDispatcher
                // See https://github.com/unoplatform/uno/issues/5818
                if(ApiInformation.IsPropertyPresent("Microsoft.System.DispatcherQueue", nameof(Microsoft.System.DispatcherQueue.HasThreadAccess)))
                {
                    return instance.DispatcherQueue.HasThreadAccess;
                }
                else
                {
                    return instance.Dispatcher.HasThreadAccess;
                }
            }
#endif
#else
            => instance.Dispatcher.HasThreadAccess;
#endif
    }
}
