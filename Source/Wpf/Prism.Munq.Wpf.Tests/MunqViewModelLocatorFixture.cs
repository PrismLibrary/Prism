using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.IocContainer.Wpf.Tests.Support.Mocks;
using Prism.IocContainer.Wpf.Tests.Support.Mocks.ViewModels;
using Prism.IocContainer.Wpf.Tests.Support.Mocks.Views;
using Prism.Mvvm;

namespace Prism.Munq.Wpf.Tests
{
    [TestClass]
    public class MunqViewModelLocatorFixture
    {
        [TestMethod]
        public void ShouldLocateViewModelAndResolveWithContainer()
        {
            var bootstrapper = new DefaultMunqBootstrapper();
            bootstrapper.Run();

            bootstrapper.BaseContainer.Register<IService, MockService>();

            MockView view = new MockView();
            Assert.IsNull(view.DataContext);

            ViewModelLocator.SetAutoWireViewModel(view, true);
            Assert.IsNotNull(view.DataContext);
            Assert.IsInstanceOfType(view.DataContext, typeof(MockViewModel));

            Assert.IsNotNull(((MockViewModel)view.DataContext).MockService);
        }
    }
}
