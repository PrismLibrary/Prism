using System;
using System.Collections.Generic;
using System.Text;
using Prism.Ioc;
using Prism.DI.Forms.Tests.Mocks.Services;
using Xunit;
using Prism.DI.Forms.Tests.Mocks.Views;
using System.Linq;
using Xamarin.Forms.Xaml;
using System.Reflection;
using Prism.DI.Forms.Tests.Mocks.ViewModels;

#if DryIoc
using ContainerExtension = Prism.DryIoc.DryIocContainerExtension;

namespace Prism.DryIoc.Forms.Tests.Fixtures
#elif Unity
using ContainerExtension = Prism.Unity.UnityContainerExtension;

namespace Prism.Unity.Forms.Tests.Fixtures
#endif
{
    public class ContainerResolutionExceptionFixture
    {
        [Fact]
        public void ThrowsContainerResolutionExceptionForUnregisteredService()
        {
            var container = new ContainerExtension();
            var ex = Record.Exception(() => container.Resolve<IServiceMock>());

            Assert.NotNull(ex);
            Assert.IsType<ContainerResolutionException>(ex);
        }

        [Fact]
        public void ThrowsContainerResolutionExceptionForUnregisteredNamedPage()
        {
            var container = new ContainerExtension();
            var ex = Record.Exception(() => container.Resolve<object>("missing"));

            Assert.NotNull(ex);
            Assert.IsType<ContainerResolutionException>(ex);
        }

        [Fact]
        public void GetErrorsDoesNotThrowException()
        {
            ContainerLocator.ResetContainer();
            var container = new ContainerExtension();
            ContainerLocator.SetContainerExtension(() => container);
            container.Register<object, BadView>("BadView");

            var ex = Record.Exception(() => container.Resolve<object>("BadView"));

            Assert.IsType<ContainerResolutionException>(ex);
            var cre = ex as ContainerResolutionException;
            ContainerResolutionErrorCollection errors = null;
            var ex2 = Record.Exception(() => errors = cre.GetErrors());
            Assert.Null(ex2);
        }

        [Fact]
        public void GetErrorsLocatesIssueWithBadView()
        {
            ContainerLocator.ResetContainer();
            var container = new ContainerExtension();
            ContainerLocator.SetContainerExtension(() => container);
            container.Register<object, BadView>("BadView");

            var ex = Record.Exception(() => container.Resolve<object>("BadView"));

            Assert.IsType<ContainerResolutionException>(ex);
            var cre = ex as ContainerResolutionException;
            var errors = cre.GetErrors();

            Assert.Contains(typeof(BadView), errors.Types);
        }

        [Fact]
        public void GetErrorsLocatesTargetInvocationException()
        {
            ContainerLocator.ResetContainer();
            var container = new ContainerExtension();
            ContainerLocator.SetContainerExtension(() => container);
            container.Register<object, BadView>("BadView");

            var ex = Record.Exception(() => container.Resolve<object>("BadView"));

            Assert.IsType<ContainerResolutionException>(ex);
            var cre = ex as ContainerResolutionException;
            var errors = cre.GetErrors();

            Assert.Contains(errors, x => x.Value is TargetInvocationException);
        }

        [Fact]
        public void GetErrorsLocatesXamlParseException()
        {
            ContainerLocator.ResetContainer();
            var container = new ContainerExtension();
            ContainerLocator.SetContainerExtension(() => container);
            container.Register<object, BadView>("BadView");

            var ex = Record.Exception(() => container.Resolve<object>("BadView"));

            Assert.IsType<ContainerResolutionException>(ex);
            var cre = ex as ContainerResolutionException;
            var errors = cre.GetErrors();

            Assert.Contains(errors, x => x.Value is XamlParseException);
        }

        [Fact]
        public void LocatesUnregisteredServiceType()
        {
            ContainerLocator.ResetContainer();
            var container = new ContainerExtension();
            ContainerLocator.SetContainerExtension(() => container);

            var ex = Record.Exception(() => container.Resolve<ConstructorArgumentViewModel>());

            Assert.IsType<ContainerResolutionException>(ex);
            var cre = ex as ContainerResolutionException;
            var errors = cre.GetErrors();

            Assert.Contains(typeof(IServiceMock), errors.Types);
        }


        [Fact]
        public void LocatesUnregisteredServiceWithMissingRegistration()
        {
            ContainerLocator.ResetContainer();
            var container = new ContainerExtension();
            ContainerLocator.SetContainerExtension(() => container);

            var ex = Record.Exception(() => container.Resolve<ConstructorArgumentViewModel>());

            Assert.IsType<ContainerResolutionException>(ex);
            var cre = ex as ContainerResolutionException;
            var errors = cre.GetErrors();

            Assert.Contains(errors, x => x.Value is ContainerResolutionException innerCre && innerCre.ServiceType == typeof(IServiceMock) && innerCre.Message == ContainerResolutionException.MissingRegistration);
        }
    }
}
