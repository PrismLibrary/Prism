using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Moq;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Services.Dialogs;
using Xunit;

namespace Prism.Wpf.Tests
{
    public class PrismBootstapperSetup : IDisposable
    {
        public PrismBootstrapper Bootstrapper { get; set; }

        public PrismBootstapperSetup()
        {
            ContainerLocator.ResetContainer();
            Bootstrapper = new PrismBootstrapper();
            Bootstrapper.Run();
        }

        public void Dispose()
        {
            ContainerLocator.ResetContainer();
        }
    }

    public class PrismBootstapperBaseFixture : IClassFixture<PrismBootstapperSetup>
    {
        PrismBootstrapper bootstrapper = null;

        public PrismBootstapperBaseFixture(PrismBootstapperSetup setup)
        {
            bootstrapper = setup.Bootstrapper;
        }

        [Fact]
        public void BootstrapperShouldCallConfigureViewModelLocator()
        {
            Assert.True(bootstrapper.ConfigureViewModelLocatorWasCalled);
        }

        [Fact]
        public void BootstrapperShouldCallInitialize()
        {
            Assert.True(bootstrapper.InitializeCalled);
        }

        [Fact]
        public void BootstrapperShouldCallCreateContainerExtension()
        {
            Assert.True(bootstrapper.CreateContainerExtensionCalled);
        }

        [Fact]
        public void BootstrapperShouldCallCreateModuleCatalog()
        {
            Assert.True(bootstrapper.CreateModuleCatalogCalled);
        }

        [Fact]
        public void BootstrapperShouldCallRegisterRequiredTypes()
        {
            Assert.True(bootstrapper.RegisterRequiredTypesCalled);
        }

        [Fact]
        public void BootstrapperShouldCallRegisterTypes()
        {
            Assert.True(bootstrapper.RegisterTypesWasCalled);
        }

        [Fact]
        public void BootstrapperShouldCallConfigureDefaultRegionBehaviors()
        {
            Assert.True(bootstrapper.ConfigureDefaultRegionBehaviorsCalled);
        }

        [Fact]
        public void BootstrapperShouldCallConfigureRegionAdapterMappings()
        {
            Assert.True(bootstrapper.ConfigureRegionAdapterMappingsCalled);
        }

        [Fact]
        public void BootstrapperShouldCallRegisterFrameworkExceptionTypes()
        {
            Assert.True(bootstrapper.RegisterFrameworkExceptionTypesCalled);
        }

        [Fact]
        public void BootstrapperShouldCallCreateShell()
        {
            Assert.True(bootstrapper.CreateShellWasCalled);
        }

        [Fact]
        public void BootstrapperShouldCallInitializeShell()
        {
            //in our mock Shell is null, so this INitializeShell should not be called by the bootstrapper
            Assert.False(bootstrapper.InitializeShellWasCalled);
        }

        [Fact]
        public void BootstrapperShouldCallOnInitialized()
        {
            Assert.True(bootstrapper.OnInitializedWasCalled);
        }

        [Fact]
        public void BootstrapperShouldCallConfigureModuleCatalog()
        {
            Assert.True(bootstrapper.ConfigureModuleCatalogCalled);
        }

        [Fact]
        public void BootstrapperShouldCallInitializeModules()
        {
            Assert.True(bootstrapper.InitializeModulesCalled);
        }

        [Fact]
        public void CreateModuleCatalogShouldReturnDefaultModuleCatalog()
        {
            Assert.NotNull(bootstrapper.DefaultModuleCatalog);
        }

        [Fact]
        public void ConfigureRegionAdapterMappingsShouldRegisterItemsControlMapping()
        {
            Assert.NotNull(bootstrapper.DefaultRegionAdapterMappings);
            Assert.NotNull(bootstrapper.DefaultRegionAdapterMappings.GetMapping(typeof(ItemsControl)));
        }

        [Fact]
        public void ConfigureRegionAdapterMappingsShouldRegisterSelectorMapping()
        {
            Assert.NotNull(bootstrapper.DefaultRegionAdapterMappings);
            Assert.NotNull(bootstrapper.DefaultRegionAdapterMappings.GetMapping(typeof(Selector)));
        }

        [Fact]
        public void ConfigureRegionAdapterMappingsShouldRegisterContentControlMapping()
        {
            Assert.NotNull(bootstrapper.DefaultRegionAdapterMappings);
            Assert.NotNull(bootstrapper.DefaultRegionAdapterMappings.GetMapping(typeof(ContentControl)));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddAutoPopulateRegionBehavior()
        {
            Assert.True(bootstrapper.DefaultRegionBehaviorTypes.ContainsKey(AutoPopulateRegionBehavior.BehaviorKey));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldBindRegionContextToDependencyObjectBehavior()
        {
            Assert.True(bootstrapper.DefaultRegionBehaviorTypes.ContainsKey(BindRegionContextToDependencyObjectBehavior.BehaviorKey));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddRegionActiveAwareBehavior()
        {
            Assert.True(bootstrapper.DefaultRegionBehaviorTypes.ContainsKey(RegionActiveAwareBehavior.BehaviorKey));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddSyncRegionContextWithHostBehavior()
        {
            Assert.True(bootstrapper.DefaultRegionBehaviorTypes.ContainsKey(SyncRegionContextWithHostBehavior.BehaviorKey));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddRegionManagerRegistrationBehavior()
        {
            Assert.True(bootstrapper.DefaultRegionBehaviorTypes.ContainsKey(RegionManagerRegistrationBehavior.BehaviorKey));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddRegionLifetimeBehavior()
        {
            Assert.True(bootstrapper.DefaultRegionBehaviorTypes.ContainsKey(RegionMemberLifetimeBehavior.BehaviorKey));
        }

        [Fact]
        public void RequiredTypesAreRegistered()
        {
            bootstrapper.MockContainer.Verify(x => x.RegisterInstance(typeof(IModuleCatalog), It.IsAny<IModuleCatalog>()), Times.Once);

            bootstrapper.MockContainer.Verify(x => x.RegisterSingleton(typeof(IDialogService), typeof(DialogService)), Times.Once);
            bootstrapper.MockContainer.Verify(x => x.RegisterSingleton(typeof(IModuleInitializer), typeof(ModuleInitializer)), Times.Once);
            bootstrapper.MockContainer.Verify(x => x.RegisterSingleton(typeof(IModuleManager), typeof(ModuleManager)), Times.Once);
            bootstrapper.MockContainer.Verify(x => x.RegisterSingleton(typeof(RegionAdapterMappings), typeof(RegionAdapterMappings)), Times.Once);
            bootstrapper.MockContainer.Verify(x => x.RegisterSingleton(typeof(IRegionManager), typeof(RegionManager)), Times.Once);
            bootstrapper.MockContainer.Verify(x => x.RegisterSingleton(typeof(IRegionNavigationContentLoader), typeof(RegionNavigationContentLoader)), Times.Once);
            bootstrapper.MockContainer.Verify(x => x.RegisterSingleton(typeof(IEventAggregator), typeof(EventAggregator)), Times.Once);
            bootstrapper.MockContainer.Verify(x => x.RegisterSingleton(typeof(IRegionViewRegistry), typeof(RegionViewRegistry)), Times.Once);
            bootstrapper.MockContainer.Verify(x => x.RegisterSingleton(typeof(IRegionBehaviorFactory), typeof(RegionBehaviorFactory)), Times.Once);

            bootstrapper.MockContainer.Verify(x => x.Register(typeof(IRegionNavigationJournalEntry), typeof(RegionNavigationJournalEntry)), Times.Once);
            bootstrapper.MockContainer.Verify(x => x.Register(typeof(IRegionNavigationJournal), typeof(RegionNavigationJournal)), Times.Once);
            bootstrapper.MockContainer.Verify(x => x.Register(typeof(IRegionNavigationService), typeof(RegionNavigationService)), Times.Once);
            bootstrapper.MockContainer.Verify(x => x.Register(typeof(IDialogWindow), typeof(DialogWindow)), Times.Once);
        }
    }

    public class PrismBootstrapper : PrismBootstrapperBase
    {
        public Mock<IContainerExtension> MockContainer { get; private set; }

        public IModuleCatalog DefaultModuleCatalog => Container.Resolve<IModuleCatalog>();

        public IRegionBehaviorFactory DefaultRegionBehaviorTypes => Container.Resolve<IRegionBehaviorFactory>();

        public RegionAdapterMappings DefaultRegionAdapterMappings => Container.Resolve<RegionAdapterMappings>();

        public bool ConfigureViewModelLocatorWasCalled { get; set; }
        public bool CreateShellWasCalled { get; set; }
        public bool InitializeShellWasCalled { get; set; }
        public bool OnInitializedWasCalled { get; set; }
        public bool RegisterTypesWasCalled { get; set; }
        public bool InitializeModulesCalled { get; internal set; }
        public bool ConfigureModuleCatalogCalled { get; internal set; }
        public bool RegisterFrameworkExceptionTypesCalled { get; internal set; }
        public bool ConfigureRegionAdapterMappingsCalled { get; internal set; }
        public bool ConfigureDefaultRegionBehaviorsCalled { get; internal set; }
        public bool RegisterRequiredTypesCalled { get; internal set; }
        public bool CreateModuleCatalogCalled { get; internal set; }
        public bool CreateContainerExtensionCalled { get; internal set; }
        public bool InitializeCalled { get; internal set; }

        protected override void Initialize()
        {
            InitializeCalled = true;

            ContainerLocator.ResetContainer();
            MockContainer = new Mock<IContainerExtension>();

            base.Initialize();
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            CreateContainerExtensionCalled = true;
            return MockContainer.Object;
        }

        protected override void ConfigureViewModelLocator()
        {
            ConfigureViewModelLocatorWasCalled = true;
            //setting this breaks other tests using VML. 
            //We need to revist those tests to ensure it is being reset each time.
            //base.ConfigureViewModelLocator();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            CreateModuleCatalogCalled = true;

            var moduleCatalog = base.CreateModuleCatalog();
            MockContainer.Setup(x => x.Resolve(typeof(IModuleCatalog))).Returns(moduleCatalog);
            return moduleCatalog;
        }

        protected override DependencyObject CreateShell()
        {
            CreateShellWasCalled = true;
            return null;
        }

        protected override void InitializeShell(DependencyObject shell)
        {
            InitializeShellWasCalled = false;
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            RegisterRequiredTypesCalled = true;

            base.RegisterRequiredTypes(containerRegistry);

            var moduleInitializer = new ModuleInitializer(MockContainer.Object);
            MockContainer.Setup(x => x.Resolve(typeof(IModuleInitializer))).Returns(moduleInitializer);
            MockContainer.Setup(x => x.Resolve(typeof(IModuleManager))).Returns(new ModuleManager(moduleInitializer, DefaultModuleCatalog));
            MockContainer.Setup(x => x.Resolve(typeof(IRegionBehaviorFactory))).Returns(new RegionBehaviorFactory(MockContainer.Object));

            var regionBehaviorFactory = new RegionBehaviorFactory(MockContainer.Object);
            MockContainer.Setup(x => x.Resolve(typeof(IRegionBehaviorFactory))).Returns(regionBehaviorFactory);

            MockContainer.Setup(x => x.Resolve(typeof(RegionAdapterMappings))).Returns(new RegionAdapterMappings());
            MockContainer.Setup(x => x.Resolve(typeof(SelectorRegionAdapter))).Returns(new SelectorRegionAdapter(regionBehaviorFactory));
            MockContainer.Setup(x => x.Resolve(typeof(ItemsControlRegionAdapter))).Returns(new ItemsControlRegionAdapter(regionBehaviorFactory));
            MockContainer.Setup(x => x.Resolve(typeof(ContentControlRegionAdapter))).Returns(new ContentControlRegionAdapter(regionBehaviorFactory));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterTypesWasCalled = true;
        }

        protected override void OnInitialized()
        {
            OnInitializedWasCalled = true;
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            ConfigureModuleCatalogCalled = true;
            base.ConfigureModuleCatalog(moduleCatalog);
        }

        protected override void InitializeModules()
        {
            InitializeModulesCalled = true;
            base.InitializeModules();
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            RegisterFrameworkExceptionTypesCalled = true;
            base.RegisterFrameworkExceptionTypes();
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            ConfigureRegionAdapterMappingsCalled = true;
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
        }

        protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
        {
            ConfigureDefaultRegionBehaviorsCalled = true;
            base.ConfigureDefaultRegionBehaviors(regionBehaviors);
        }
    }
}
