using Xunit;
using Prism.IocContainer.Wpf.Tests.Support.Mocks;
using Prism.IocContainer.Wpf.Tests.Support.Mocks.ViewModels;
using Prism.IocContainer.Wpf.Tests.Support.Mocks.Views;
using Prism.Mvvm;
using Prism.Ninject.Wpf.Tests.Mocks;

namespace Prism.Ninject.Wpf.Tests
{
    [Collection("ServiceLocator")]
    public class NinjectViewModelLocatorFixture
    {
        [StaFact]
        public void ShouldLocateViewModelAndResolveWithKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            bootstrapper.Kernel.Bind<IService>().To<MockService>();

            var view = new MockView();
            Assert.Null(view.DataContext);

            ViewModelLocator.SetAutoWireViewModel(view, true);
            Assert.NotNull(view.DataContext);
            Assert.IsType<MockViewModel>(view.DataContext);

            Assert.NotNull(((MockViewModel) view.DataContext).MockService);
        }
    }
}