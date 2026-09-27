using System;
using Moq;
using Prism.Container.Unity;
using Prism.Container.Wpf.Mocks;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using Unity;
using Unity.Lifetime;
using Xunit;

namespace Prism.Container.Wpf.Tests.Bootstrapper
{
    public partial class BootstrapperRunMethodFixture
    {
        [StaFact]
        public void RunAddsCompositionContainerToContainer()
        {
            var bootstrapper = new MockBootstrapper();

            var createdContainer = bootstrapper.CallCreateContainer();
            var returnedContainer = createdContainer.Resolve<IUnityContainer>();
            Assert.NotNull(returnedContainer);
            Assert.Equal(typeof(UnityContainer), returnedContainer.GetType());
        }

        [StaFact]
        public void RunRegistersInstanceOfIModuleCatalog()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterInstance(typeof(IModuleCatalog), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IInstanceLifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIModuleInitializer()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IModuleInitializer), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<ITypeLifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionManager()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IRegionManager), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<ITypeLifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForRegionAdapterMappings()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(RegionAdapterMappings), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<ITypeLifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionViewRegistry()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IRegionViewRegistry), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<ITypeLifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionBehaviorFactory()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IRegionBehaviorFactory), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<ITypeLifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIEventAggregator()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.RegisterType(typeof(IEventAggregator), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<ITypeLifetimeManager>()), Times.Once());
        }

        [StaFact]
        public void RunFalseShouldNotRegisterDefaultServicesAndTypes()
        {
            var mockedContainer = new Mock<IUnityContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);
            bootstrapper.Run(false);

            mockedContainer.Verify(c => c.RegisterType(typeof(IEventAggregator), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<ITypeLifetimeManager>()), Times.Never());
            mockedContainer.Verify(c => c.RegisterType(typeof(IRegionManager), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<ITypeLifetimeManager>()), Times.Never());
            mockedContainer.Verify(c => c.RegisterType(typeof(RegionAdapterMappings), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<ITypeLifetimeManager>()), Times.Never());
            mockedContainer.Verify(c => c.RegisterType(typeof(IModuleInitializer), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<ITypeLifetimeManager>()), Times.Never());
        }

        private static void SetupMockedContainerForVerificationTests(Mock<IUnityContainer> mockedContainer)
        {
            var mockedModuleInitializer = new Mock<IModuleInitializer>();
            var mockedModuleManager = new Mock<IModuleManager>();
            var regionAdapterMappings = new RegionAdapterMappings();

            var containerExtension = new UnityContainerExtension(mockedContainer.Object);
            var regionBehaviorFactory = new RegionBehaviorFactory(containerExtension);

            mockedContainer.Setup(c => c.RegisterInstance(It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IInstanceLifetimeManager>()));

            mockedContainer.Setup(c => c.Resolve(typeof(IModuleCatalog), It.IsAny<string>())).Returns(
                new ModuleCatalog());

            mockedContainer.Setup(c => c.Resolve(typeof(IModuleInitializer), It.IsAny<string>())).Returns(
                mockedModuleInitializer.Object);

            mockedContainer.Setup(c => c.Resolve(typeof(IModuleManager), It.IsAny<string>())).Returns(
                mockedModuleManager.Object);

            mockedContainer.Setup(c => c.Resolve(typeof(RegionAdapterMappings), It.IsAny<string>())).Returns(
                regionAdapterMappings);

            mockedContainer.Setup(c => c.Resolve(typeof(SelectorRegionAdapter), It.IsAny<string>())).Returns(
                new SelectorRegionAdapter(regionBehaviorFactory));

            mockedContainer.Setup(c => c.Resolve(typeof(ItemsControlRegionAdapter), It.IsAny<string>())).Returns(
                new ItemsControlRegionAdapter(regionBehaviorFactory));

            mockedContainer.Setup(c => c.Resolve(typeof(ContentControlRegionAdapter), It.IsAny<string>())).Returns(
                new ContentControlRegionAdapter(regionBehaviorFactory));
        }
    }
}
