using DryIoc;
using CommonServiceLocator;
using Xunit;
using Prism.IocContainer.Wpf.Tests.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            IServiceLocator serviceLocator = bootstrapper.Container.Resolve<IServiceLocator>();
            object viewInstance = serviceLocator.GetInstance<object>(nameof(NavigateView));

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
