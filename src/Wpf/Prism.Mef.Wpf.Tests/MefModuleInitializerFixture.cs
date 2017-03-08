

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Logging;
using Prism.Mef.Modularity;
using Prism.Modularity;

namespace Prism.Mef.Wpf.Tests
{
    [TestClass]
    public class MefModuleInitializerFixture
    {
        [TestMethod]
        public void WhenInitializingAModuleWithNoCatalogPendingToBeLoaded_ThenInitializesTheModule()
        {
            var aggregateCatalog = new AggregateCatalog(new AssemblyCatalog(typeof(MefModuleInitializer).Assembly));
            var compositionContainer = new CompositionContainer(aggregateCatalog);
            compositionContainer.ComposeExportedValue(aggregateCatalog);

            var serviceLocatorMock = new Mock<IServiceLocator>();
            var loggerFacadeMock = new Mock<ILoggerFacade>();

            var serviceLocator = serviceLocatorMock.Object;
            var loggerFacade = loggerFacadeMock.Object;

            compositionContainer.ComposeExportedValue(serviceLocator);
            compositionContainer.ComposeExportedValue(loggerFacade);

            aggregateCatalog.Catalogs.Add(new TypeCatalog(typeof(TestModuleForInitializer)));

            var moduleInitializer = compositionContainer.GetExportedValue<IModuleInitializer>();

            var moduleInfo = new ModuleInfo("TestModuleForInitializer", typeof(TestModuleForInitializer).AssemblyQualifiedName);

            var module = compositionContainer.GetExportedValues<IModule>().OfType<TestModuleForInitializer>().First();

            Assert.IsFalse(module.Initialized);

            moduleInitializer.Initialize(moduleInfo);

            Assert.IsTrue(module.Initialized);
        }

        [TestMethod]
        public void WhenInitializingAModuleWithACatalogPendingToBeLoaded_ThenLoadsTheCatalogInitializesTheModule()
        {
            var aggregateCatalog = new AggregateCatalog(new AssemblyCatalog(typeof(MefModuleInitializer).Assembly));
            var compositionContainer = new CompositionContainer(aggregateCatalog);
            compositionContainer.ComposeExportedValue(aggregateCatalog);

            var serviceLocatorMock = new Mock<IServiceLocator>();
            var loggerFacadeMock = new Mock<ILoggerFacade>();

            var serviceLocator = serviceLocatorMock.Object;
            var loggerFacade = loggerFacadeMock.Object;

            compositionContainer.ComposeExportedValue(serviceLocator);
            compositionContainer.ComposeExportedValue(loggerFacade);

            var moduleInitializer = compositionContainer.GetExportedValue<IModuleInitializer>();
            var repository = compositionContainer.GetExportedValue<DownloadedPartCatalogCollection>();

            var moduleInfo = new ModuleInfo("TestModuleForInitializer", typeof(TestModuleForInitializer).AssemblyQualifiedName);

            repository.Add(moduleInfo, new TypeCatalog(typeof(TestModuleForInitializer)));

            moduleInitializer.Initialize(moduleInfo);

            ComposablePartCatalog existingCatalog;
            Assert.IsFalse(repository.TryGet(moduleInfo, out existingCatalog));

            var module = compositionContainer.GetExportedValues<IModule>().OfType<TestModuleForInitializer>().First();

            Assert.IsTrue(module.Initialized);
        }
    }

    [ModuleExport(typeof(TestModuleForInitializer))]
    public class TestModuleForInitializer : IModule
    {
        public void Initialize()
        {
            this.Initialized = true;
        }

        public bool Initialized { get; private set; }
    }

}
