using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.IocContainer.Wpf.Tests.Support.Mocks;
using Prism.IocContainer.Wpf.Tests.Support.Mocks.ViewModels;
using Prism.IocContainer.Wpf.Tests.Support.Mocks.Views;
using Prism.Mvvm;
using Prism.Ninject.Wpf.Tests.Mocks;

namespace Prism.Ninject.Wpf.Tests
{
    [TestClass]
    public class NinjectViewModelLocatorFixture
    {
        [TestMethod]
        public void ShouldLocateViewModelAndResolveWithKernel()
        {
            var bootstrapper = new DefaultNinjectBootstrapper();
            bootstrapper.Run();

            bootstrapper.Kernel.Bind<IService>().To<MockService>();

            var view = new MockView();
            Assert.IsNull(view.DataContext);

            ViewModelLocator.SetAutoWireViewModel(view, true);
            Assert.IsNotNull(view.DataContext);
            Assert.IsInstanceOfType(view.DataContext, typeof(MockViewModel));

            Assert.IsNotNull(((MockViewModel) view.DataContext).MockService);
        }
    }
}