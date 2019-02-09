using Prism.Ioc;
using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using System.ComponentModel;

namespace Prism.Services
{
    public static class GestureServiceRegistry
    {
        private static Dictionary<CoreWindow, IGestureService> _cache { get; }
            = new Dictionary<CoreWindow, IGestureService>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IGestureService GetForCurrentView(CoreWindow window = null)
        {
            if (!_cache.ContainsKey(window ?? Window.Current.CoreWindow))
            {
                throw new Exception("Not setup for current view.");
            }
            return _cache[Window.Current.CoreWindow];
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetupForCurrentView(this IContainerProvider container, CoreWindow window)
        {
            if (_cache.ContainsKey(window))
            {
                throw new Exception("Already setup for current view.");
            }
            var service = container.Resolve<IGestureService>((typeof(CoreWindow), window));
            _cache.Add(window, service);

            // remove when closed

            void Window_Closed(CoreWindow sender, CoreWindowEventArgs args)
            {
                window.Closed -= Window_Closed;
                if (_cache.ContainsKey(window))
                {
                    if(_cache[window] is IDestructibleGestureService disposable)
                    {
                        disposable.Destroy(window);
                    }

                    _cache.Remove(window);
                }
            }
            window.Closed += Window_Closed;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ResetCache() => _cache.Clear();
    }
}
