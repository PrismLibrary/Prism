using Grace.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.IocContainer.Wpf.Tests.Support.Mocks;
using Prism.IocContainer.Wpf.Tests.Support.Mocks.ViewModels;
using Prism.IocContainer.Wpf.Tests.Support.Mocks.Views;
using Prism.Mvvm;

namespace Prism.Grace.Wpf.Tests
{
    [TestClass]
    public class GraceViewModelLocatorFixture
    {
        [TestMethod]
        public void ShouldLocateViewModelAndResolveWithContainer()
        {
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();

            bootstrapper.BaseContainer.Configure(c => c.ExportAs<MockService, IService>());

            MockView view = new MockView();
            Assert.IsNull(view.DataContext);

            ViewModelLocator.SetAutoWireViewModel(view, true);
            Assert.IsNotNull(view.DataContext);
            Assert.IsInstanceOfType(view.DataContext, typeof(MockViewModel));

            Assert.IsNotNull(((MockViewModel)view.DataContext).MockService);
        }
    }
}
