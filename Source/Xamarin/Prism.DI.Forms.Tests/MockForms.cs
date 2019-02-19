using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Mocks
{
    public static class MockForms
    {
        /// <summary>
        /// Callback for asserting against Device.OpenUri
        /// NOTE: MockForms.Init() clears this value
        /// </summary>
        public static Action<Uri> OpenUriAction { get; set; }

        public static void Init(string runtimePlatform = "Test")
        {
            Device.Info = new MockDeviceInfo();
            Device.PlatformServices = new PlatformServices(runtimePlatform);
            DependencyService.Register<MockResourcesProvider>();
            DependencyService.Register<MockDeserializer>();
            OpenUriAction = null;
        }

        public static void UpdateRuntimePlatform(string runtimePlatform)
        {
            Device.PlatformServices = new PlatformServices(runtimePlatform);
        }

        private class PlatformServices : IPlatformServices
        {
            public PlatformServices(string runtimePlatform)
            {
                RuntimePlatform = runtimePlatform;
            }

            public bool IsInvokeRequired
            {
                get { return false; }
            }

            public string RuntimePlatform
            {
                get;
                private set;
            }

            public void BeginInvokeOnMainThread(Action action)
            {
                action();
            }

            public Ticker CreateTicker()
            {
                return new MockTicker();
            }

            public Assembly[] GetAssemblies()
            {
                return new Assembly[0];
            }

            public string GetMD5Hash(string input)
            {
                throw new NotImplementedException();
            }

            public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
            {
                return 14;
            }

            public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
            {
                throw new NotImplementedException();
            }

            public Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public IIsolatedStorageFile GetUserStoreForApplication()
            {
                throw new NotImplementedException();
            }

            public void OpenUriAction(Uri uri)
            {
                MockForms.OpenUriAction?.Invoke(uri);
            }

            public void QuitApplication()
            {
                throw new NotImplementedException();
            }

            public void StartTimer(TimeSpan interval, Func<bool> callback) { }
        }

        internal class MockTicker : Ticker
        {
            bool _enabled;

            protected override void EnableTimer()
            {
                _enabled = true;

                while (_enabled)
                {
                    SendSignals(16);
                }
            }

            protected override void DisableTimer()
            {
                _enabled = false;
            }
        }

        internal class MockResourcesProvider : ISystemResourcesProvider
        {
            public IResourceDictionary GetSystemResources()
            {
                var dictionary = new ResourceDictionary();
                Style style;
                style = new Style(typeof(Label));
                dictionary[Device.Styles.BodyStyleKey] = style;

                style = new Style(typeof(Label));
                style.Setters.Add(Label.FontSizeProperty, 50);
                dictionary[Device.Styles.TitleStyleKey] = style;

                style = new Style(typeof(Label));
                style.Setters.Add(Label.FontSizeProperty, 40);
                dictionary[Device.Styles.SubtitleStyleKey] = style;

                style = new Style(typeof(Label));
                style.Setters.Add(Label.FontSizeProperty, 30);
                dictionary[Device.Styles.CaptionStyleKey] = style;

                style = new Style(typeof(Label));
                style.Setters.Add(Label.FontSizeProperty, 20);
                dictionary[Device.Styles.ListItemTextStyleKey] = style;

                style = new Style(typeof(Label));
                style.Setters.Add(Label.FontSizeProperty, 10);
                dictionary[Device.Styles.ListItemDetailTextStyleKey] = style;

                return dictionary;
            }
        }

        internal class MockDeserializer : IDeserializer
        {
            public Task<IDictionary<string, object>> DeserializePropertiesAsync()
            {
                return Task.FromResult<IDictionary<string, object>>(new Dictionary<string, object>());
            }

            public Task SerializePropertiesAsync(IDictionary<string, object> properties)
            {
                return Task.FromResult(false);
            }
        }

        internal class MockDeviceInfo : DeviceInfo
        {
            public override Size PixelScreenSize => throw new NotImplementedException();

            public override Size ScaledScreenSize => throw new NotImplementedException();

            public override double ScalingFactor => throw new NotImplementedException();
        }
    }
}
