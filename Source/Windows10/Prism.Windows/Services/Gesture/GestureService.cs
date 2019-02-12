using Prism.Logging;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using System.ComponentModel;

namespace Prism.Services
{
    public class GestureService : IGestureService, IDestructibleGestureService
    {
        public GestureService(CoreWindow window, ILoggerFacade logger)
        {
            window.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
            window.PointerPressed += CoreWindow_PointerPressed;
            SystemNavigationManager.GetForCurrentView().BackRequested += GestureService_BackRequested;
            _logger = logger;
        }

        public event EventHandler MenuRequested;
        public event EventHandler BackRequested;
        public event EventHandler SearchRequested;
        public event EventHandler RefreshRequested;
        public event EventHandler ForwardRequested;
        public event TypedEventHandler<object, KeyDownEventArgs> KeyDown;

        #region Barrier

        List<GestureBarrier> _barriers = new List<GestureBarrier>();
        private readonly ILoggerFacade _logger;

        public GestureBarrier CreateBarrier(Gesture gesture)
        {
            GestureBarrier barrier = null;
            return barrier = new GestureBarrier
            {
                Gesture = gesture,
                Complete = () => _barriers.Remove(barrier),
            };
        }
        bool IfCanRaiseEvent(Gesture evt, Action action)
        {
            if (_barriers.Any(x => x.Gesture.Equals(evt)))
            {
                return false;
            }
            action();
            return true;
        }

        #endregion  

        public bool RaiseRefreshRequested() => IfCanRaiseEvent(Gesture.Refresh, () => RefreshRequested?.Invoke(this, EventArgs.Empty));
        public bool RaiseBackRequested() => IfCanRaiseEvent(Gesture.Back, () => BackRequested?.Invoke(this, EventArgs.Empty));
        public bool RaiseForwardRequested() => IfCanRaiseEvent(Gesture.Forward, () => ForwardRequested?.Invoke(this, EventArgs.Empty));
        public bool RaiseSearchRequested() => IfCanRaiseEvent(Gesture.Search, () => SearchRequested?.Invoke(this, EventArgs.Empty));
        public bool RaiseMenuRequested() => IfCanRaiseEvent(Gesture.Menu, () => MenuRequested?.Invoke(null, EventArgs.Empty));

        public void Destroy(CoreWindow window)
        {
            window.Dispatcher.AcceleratorKeyActivated -= Dispatcher_AcceleratorKeyActivated;
            window.PointerPressed -= CoreWindow_PointerPressed;
            SystemNavigationManager.GetForCurrentView().BackRequested -= GestureService_BackRequested;
        }

        private void GestureService_BackRequested(object sender, BackRequestedEventArgs e)
        {
            RaiseBackRequested();
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs e)
        {
            var properties = e.CurrentPoint.Properties;
            // Ignore button chords with the left, right, and middle buttons
            if (properties.IsLeftButtonPressed
                || properties.IsRightButtonPressed
                || properties.IsMiddleButtonPressed)
            {
                return;
            }
            TestForNavigateRequested(e, properties);
        }

        private void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            if (!e.EventType.ToString().Contains("Down") || e.Handled)
            {
                return;
            }
            var args = new KeyDownEventArgs(e.VirtualKey) { EventArgs = e };
            TestForSearchRequested(args);
            TestForMenuRequested(args);
            TestForNavigateRequested(args);
            KeyDown?.Invoke(null, args);
        }

        private void TestForNavigateRequested(KeyDownEventArgs e)
        {
            if ((e.VirtualKey == VirtualKey.GoBack)
                || (e.VirtualKey == VirtualKey.NavigationLeft)
                || (e.VirtualKey == VirtualKey.GamepadMenu)
                || (e.VirtualKey == VirtualKey.GamepadLeftShoulder)
                || (e.OnlyAlt && e.VirtualKey == VirtualKey.Back)
                || (e.OnlyAlt && e.VirtualKey == VirtualKey.Left))
            {
                _logger.Log($"{nameof(GestureService)}.{nameof(BackRequested)}", Category.Info, Priority.None);
                RaiseBackRequested();
            }
            else if ((e.VirtualKey == VirtualKey.GoForward)
                || (e.VirtualKey == VirtualKey.NavigationRight)
                || (e.VirtualKey == VirtualKey.GamepadRightShoulder)
                || (e.OnlyAlt && e.VirtualKey == VirtualKey.Right))
            {
                _logger.Log($"{nameof(GestureService)}.{nameof(ForwardRequested)}", Category.Info, Priority.None);
                RaiseForwardRequested();
            }
            else if ((e.VirtualKey == VirtualKey.Refresh)
                || (e.VirtualKey == VirtualKey.F5))
            {
                _logger.Log($"{nameof(GestureService)}.{nameof(RefreshRequested)}", Category.Info, Priority.None);
                RaiseRefreshRequested();
            }
            // this is still a preliminary value?
            else if ((e.VirtualKey == VirtualKey.M) && e.OnlyAlt)
            {
                _logger.Log($"{nameof(GestureService)}.{nameof(MenuRequested)}", Category.Info, Priority.None);
                RaiseMenuRequested();
            }
        }

        private void TestForNavigateRequested(PointerEventArgs e, PointerPointProperties properties)
        {
            // If back or foward are pressed (but not both) 
            var backPressed = properties.IsXButton1Pressed;
            var forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                e.Handled = true;
                if (backPressed)
                {
                    _logger.Log($"{nameof(GestureService)}.{nameof(BackRequested)}", Category.Info, Priority.None);
                    RaiseBackRequested();
                }
                else if (forwardPressed)
                {
                    _logger.Log($"{nameof(GestureService)}.{nameof(ForwardRequested)}", Category.Info, Priority.None);
                    RaiseForwardRequested();
                }
            }
        }

        private void TestForMenuRequested(KeyDownEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.GamepadMenu)
            {
                _logger.Log($"{nameof(GestureService)}.{nameof(MenuRequested)}", Category.Info, Priority.None);
                RaiseMenuRequested();
            }
        }

        private void TestForSearchRequested(KeyDownEventArgs args)
        {
            if (args.OnlyControl && args.Character.ToString().ToLower().Equals("e"))
            {
                _logger.Log($"{nameof(GestureService)}.{nameof(SearchRequested)}", Category.Info, Priority.None);
                RaiseSearchRequested();
            }
        }
    }
}
