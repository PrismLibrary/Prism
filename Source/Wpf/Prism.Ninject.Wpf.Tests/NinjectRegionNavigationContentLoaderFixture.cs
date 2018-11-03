using System.Linq;
using CommonServiceLocator;
using Xunit;
using Ninject;
using Prism.IocContainer.Wpf.Tests.Support.Mocks.Views;
using Prism.Ninject.Wpf.Tests.Mocks;
using Prism.Regions;

namespace Prism.Ninject.Wpf.Tests
{
    [Collection("ServiceLocator")]
    public class NinjectRegionNavigationContentLoaderFixture
    {
        [StaFact]
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

            Assert.True(testRegion.Views.Contains(view));
            Assert.True(testRegion.Views.Count() == 1);
            Assert.True(testRegion.ActiveViews.Count() == 1);
            Assert.True(testRegion.ActiveViews.Contains(view));
        }

        [StaFact]
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

            Assert.True(testRegion.Views.Contains(view));
            Assert.True(testRegion.ActiveViews.Count() == 1);
            Assert.True(testRegion.ActiveViews.Contains(view));
        }

        public void ConfigureMockServiceLocator(IKernel kernel)
        {
            var serviceLocator = new MockServiceLocator(kernel);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }
    }
}