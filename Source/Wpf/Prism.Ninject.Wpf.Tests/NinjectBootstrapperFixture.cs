using System;
using Xunit;
using Ninject;
using Prism.IocContainer.Wpf.Tests.Support;
using Prism.Logging;
using Prism.Modularity;
using Prism.Ninject.Wpf.Tests.Mocks;
using Prism.Regions;

namespace Prism.Ninject.Wpf.Tests
{
    [Collection("ServiceLocator")]
    public class NinjectBootstrapperFixture : BootstrapperFixtureBase
    {
        [StaFact]
        public void KernelDefaultsToNull()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            var kernel = bootstrapper.Kernel;

            Assert.Null(kernel);
        }

        [StaFact]
        public void CanCreateConcreteBootstrapper()
        {
            new DefaultNinjectBootstrapper();
        }

        [StaFact]
        public void CreateKernelShouldInitializeKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            IKernel kernel = bootstrapper.CallCreateKernel();

            Assert.NotNull(kernel);
            Assert.IsAssignableFrom<IKernel>(kernel);
        }

        [StaFact]
        public void ConfigureKernelAddsModuleCatalogToKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            var returnedCatalog = bootstrapper.Kernel.Get<IModuleCatalog>();
            Assert.NotNull(returnedCatalog);
            Assert.True(returnedCatalog is ModuleCatalog);
        }

        [StaFact]
        public void ConfigureKernelAddsLoggerFacadeToKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            var returnedCatalog = bootstrapper.Kernel.Get<ILoggerFacade>();
            Assert.NotNull(returnedCatalog);
        }

        [StaFact]
        public void ConfigureKernelAddsRegionNavigationJournalEntryToKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.Kernel.Get<IRegionNavigationJournalEntry>();
            var actual2 = bootstrapper.Kernel.Get<IRegionNavigationJournalEntry>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotSame(actual1, actual2);
        }

        [StaFact]
        public void ConfigureKernelAddsRegionNavigationJournalToKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.Kernel.Get<IRegionNavigationJournal>();
            var actual2 = bootstrapper.Kernel.Get<IRegionNavigationJournal>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotSame(actual1, actual2);
        }

        [StaFact]
        public void ConfigureKernelAddsRegionNavigationServiceToKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.Kernel.Get<IRegionNavigationService>();
            var actual2 = bootstrapper.Kernel.Get<IRegionNavigationService>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotSame(actual1, actual2);
        }

        [StaFact]
        public void ConfigureKernelAddsNavigationTargetHandlerToKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.Kernel.Get<IRegionNavigationContentLoader>();
            var actual2 = bootstrapper.Kernel.Get<IRegionNavigationContentLoader>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.Same(actual1, actual2);
        }

        [StaFact]
        public void RegisterFrameworkExceptionTypesShouldRegisterActivationException()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            bootstrapper.CallRegisterFrameworkExceptionTypes();

            Assert.True(ExceptionExtensions.IsFrameworkExceptionRegistered(
                typeof(ActivationException)));
        }
    }
}