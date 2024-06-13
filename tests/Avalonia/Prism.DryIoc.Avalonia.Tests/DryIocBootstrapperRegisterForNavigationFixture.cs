using CommonServiceLocator;
using DryIoc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.IocContainer.Avalonia.Tests.Support;

namespace Prism.DryIoc.Avalonia.Tests
{
    [TestClass]
    public class DryIocBootstrapperRegisterForNavigationFixture : BootstrapperFixtureBase
    {
        [TestMethod]
        public void RunCheckIfViewRegisteredForNavigationCanBeResolvedTroughServiceLocatorWithObjectServiceType()
        {
            var bootstrapper = new RegisterForNavigationBootstrapper();
            bootstrapper.Run();
            IServiceLocator serviceLocator = bootstrapper.Container.Resolve<IServiceLocator>();
            object viewInstance = serviceLocator.GetInstance<object>(nameof(NavigateView));

            Assert.IsNotNull(viewInstance);
            Assert.IsInstanceOfType(viewInstance, typeof(NavigateView));
        }

        private class RegisterForNavigationBootstrapper : DryIocBootstrapper
        {
            protected override void ConfigureContainer()
            {
                base.ConfigureContainer();
                Container.RegisterTypeForNavigation<NavigateView>();
            }
        }

        public class NavigateView
        {

        }
    }
}
