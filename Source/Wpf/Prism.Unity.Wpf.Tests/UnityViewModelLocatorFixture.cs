using Unity;
using Xunit;
using Prism.IocContainer.Wpf.Tests.Support.Mocks;
using Prism.IocContainer.Wpf.Tests.Support.Mocks.ViewModels;
using Prism.IocContainer.Wpf.Tests.Support.Mocks.Views;
using Prism.Mvvm;

namespace Prism.Unity.Wpf.Tests
{
    [Collection("ServiceLocator")]
    public class UnityViewModelLocatorFixture
    {
        [StaFact]
        public void ShouldLocateViewModelAndResolveWithContainer()
        {
            var bootstrapper = new DefaultUnityBootstrapper();
            bootstrapper.Run();

            bootstrapper.BaseContainer.RegisterType<IService, MockService>();

            MockView view = new MockView();
            Assert.Null(view.DataContext);

            ViewModelLocator.SetAutoWireViewModel(view, true);
            Assert.NotNull(view.DataContext);
            Assert.IsType<MockViewModel>(view.DataContext);

            Assert.NotNull(((MockViewModel)view.DataContext).MockService);
        }
    }
}
