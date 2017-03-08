

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Mef.Modularity;
using Prism.Modularity;

namespace Prism.Mef.Wpf.Tests
{
    [TestClass]
    public class MefFileModuleTypeLoaderFixture
    {
        [TestMethod]
        public void CanLoadModulesWithUrlThatHaveFilePrefix()
        {
            MefFileModuleTypeLoader loader = new MefFileModuleTypeLoader();
            ModuleInfo info = this.CreateModuleInfo();

            bool canLoad = loader.CanLoadModuleType(info);

            Assert.IsTrue(canLoad);
        }

        [TestMethod]
        public void CannotLoadModulesWithUrlThatDontHaveFilePrefix()
        {
            MefFileModuleTypeLoader loader = new MefFileModuleTypeLoader();
            ModuleInfo info = this.CreateModuleInfo();
            info.Ref = "MefModulesForTesting.dll";

            bool canLoad = loader.CanLoadModuleType(info);

            Assert.IsFalse(canLoad);
        }

        [TestMethod]
        public void LoadModuleTypeShouldRaiseTheCorrespondingEvents()
        {
            AggregateCatalog catalog = new AggregateCatalog();
            var newCatalog = DefaultPrismServiceRegistrar.RegisterRequiredPrismServicesIfMissing(catalog);

            CompositionContainer container = new CompositionContainer(newCatalog);
            container.ComposeExportedValue<AggregateCatalog>(catalog);

            MefFileModuleTypeLoader loader = container.GetExportedValue<MefFileModuleTypeLoader>();
            ModuleInfo info = this.CreateModuleInfo();

            // Flags
            bool progressChangedRaised = false;
            bool completedRaised = false;

            // We subscribe to the events
            loader.ModuleDownloadProgressChanged += (o, e) => { progressChangedRaised = true; };
            loader.LoadModuleCompleted += (o, e) => { completedRaised = true; };

            loader.LoadModuleType(info);

            Assert.IsTrue(progressChangedRaised);
            Assert.IsTrue(completedRaised);
        }

        public ModuleInfo CreateModuleInfo()
        {
            ModuleInfo info = new ModuleInfo();
            info.ModuleName = "MefModuleOne";
            info.Ref = "file:///MefModulesForTesting.dll";

            return info;
        }
    }
}
