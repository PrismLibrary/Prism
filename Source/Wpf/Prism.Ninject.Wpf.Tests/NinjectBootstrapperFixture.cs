using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Prism.IocContainer.Wpf.Tests.Support;
using Prism.Logging;
using Prism.Modularity;
using Prism.Ninject.Wpf.Tests.Mocks;
using Prism.Regions;

namespace Prism.Ninject.Wpf.Tests
{
    [TestClass]
    public class NinjectBootstrapperFixture : BootstrapperFixtureBase
    {
        [TestMethod]
        public void KernelDefaultsToNull()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            var kernel = bootstrapper.Kernel;

            Assert.IsNull(kernel);
        }

        [TestMethod]
        public void CanCreateConcreteBootstrapper()
        {
            new DefaultNinjectBootstrapper();
        }

        [TestMethod]
        public void CreateKernelShouldInitializeKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            IKernel kernel = bootstrapper.CallCreateKernel();

            Assert.IsNotNull(kernel);
            Assert.IsInstanceOfType(kernel, typeof(IKernel));
        }

        [TestMethod]
        public void ConfigureKernelAddsModuleCatalogToKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            var returnedCatalog = bootstrapper.Kernel.Get<IModuleCatalog>();
            Assert.IsNotNull(returnedCatalog);
            Assert.IsTrue(returnedCatalog is ModuleCatalog);
        }

        [TestMethod]
        public void ConfigureKernelAddsLoggerFacadeToKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            var returnedCatalog = bootstrapper.Kernel.Get<ILoggerFacade>();
            Assert.IsNotNull(returnedCatalog);
        }

        [TestMethod]
        public void ConfigureKernelAddsRegionNavigationJournalEntryToKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.Kernel.Get<IRegionNavigationJournalEntry>();
            var actual2 = bootstrapper.Kernel.Get<IRegionNavigationJournalEntry>();

            Assert.IsNotNull(actual1);
            Assert.IsNotNull(actual2);
            Assert.AreNotSame(actual1, actual2);
        }

        [TestMethod]
        public void ConfigureKernelAddsRegionNavigationJournalToKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.Kernel.Get<IRegionNavigationJournal>();
            var actual2 = bootstrapper.Kernel.Get<IRegionNavigationJournal>();

            Assert.IsNotNull(actual1);
            Assert.IsNotNull(actual2);
            Assert.AreNotSame(actual1, actual2);
        }

        [TestMethod]
        public void ConfigureKernelAddsRegionNavigationServiceToKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.Kernel.Get<IRegionNavigationService>();
            var actual2 = bootstrapper.Kernel.Get<IRegionNavigationService>();

            Assert.IsNotNull(actual1);
            Assert.IsNotNull(actual2);
            Assert.AreNotSame(actual1, actual2);
        }

        [TestMethod]
        public void ConfigureKernelAddsNavigationTargetHandlerToKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.Kernel.Get<IRegionNavigationContentLoader>();
            var actual2 = bootstrapper.Kernel.Get<IRegionNavigationContentLoader>();

            Assert.IsNotNull(actual1);
            Assert.IsNotNull(actual2);
            Assert.AreSame(actual1, actual2);
        }

        [TestMethod]
        public void RegisterFrameworkExceptionTypesShouldRegisterActivationException()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();

            bootstrapper.CallRegisterFrameworkExceptionTypes();

            Assert.IsTrue(ExceptionExtensions.IsFrameworkExceptionRegistered(
                typeof(ActivationException)));
        }
    }
}