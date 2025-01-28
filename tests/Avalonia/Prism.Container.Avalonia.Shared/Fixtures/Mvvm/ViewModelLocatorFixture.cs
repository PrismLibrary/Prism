using Prism.Container.Avalonia.Mocks;
using Prism.Ioc;
using Prism.IocContainer.Avalonia.Tests.Support.Mocks;
using Prism.IocContainer.Avalonia.Tests.Support.Mocks.ViewModels;
using Prism.IocContainer.Avalonia.Tests.Support.Mocks.Views;
using Prism.Mvvm;
using Xunit;

namespace Prism.Container.Avalonia.Tests.Mvvm
{
    [Collection(nameof(ContainerExtension))]
    public class ViewModelLocatorFixture
    {
        [StaFact]
        public void ShouldLocateViewModelAndResolveWithContainer()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();

            bootstrapper.ContainerRegistry.Register<IService, MockService>();

            var view = new MockView();
            Assert.Null(view.DataContext);

            ViewModelLocator.SetAutoWireViewModel(view, true);
            Assert.NotNull(view.DataContext);
            Assert.IsType<MockViewModel>(view.DataContext);

            Assert.NotNull(((MockViewModel)view.DataContext).MockService);
        }
    }
}
