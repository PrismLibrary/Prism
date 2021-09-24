using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Prism.Ioc;
using Prism.Ioc.Mocks.Services;
using Prism.Ioc.Mocks.ViewModels;
using Prism.Ioc.Mocks.Views;
using Xunit;

namespace Prism.Ioc.Tests
{
    [Collection(nameof(ContainerExtension))]
    public class ContainerResolutionExceptionFixture : TestBase
    {
        public ContainerResolutionExceptionFixture(ContainerSetup setup)
            : base(setup)
        {
        }

        [Fact]
        public void ThrowsContainerResolutionExceptionForUnregisteredService()
        {
            var container = Setup.CreateContainer();
            var ex = Record.Exception(() => container.Resolve<IServiceA>());

            Assert.NotNull(ex);
            Assert.IsType<ContainerResolutionException>(ex);
        }

        [Fact]
        public void ThrowsContainerResolutionExceptionForUnregisteredNamedPage()
        {
            var container = Setup.CreateContainer();
            var ex = Record.Exception(() => container.Resolve<object>("missing"));

            Assert.NotNull(ex);
            Assert.IsType<ContainerResolutionException>(ex);
        }

        [Fact]
        public void GetErrorsDoesNotThrowException()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<object, BadView>("BadView");

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
            var container = Setup.CreateContainer();
            Setup.Registry.Register<object, BadView>("BadView");

            var ex = Record.Exception(() => container.Resolve<object>("BadView"));

            Assert.IsType<ContainerResolutionException>(ex);
            var cre = ex as ContainerResolutionException;
            var errors = cre.GetErrors();

            Assert.Contains(typeof(BadView), errors.Types);
        }

        [Fact]
        public void GetErrorsLocatesTargetInvocationException()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<object, BadView>("BadView");

            var ex = Record.Exception(() => container.Resolve<object>("BadView"));

            Assert.IsType<ContainerResolutionException>(ex);
            var cre = ex as ContainerResolutionException;
            var errors = cre.GetErrors();

            Assert.Contains(errors, x => x.Value is TargetInvocationException);
        }

        [Fact]
        public void GetErrorsLocatesXamlParseException()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<object, BadView>("BadView");

            var ex = Record.Exception(() => container.Resolve<object>("BadView"));

            Assert.IsType<ContainerResolutionException>(ex);
            var cre = ex as ContainerResolutionException;
            var errors = cre.GetErrors();

            Assert.Contains(errors, x => x.Value is XamlParseException);
        }

        [Fact]
        public void LocatesUnregisteredServiceType()
        {
            var container = Setup.CreateContainer();

            var ex = Record.Exception(() => container.Resolve<ConstructorArgumentViewModel>());

            Assert.IsType<ContainerResolutionException>(ex);
            var cre = ex as ContainerResolutionException;
            var errors = cre.GetErrors();

            Assert.Contains(typeof(IServiceA), errors.Types);
        }

        [Fact]
        public void LocatesUnregisteredServiceWithMissingRegistration()
        {
            var container = Setup.CreateContainer();

            var ex = Record.Exception(() => container.Resolve<ConstructorArgumentViewModel>());

            Assert.IsType<ContainerResolutionException>(ex);
            var cre = ex as ContainerResolutionException;
            var errors = cre.GetErrors();

            Assert.Contains(errors, x => x.Value is ContainerResolutionException innerCre && innerCre.ServiceType == typeof(IServiceA) && innerCre.Message == ContainerResolutionException.MissingRegistration);
        }
    }
}
