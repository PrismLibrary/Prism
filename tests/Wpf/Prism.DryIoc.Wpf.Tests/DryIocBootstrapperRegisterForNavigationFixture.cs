using DryIoc;
using Xunit;
using Prism.IocContainer.Wpf.Tests.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;

namespace Prism.DryIoc.Wpf.Tests
{
    [Collection("ServiceLocator")]
    public class DryIocBootstrapperRegisterForNavigationFixture : BootstrapperFixtureBase
    {
        [Fact]
        public void RunCheckIfViewRegisteredForNavigationCanBeResolvedTroughServiceLocatorWithObjectServiceType()
        {
            var bootstrapper = new RegisterForNavigationBootstrapper();
            bootstrapper.Run();

            object viewInstance = ContainerLocator.Container.Resolve<object>(nameof(NavigateView));

            Assert.NotNull(viewInstance);
            Assert.IsType<NavigateView>(viewInstance);
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
