

using System.Linq;
using CommonServiceLocator;
using Unity;
using Xunit;
using Prism.IocContainer.Wpf.Tests.Support.Mocks;
using Prism.IocContainer.Wpf.Tests.Support.Mocks.Views;
using Prism.Regions;
using Prism.Unity.Wpf.Tests.Mocks;

namespace Prism.Unity.Wpf.Tests
{
    
    public class UnityRegionNavigationContentLoaderFixture
    {
        [Fact]
        public void ShouldFindCandidateViewInRegion()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<object, MockView>("MockView");

            this.ConfigureMockServiceLocator(container);

            // We cannot access the UnityRegionNavigationContentLoader directly so we need to call its
            // GetCandidatesFromRegion method through a navigation request.
            IRegion testRegion = new Region();

            MockView view = new MockView();
            testRegion.Add(view);
            testRegion.Deactivate(view);

            testRegion.RequestNavigate("MockView");

            Assert.True(testRegion.Views.Contains(view));
            Assert.True(testRegion.Views.Count() == 1);
            Assert.True(testRegion.ActiveViews.Count() == 1);
            Assert.True(testRegion.ActiveViews.Contains(view));
        }

        [Fact]
        public void ShouldFindCandidateViewWithFriendlyNameInRegion()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<MockView>("SomeView");

            this.ConfigureMockServiceLocator(container);

            // We cannot access the UnityRegionNavigationContentLoader directly so we need to call its
            // GetCandidatesFromRegion method through a navigation request.
            IRegion testRegion = new Region();

            MockView view = new MockView();
            testRegion.Add(view);
            testRegion.Deactivate(view);

            testRegion.RequestNavigate("SomeView");

            Assert.True(testRegion.Views.Contains(view));
            Assert.True(testRegion.ActiveViews.Count() == 1);
            Assert.True(testRegion.ActiveViews.Contains(view));
        }

        public void ConfigureMockServiceLocator(IUnityContainer container)
        {
            MockServiceLocator serviceLocator = new MockServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }
    }
}
