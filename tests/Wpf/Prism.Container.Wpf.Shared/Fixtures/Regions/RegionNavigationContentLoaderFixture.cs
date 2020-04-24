using System.Linq;
using Prism.Ioc;
using Prism.IocContainer.Wpf.Tests.Support.Mocks.Views;
using Prism.Regions;
using Xunit;
using static Prism.Container.Wpf.Tests.ContainerHelper;

namespace Prism.Container.Wpf.Tests.Regions
{
    [Collection(nameof(ContainerExtension))]
    public class RegionNavigationContentLoaderFixture
    {
        readonly IContainerExtension _container;

        public RegionNavigationContentLoaderFixture()
        {
            _container = CreateContainerExtension();
            _container.RegisterInstance<IContainerExtension>(_container);
            _container.Register<IRegionNavigationService, RegionNavigationService>();
            _container.Register(typeof(IRegionNavigationContentLoader), typeof(RegionNavigationContentLoader));
            _container.Register<IRegionNavigationJournal, RegionNavigationJournal>();
            ContainerLocator.ResetContainer();
            ContainerLocator.SetContainerExtension(() => _container);
        }

        [StaFact]
        public void ShouldFindCandidateViewInRegion()
        {
            _container.RegisterForNavigation<MockView>();
            //_container.RegisterType<object, MockView>("MockView");

            // We cannot access the UnityRegionNavigationContentLoader directly so we need to call its
            // GetCandidatesFromRegion method through a navigation request.
            IRegion testRegion = new Region();

            MockView view = new MockView();
            testRegion.Add(view);
            testRegion.Deactivate(view);

            Assert.True(((IContainerRegistry)_container).IsRegistered<object>("MockView"));

            testRegion.RequestNavigate("MockView");

            Assert.Contains(view, testRegion.Views);
            Assert.Single(testRegion.Views);
            Assert.Single(testRegion.ActiveViews);
            Assert.Contains(view, testRegion.ActiveViews);
        }

        [StaFact]
        public void ShouldFindCandidateViewWithFriendlyNameInRegion()
        {
            _container.RegisterForNavigation<MockView>("SomeView");

            // We cannot access the Container specific RegionNavigationContentLoader directly so we need to call its
            // GetCandidatesFromRegion method through a navigation request.
            IRegion testRegion = new Region();

            var view = _container.Resolve<object>("SomeView") as MockView;
            testRegion.Add(view);
            testRegion.Deactivate(view);

            Assert.Empty(testRegion.ActiveViews);
            testRegion.RequestNavigate("SomeView");

            Assert.Contains(view, testRegion.Views);
            Assert.Single(testRegion.Views);
            Assert.Single(testRegion.ActiveViews);
            Assert.Contains(view, testRegion.ActiveViews);
        }
    }
}
