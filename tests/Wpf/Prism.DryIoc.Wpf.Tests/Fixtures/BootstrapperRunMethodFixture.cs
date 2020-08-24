using System;
using DryIoc;
using Moq;
using Prism.Container.Wpf.Mocks;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
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
            var returnedContainer = createdContainer.Resolve<IContainer>();
            Assert.NotNull(returnedContainer);
            Assert.Equal(typeof(global::DryIoc.Container), returnedContainer.GetType());
        }

        [StaFact]
        public void RunRegistersInstanceOfIModuleCatalog()
        {
            var mockedContainer = new Mock<IContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.Register(It.IsAny<Factory>(), typeof(IModuleCatalog), It.IsAny<object>(), It.IsAny<IfAlreadyRegistered?>(), It.IsAny<bool>()));
        }

        [StaFact]
        public void RunRegistersTypeForIModuleInitializer()
        {
            var mockedContainer = new Mock<IContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.Register(It.IsAny<Factory>(), typeof(IModuleInitializer), null, It.IsAny<IfAlreadyRegistered?>(), It.IsAny<bool>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionManager()
        {
            var mockedContainer = new Mock<IContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.Register(It.IsAny<Factory>(), typeof(IRegionManager), null, It.IsAny<IfAlreadyRegistered?>(), It.IsAny<bool>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForRegionAdapterMappings()
        {
            var mockedContainer = new Mock<IContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.Register(It.IsAny<Factory>(), typeof(RegionAdapterMappings), null, It.IsAny<IfAlreadyRegistered?>(), It.IsAny<bool>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionViewRegistry()
        {
            var mockedContainer = new Mock<IContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.Register(It.IsAny<Factory>(), typeof(IRegionViewRegistry), null, It.IsAny<IfAlreadyRegistered?>(), It.IsAny<bool>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionBehaviorFactory()
        {
            var mockedContainer = new Mock<IContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.Register(It.IsAny<Factory>(), typeof(IRegionBehaviorFactory), null, It.IsAny<IfAlreadyRegistered?>(), It.IsAny<bool>()), Times.Once());
        }

        [StaFact]
        public void RunRegistersTypeForIEventAggregator()
        {
            var mockedContainer = new Mock<IContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

            bootstrapper.Run();

            mockedContainer.Verify(c => c.Register(It.IsAny<Factory>(), typeof(IEventAggregator), null, It.IsAny<IfAlreadyRegistered?>(), It.IsAny<bool>()), Times.Once());
        }

        [StaFact]
        public void RunFalseShouldNotRegisterDefaultServicesAndTypes()
        {
            var mockedContainer = new Mock<IContainer>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);
            bootstrapper.Run(false);

            mockedContainer.Verify(c => c.Register(It.IsAny<Factory>(), typeof(IEventAggregator), null, It.IsAny<IfAlreadyRegistered?>(), It.IsAny<bool>()), Times.Never());
            mockedContainer.Verify(c => c.Register(It.IsAny<Factory>(), typeof(IRegionManager), null, It.IsAny<IfAlreadyRegistered?>(), It.IsAny<bool>()), Times.Never());
            mockedContainer.Verify(c => c.Register(It.IsAny<Factory>(), typeof(RegionAdapterMappings), null, It.IsAny<IfAlreadyRegistered?>(), It.IsAny<bool>()), Times.Never());
            mockedContainer.Verify(c => c.Register(It.IsAny<Factory>(), typeof(IModuleInitializer), null, It.IsAny<IfAlreadyRegistered?>(), It.IsAny<bool>()), Times.Never());
        }

        private static void SetupMockedContainerForVerificationTests(Mock<IContainer> mockedContainer)
        {
            var mockedModuleInitializer = new Mock<IModuleInitializer>();
            var mockedModuleManager = new Mock<IModuleManager>();
            var regionAdapterMappings = new RegionAdapterMappings();

            var containerExtension = new DryIocContainerExtension(mockedContainer.Object);
            var regionBehaviorFactory = new RegionBehaviorFactory(containerExtension);

            mockedContainer.Setup(c => c.Register(It.IsAny<Factory>(), It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<IfAlreadyRegistered?>(), It.IsAny<bool>()));

            // NOTE: The actual method called by Prism's DryIocContainerExtension is off over the IResolver not IContainer
            mockedContainer.As<IResolver>().Setup(r => r.Resolve(typeof(IModuleCatalog), It.IsAny<object>(), IfUnresolved.Throw, It.IsAny<Type>(), It.IsAny<Request>(), It.IsAny<object[]>())).Returns(
                new ModuleCatalog());

            mockedContainer.As<IResolver>().Setup(c => c.Resolve(typeof(IModuleInitializer), It.IsAny<object>(), IfUnresolved.Throw, It.IsAny<Type>(), It.IsAny<Request>(), It.IsAny<object[]>())).Returns(
                mockedModuleInitializer.Object);

            mockedContainer.As<IResolver>().Setup(c => c.Resolve(typeof(IModuleManager), It.IsAny<object>(), IfUnresolved.Throw, It.IsAny<Type>(), It.IsAny<Request>(), It.IsAny<object[]>())).Returns(
                mockedModuleManager.Object);

            mockedContainer.As<IResolver>().Setup(c => c.Resolve(typeof(RegionAdapterMappings), It.IsAny<object>(), IfUnresolved.Throw, It.IsAny<Type>(), It.IsAny<Request>(), It.IsAny<object[]>())).Returns(
                regionAdapterMappings);

            mockedContainer.As<IResolver>().Setup(c => c.Resolve(typeof(SelectorRegionAdapter), It.IsAny<object>(), IfUnresolved.Throw, It.IsAny<Type>(), It.IsAny<Request>(), It.IsAny<object[]>())).Returns(
                new SelectorRegionAdapter(regionBehaviorFactory));

            mockedContainer.As<IResolver>().Setup(c => c.Resolve(typeof(ItemsControlRegionAdapter), It.IsAny<object>(), IfUnresolved.Throw, It.IsAny<Type>(), It.IsAny<Request>(), It.IsAny<object[]>())).Returns(
                new ItemsControlRegionAdapter(regionBehaviorFactory));

            mockedContainer.As<IResolver>().Setup(c => c.Resolve(typeof(ContentControlRegionAdapter), It.IsAny<object>(), IfUnresolved.Throw, It.IsAny<Type>(), It.IsAny<Request>(), It.IsAny<object[]>())).Returns(
                new ContentControlRegionAdapter(regionBehaviorFactory));
        }
    }
}
