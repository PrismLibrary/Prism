using DryIoc;
using Prism.DryIoc.Forms.Tests.Mocks.ViewModels;
using Prism.DryIoc.Forms.Tests.Mocks.Views;
using Prism.DryIoc.Forms.Tests.Services;

namespace Prism.DryIoc.Forms.Tests.Mocks
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
            Container.RegisterTypeForNavigation<ViewMock>("view");
            Container.RegisterTypeForNavigation<ViewAMock, ViewModelAMock>();
            Container.Register<ViewModelAMock>();
            Container.Register<ViewModelBMock>(serviceKey: ViewModelBMock.Key);
            Container.RegisterTypeForNavigation<AutowireView, AutowireViewModel>();
        }
    }
}