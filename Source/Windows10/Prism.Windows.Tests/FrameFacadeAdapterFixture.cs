using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Prism.Events;
using Prism.Windows.Navigation;
using Prism.Windows.Tests.Mocks;
using Prism.Windows.Tests.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Prism.Windows.Tests
{
    [TestClass]
    public class FrameFacadeAdapterFixture
    {
        [TestMethod]
        public async Task Navigate_Raises_NavigationStateChangedEvent()
        {
            var tcs = new TaskCompletionSource<bool>();
            var eventAggregator = new EventAggregator();

            eventAggregator.GetEvent<NavigationStateChangedEvent>().Subscribe((args) => tcs.SetResult(args.StateChange == StateChangeType.Forward));

            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var frame = new FrameFacadeAdapter(new Frame(), eventAggregator);
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);

                navigationService.Navigate("Mock", 1);
            });

            await Task.WhenAny(tcs.Task, Task.Delay(200));

            if (tcs.Task.IsCompleted)
            {
                Assert.IsTrue(tcs.Task.Result);
            }
            else
            {
                Assert.Fail("NavigationStateChangedEvent event wasn't raised within 200 ms.");
            }
        }

        [TestMethod]
        public async Task GoBack_Raises_NavigationStateChangedEvent()
        {
            var tcs = new TaskCompletionSource<bool>();
            var eventAggregator = new EventAggregator();

            eventAggregator.GetEvent<NavigationStateChangedEvent>().Subscribe((args) =>
            {
                if (args.StateChange == StateChangeType.Back)
                {
                    tcs.SetResult(true);
                }
            });

            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var frame = new FrameFacadeAdapter(new Frame(), eventAggregator);
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);

                navigationService.Navigate("Mock", 1);
                navigationService.Navigate("Mock", 2);

                navigationService.GoBack();
            });

            await Task.WhenAny(tcs.Task, Task.Delay(200));

            if (tcs.Task.IsCompleted)
            {
                Assert.IsTrue(tcs.Task.Result);
            }
            else
            {
                Assert.Fail("NavigationStateChangedEvent event wasn't raised within 200 ms.");
            }
        }

        [TestMethod]
        public async Task GoForward_Raises_NavigationStateChangedEvent()
        {
            int hitCount = 0;
            var tcs = new TaskCompletionSource<bool>();
            var eventAggregator = new EventAggregator();

            eventAggregator.GetEvent<NavigationStateChangedEvent>().Subscribe((args) =>
            {
                if (hitCount++ > 2 && args.StateChange == StateChangeType.Forward)
                {
                    tcs.SetResult(true);
                }
            });

            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var frame = new FrameFacadeAdapter(new Frame(), eventAggregator);
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);

                navigationService.Navigate("Mock", 1);
                navigationService.Navigate("Mock", 2);
                navigationService.GoBack();

                navigationService.GoForward();
            });

            await Task.WhenAny(tcs.Task, Task.Delay(200));

            if (tcs.Task.IsCompleted)
            {
                Assert.IsTrue(tcs.Task.Result);
            }
            else
            {
                Assert.Fail("NavigationStateChangedEvent event wasn't raised within 200 ms.");
            }
        }

        [TestMethod]
        public async Task Set_State_Raises_NavigationStateChangedEvent()
        {
            var tcs = new TaskCompletionSource<bool>();
            var eventAggregator = new EventAggregator();

            eventAggregator.GetEvent<NavigationStateChangedEvent>().Subscribe((args) => tcs.SetResult(true));

            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var frame = new FrameFacadeAdapter(new Frame(), eventAggregator);

                frame.SetNavigationState("1,0");
            });

            await Task.WhenAny(tcs.Task, Task.Delay(200));

            if (tcs.Task.IsCompleted)
            {
                Assert.IsTrue(tcs.Task.Result);
            }
            else
            {
                Assert.Fail("NavigationStateChangedEvent event wasn't raised within 200 ms.");
            }
        }
    }
}
