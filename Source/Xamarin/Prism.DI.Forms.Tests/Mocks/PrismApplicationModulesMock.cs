using Prism.DI.Forms.Tests.Mocks.Modules;
using Prism.Modularity;

namespace Prism.DI.Forms.Tests
{
    public class PrismApplicationModulesMock : PrismApplicationMock
    {
        public PrismApplicationModulesMock(IPlatformInitializer platformInitializer)
            : base(platformInitializer)
        {

        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule(new ModuleInfo(typeof(ModuleMock))
            {
                InitializationMode = InitializationMode.WhenAvailable,
                ModuleName = "ModuleMock"
            });
        }
    }
}