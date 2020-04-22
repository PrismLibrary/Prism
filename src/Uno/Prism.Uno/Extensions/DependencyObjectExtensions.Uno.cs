using System;
using System.Collections.Generic;
using System.Text;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

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
#else
            => instance.Dispatcher.HasThreadAccess;
#endif
    }
}
