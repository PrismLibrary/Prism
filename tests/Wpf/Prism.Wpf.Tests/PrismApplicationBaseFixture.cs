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
    public class PrismApplicationSetup : IDisposable
    {
        public PrismApplication Application { get; set; }

        public PrismApplicationSetup()
        {
            ContainerLocator.ResetContainer();
            Application = new PrismApplication();
            Application.CallOnStartup();
        }

        public void Dispose()
        {
            ContainerLocator.ResetContainer();
            Application.Shutdown();
        }
    }

    public class PrismApplicationBaseFixture : IClassFixture<PrismApplicationSetup>
    {
        PrismApplication application = null;

        public PrismApplicationBaseFixture(PrismApplicationSetup setup)
        {
            application = setup.Application;
        }

        [Fact]
        public void applicationShouldCallConfigureViewModelLocator()
        {
            Assert.True(application.ConfigureViewModelLocatorWasCalled);
        }

        [Fact]
        public void applicationShouldCallInitialize()
        {
            Assert.True(application.InitializeCalled);
        }

        [Fact]
        public void applicationShouldCallCreateContainerExtension()
        {
            Assert.True(application.CreateContainerExtensionCalled);
        }

        [Fact]
        public void applicationShouldCallCreateModuleCatalog()
        {
            Assert.True(application.CreateModuleCatalogCalled);
        }

        [Fact]
        public void applicationShouldCallRegisterRequiredTypes()
        {
            Assert.True(application.RegisterRequiredTypesCalled);
        }

        [Fact]
        public void applicationShouldCallRegisterTypes()
        {
            Assert.True(application.RegisterTypesWasCalled);
        }

        [Fact]
        public void applicationShouldCallConfigureDefaultRegionBehaviors()
        {
            Assert.True(application.ConfigureDefaultRegionBehaviorsCalled);
        }

        [Fact]
        public void applicationShouldCallConfigureRegionAdapterMappings()
        {
            Assert.True(application.ConfigureRegionAdapterMappingsCalled);
        }

        [Fact]
        public void applicationShouldCallRegisterFrameworkExceptionTypes()
        {
            Assert.True(application.RegisterFrameworkExceptionTypesCalled);
        }

        [Fact]
        public void applicationShouldCallCreateShell()
        {
            Assert.True(application.CreateShellWasCalled);
        }

        [Fact]
        public void applicationShouldCallInitializeShell()
        {
            //in our mock Shell is null, so this INitializeShell should not be called by the application
            Assert.False(application.InitializeShellWasCalled);
        }

        [Fact]
        public void applicationShouldCallOnInitialized()
        {
            Assert.True(application.OnInitializedWasCalled);
        }

        [Fact]
        public void applicationShouldCallConfigureModuleCatalog()
        {
            Assert.True(application.ConfigureModuleCatalogCalled);
        }

        [Fact]
        public void applicationShouldCallInitializeModules()
        {
            Assert.True(application.InitializeModulesCalled);
        }

        [Fact]
        public void CreateModuleCatalogShouldReturnDefaultModuleCatalog()
        {
            Assert.NotNull(application.DefaultModuleCatalog);
        }

        [Fact]
        public void ConfigureRegionAdapterMappingsShouldRegisterItemsControlMapping()
        {
            Assert.NotNull(application.DefaultRegionAdapterMappings);
            Assert.NotNull(application.DefaultRegionAdapterMappings.GetMapping(typeof(ItemsControl)));
        }

        [Fact]
        public void ConfigureRegionAdapterMappingsShouldRegisterSelectorMapping()
        {
            Assert.NotNull(application.DefaultRegionAdapterMappings);
            Assert.NotNull(application.DefaultRegionAdapterMappings.GetMapping(typeof(Selector)));
        }

        [Fact]
        public void ConfigureRegionAdapterMappingsShouldRegisterContentControlMapping()
        {
            Assert.NotNull(application.DefaultRegionAdapterMappings);
            Assert.NotNull(application.DefaultRegionAdapterMappings.GetMapping(typeof(ContentControl)));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddAutoPopulateRegionBehavior()
        {
            Assert.True(application.DefaultRegionBehaviorTypes.ContainsKey(AutoPopulateRegionBehavior.BehaviorKey));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldBindRegionContextToDependencyObjectBehavior()
        {
            Assert.True(application.DefaultRegionBehaviorTypes.ContainsKey(BindRegionContextToDependencyObjectBehavior.BehaviorKey));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddRegionActiveAwareBehavior()
        {
            Assert.True(application.DefaultRegionBehaviorTypes.ContainsKey(RegionActiveAwareBehavior.BehaviorKey));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddSyncRegionContextWithHostBehavior()
        {
            Assert.True(application.DefaultRegionBehaviorTypes.ContainsKey(SyncRegionContextWithHostBehavior.BehaviorKey));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddRegionManagerRegistrationBehavior()
        {
            Assert.True(application.DefaultRegionBehaviorTypes.ContainsKey(RegionManagerRegistrationBehavior.BehaviorKey));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddRegionLifetimeBehavior()
        {
            Assert.True(application.DefaultRegionBehaviorTypes.ContainsKey(RegionMemberLifetimeBehavior.BehaviorKey));
        }

        [Fact]
        public void RequiredTypesAreRegistered()
        {
            application.MockContainer.Verify(x => x.RegisterInstance(typeof(IModuleCatalog), It.IsAny<IModuleCatalog>()), Times.Once);

            application.MockContainer.Verify(x => x.RegisterSingleton(typeof(IDialogService), typeof(DialogService)), Times.Once);
            application.MockContainer.Verify(x => x.RegisterSingleton(typeof(IModuleInitializer), typeof(ModuleInitializer)), Times.Once);
            application.MockContainer.Verify(x => x.RegisterSingleton(typeof(IModuleManager), typeof(ModuleManager)), Times.Once);
            application.MockContainer.Verify(x => x.RegisterSingleton(typeof(RegionAdapterMappings), typeof(RegionAdapterMappings)), Times.Once);
            application.MockContainer.Verify(x => x.RegisterSingleton(typeof(IRegionManager), typeof(RegionManager)), Times.Once);
            application.MockContainer.Verify(x => x.RegisterSingleton(typeof(IRegionNavigationContentLoader), typeof(RegionNavigationContentLoader)), Times.Once);
            application.MockContainer.Verify(x => x.RegisterSingleton(typeof(IEventAggregator), typeof(EventAggregator)), Times.Once);
            application.MockContainer.Verify(x => x.RegisterSingleton(typeof(IRegionViewRegistry), typeof(RegionViewRegistry)), Times.Once);
            application.MockContainer.Verify(x => x.RegisterSingleton(typeof(IRegionBehaviorFactory), typeof(RegionBehaviorFactory)), Times.Once);

            application.MockContainer.Verify(x => x.Register(typeof(IRegionNavigationJournalEntry), typeof(RegionNavigationJournalEntry)), Times.Once);
            application.MockContainer.Verify(x => x.Register(typeof(IRegionNavigationJournal), typeof(RegionNavigationJournal)), Times.Once);
            application.MockContainer.Verify(x => x.Register(typeof(IRegionNavigationService), typeof(RegionNavigationService)), Times.Once);
            application.MockContainer.Verify(x => x.Register(typeof(IDialogWindow), typeof(DialogWindow)), Times.Once);
        }
    }

    public class PrismApplication : PrismApplicationBase
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

        public void CallOnStartup()
        {
            base.OnStartup(null);
        }

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

        protected override Window CreateShell()
        {
            CreateShellWasCalled = true;
            return null;
        }

        protected override void InitializeShell(Window shell)
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
