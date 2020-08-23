using System;
using Moq;
using Prism.Container.Wpf.Mocks;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.DryIoc;
using Xunit;
using DryIoc;
using System.Windows.Documents;

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

        //TODO: I have no idea why this wouldn't work
        //[StaFact]
        //public void RunRegistersInstanceOfIModuleCatalog()
        //{
        //    var mockedContainer = new Mock<IContainer>();
        //    SetupMockedContainerForVerificationTests(mockedContainer);

        //    var bootstrapper = new MockedContainerBootstrapper(mockedContainer.Object);

        //    bootstrapper.Run();

        //    mockedContainer.Verify(c => c.UseInstance(typeof(IModuleCatalog), It.IsAny<object>(), IfAlreadyRegistered.Replace, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<object>()));
        //}

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

            mockedContainer.Setup(c => c.Resolve(typeof(IModuleCatalog), IfUnresolved.Throw)).Returns(
                new ModuleCatalog());

            mockedContainer.Setup(c => c.Resolve(typeof(IModuleInitializer), IfUnresolved.Throw)).Returns(
                mockedModuleInitializer.Object);

            mockedContainer.Setup(c => c.Resolve(typeof(IModuleManager), IfUnresolved.Throw)).Returns(
                mockedModuleManager.Object);

            mockedContainer.Setup(c => c.Resolve(typeof(RegionAdapterMappings), IfUnresolved.Throw)).Returns(
                regionAdapterMappings);

            mockedContainer.Setup(c => c.Resolve(typeof(SelectorRegionAdapter), IfUnresolved.Throw)).Returns(
                new SelectorRegionAdapter(regionBehaviorFactory));

            mockedContainer.Setup(c => c.Resolve(typeof(ItemsControlRegionAdapter), IfUnresolved.Throw)).Returns(
                new ItemsControlRegionAdapter(regionBehaviorFactory));

            mockedContainer.Setup(c => c.Resolve(typeof(ContentControlRegionAdapter), IfUnresolved.Throw)).Returns(
                new ContentControlRegionAdapter(regionBehaviorFactory));
        }
    }
}
