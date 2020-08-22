using System;
using Moq;
using Prism;
using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation;

namespace MockApp
{
    [AutoRegisterForNavigation]
    public class MockPrismApp : PrismApplicationBase, IApplicationProvider
    {
        public Mock<IContainerExtension> MockContainer { get; private set; }

        public static Func<Type, string> PageNameDelegate { get; set; }

        protected override void Initialize()
        {
            ContainerLocator.ResetContainer();
            MockContainer = new Mock<IContainerExtension>();
            MockContainer.Setup(x => x.Resolve(typeof(IModuleCatalog)))
                .Returns(new ModuleCatalog());
            MockContainer.Setup(x => x.Resolve(typeof(INavigationService), NavigationServiceName))
                .Returns(new PageNavigationService(MockContainer.Object, this, new PageBehaviorFactory()));
            base.Initialize();
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            return MockContainer.Object;
        }

        protected override void OnInitialized()
        {
            // Clean up the Delegate for the next test.
            PageNameDelegate = null;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        protected override string GetNavigationSegmentNameFromType(Type pageType)
        {
            if(PageNameDelegate is null)
                return base.GetNavigationSegmentNameFromType(pageType);

            return PageNameDelegate(pageType);
        }
    }
}
