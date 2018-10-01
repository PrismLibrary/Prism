using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Prism.Services
{
    public static class GestureServiceLocator
    {
        private static readonly Dictionary<CoreWindow, IGestureService> _cache
            = new Dictionary<CoreWindow, IGestureService>();

        private static Func<CoreWindow, IGestureService> _factory;

        public static IGestureService GetGestureService(CoreWindow coreWindow)
        {
            if (!_cache.ContainsKey(coreWindow))
            {
                var gestureService = _factory(coreWindow);
                void Window_Closed(CoreWindow sender, CoreWindowEventArgs args)
                {
                    sender.Closed -= Window_Closed;
                    if (_cache.ContainsKey(sender))
                    {
                        if (_cache[sender] is IGestureServiceInternal disposable)
                            disposable.Dispose(sender);

                        _cache.Remove(sender);
                    }
                }
                coreWindow.Closed += Window_Closed;
            }

            return _cache[coreWindow];
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetDefaultFactory(Func<CoreWindow, IGestureService> factory) =>
            _factory = factory;
    }
}
