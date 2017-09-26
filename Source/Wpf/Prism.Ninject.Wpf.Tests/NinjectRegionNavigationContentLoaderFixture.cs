using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Prism.IocContainer.Wpf.Tests.Support.Mocks.Views;
using Prism.Ninject.Wpf.Tests.Mocks;
using Prism.Regions;

namespace Prism.Ninject.Wpf.Tests
{
    [TestClass]
    public class NinjectRegionNavigationContentLoaderFixture
    {
        [TestMethod]
        public void ShouldFindCandidateViewInRegion()
        {
            IKernel kernel = new StandardKernel();
            kernel.Bind<object, MockView>().To<MockView>().Named("MockView");

            ConfigureMockServiceLocator(kernel);

            IRegion testRegion = new Region();

            var view = new MockView();
            testRegion.Add(view);
            testRegion.Deactivate(view);

            testRegion.RequestNavigate("MockView");

            Assert.IsTrue(testRegion.Views.Contains(view));
            Assert.IsTrue(testRegion.Views.Count() == 1);
            Assert.IsTrue(testRegion.ActiveViews.Count() == 1);
            Assert.IsTrue(testRegion.ActiveViews.Contains(view));
        }

        [TestMethod]
        public void ShouldFindCandidateViewWithFriendlyNameInRegion()
        {
            IKernel kernel = new StandardKernel();
            kernel.Bind<object, MockView>().To<MockView>().Named("SomeView");

            ConfigureMockServiceLocator(kernel);

            IRegion testRegion = new Region();

            var view = new MockView();
            testRegion.Add(view);
            testRegion.Deactivate(view);

            testRegion.RequestNavigate("SomeView");

            Assert.IsTrue(testRegion.Views.Contains(view));
            Assert.IsTrue(testRegion.ActiveViews.Count() == 1);
            Assert.IsTrue(testRegion.ActiveViews.Contains(view));
        }

        public void ConfigureMockServiceLocator(IKernel kernel)
        {
            MockServiceLocator serviceLocator = new MockServiceLocator(kernel);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }
    }
}