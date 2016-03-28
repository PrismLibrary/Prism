using DryIoc;
using Prism.DryIoc.Forms.Tests.Services;

namespace Prism.DryIoc.Forms.Tests
{
    public class PrismApplicationMock : PrismApplication
    {
        public bool Initialized { get; private set; }

        protected override void OnInitialized()
        {
            Initialized = true;
        }

        protected override void RegisterTypes()
        {
            Container.Register<IDryIocServiceMock, DryIocServiceMock>();
        }
    }
}