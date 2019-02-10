using System.Collections.Generic;
using System.ComponentModel;
using Prism.Ioc;
using Prism.Services;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Prism.Navigation
{
    public static class NavigationServiceRegistry
    {
        private static Dictionary<Frame, INavigationService> _instances { get; } = new Dictionary<Frame, INavigationService>();

        /// <summary>
        /// Creates a navigation service
        /// </summary>
        /// <param name="gestures">Optional default getures tied to this Frame</param>
        /// <returns>INavigationService</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INavigationService CreateNavigationService(this IContainerProvider container, params Gesture[] gestures)
        {
            return CreateNavigationService(container, null, Window.Current.CoreWindow, gestures);
        }

        /// <summary>
        /// Creates a navigation service
        /// </summary>
        /// <param name="frame">Required XAML Frame</param>
        /// <param name="gestures">Optional default getures tied to this Frame</param>
        /// <returns>INavigationService</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INavigationService CreateNavigationService(this IContainerProvider container, Frame frame, params Gesture[] gestures)
        {
            return CreateNavigationService(container, frame, Window.Current.CoreWindow, gestures);
        }

        /// <summary>
        /// Creates a navigation service
        /// </summary>
        /// <param name="gestures">Optional default getures tied to this Frame</param>
        /// <returns>INavigationService</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INavigationService CreateNavigationService(this IContainerProvider container, CoreWindow window, params Gesture[] gestures)
        {
            return CreateNavigationService(container, null, window, gestures);
        }

        /// <summary>
        /// Creates a navigation service
        /// </summary>
        /// <param name="frame">Required XAML Frame</param>
        /// <param name="gestures">Optional default getures tied to this Frame</param>
        /// <returns>INavigationService</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INavigationService CreateNavigationService(this IContainerProvider container, Frame frame, CoreWindow window, params Gesture[] gestures)
        {
            if (frame is null)
                frame = new Frame();

            if (_instances.ContainsKey(frame))
                return _instances[frame];

            var gesture_service = GestureServiceRegistry.GetForCurrentView(window);
            var navigation_service = container.Resolve<IPlatformNavigationService>(PrismApplicationBase.NavigationServiceParameterName, (typeof(Frame), frame));
            foreach (var gesture in gestures)
            {
                switch (gesture)
                {
                    case Gesture.Back:
                        gesture_service.BackRequested += async (s, e) => await navigation_service.GoBackAsync();
                        break;
                    case Gesture.Forward:
                        gesture_service.ForwardRequested += async (s, e) => await navigation_service.GoForwardAsync(default(INavigationParameters));
                        break;
                    case Gesture.Refresh:
                        gesture_service.RefreshRequested += async (s, e) => await navigation_service.RefreshAsync();
                        break;
                }
            }

            _instances.Add(frame, navigation_service);
            return navigation_service;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ClearCache() => _instances.Clear();
    }
}
