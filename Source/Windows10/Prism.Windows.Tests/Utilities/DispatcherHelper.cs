using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;

namespace Prism.Windows.Tests.Utilities
{
    public static class DispatcherHelper
    {
        /// <summary>
        /// Executes the given action on the UI thread via CoreDispatcher.
        /// </summary>
        /// <param name="action">The action that needs to execute on the UI thread.</param>
        /// <returns></returns>
        public static IAsyncAction ExecuteOnUIThread(DispatchedHandler action)
        {
            return CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, action);
        }
    }
}
