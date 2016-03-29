using Prism.Modularity;

namespace Prism.DryIoc.Forms.Tests.Mocks.Modules
{
    public class ModuleMock : IModule
    {
        public bool Initialized { get; private set; }

        public void Initialize()
        {
            Initialized = true;
        }
    }    
}
