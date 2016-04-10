using Prism.Logging;
using System;
using Xamarin.Forms;
using Xunit;
using Prism.Navigation;

namespace Prism.Forms.Tests
{
    public class BootstrapperFixture
    {
        [Fact]
        public void LoggerDefaultsToNull()
        {
            var bootstrapper = new DefaultBootstrapper();
            Assert.Null(bootstrapper.BaseLogger);
        }

        [Fact]
        public void CreateLoggerInitializesLogger()
        {
            var bootstrapper = new DefaultBootstrapper();
            bootstrapper.CallCreateLogger();

            Assert.NotNull(bootstrapper.BaseLogger);

            Assert.IsType(typeof(DebugLogger), bootstrapper.BaseLogger);
        }

        [Fact]
        public void ApplicationDefaultsToNull()
        {
            var bootstrapper = new DefaultBootstrapper();
            Assert.Null(bootstrapper.BaseApp);
        }

        [Fact]
        public void BootstrapperRuns()
        {
            var bootstrapper = new DefaultBootstrapper();
            bootstrapper.Run(null);

            Assert.True(bootstrapper.RunCalled);
            Assert.Null(bootstrapper.BaseApp);
            Assert.True(bootstrapper.ConfigureViewModelLocatorCalled);
        }
    }

    public class DefaultBootstrapper : Bootstrapper
    {
        public ILoggerFacade BaseLogger
        {
            get { return base.Logger; }
        }

        public Application BaseApp
        {
            get { return base.App; }
            set { base.App = value; }
        }

        public bool RunCalled { get; set; } = false;

        public bool ConfigureViewModelLocatorCalled { get; set; } = false;

        public void CallCreateLogger()
        {
            this.Logger = base.CreateLogger();
        }

        public override void Run()
        {
            RunCalled = true;
        }

        protected override void ConfigureViewModelLocator()
        {
            ConfigureViewModelLocatorCalled = true;
        }

        protected override Page CreateMainPage()
        {
            throw new NotImplementedException();
        }

        protected override void RegisterTypes()
        {
            throw new NotImplementedException();
        }

        protected override INavigationService CreateNavigationService()
        {
            throw new NotImplementedException();
        }

        protected override void OnInitialized()
        {
            throw new NotImplementedException();
        }
    }
}
