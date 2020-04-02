

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using CommonServiceLocator;
using Xunit;
using Moq;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Mvvm;
using Prism.Wpf.Tests.Mocks.Views;
using Prism.Wpf.Tests.Mocks.ViewModels;
using Prism.Ioc;

namespace Prism.Wpf.Tests
{
    
    public class BootstrapperFixture
    {
        [Fact]
        public void LoggerDefaultsToNull()
        {
            var bootstrapper = new DefaultBootstrapper();

            Assert.Null(bootstrapper.BaseLogger);
        }

        [Fact]
        public void ModuleCatalogDefaultsToNull()
        {
            var bootstrapper = new DefaultBootstrapper();

            Assert.Null(bootstrapper.BaseModuleCatalog);
        }

        [Fact]
        public void ShellDefaultsToNull()
        {
            var bootstrapper = new DefaultBootstrapper();

            Assert.Null(bootstrapper.BaseShell);
        }

        [Fact]
        public void CreateLoggerInitializesLogger()
        {
            var bootstrapper = new DefaultBootstrapper();
            bootstrapper.CallCreateLogger();

            Assert.NotNull(bootstrapper.BaseLogger);

            Assert.IsType<TextLogger>(bootstrapper.BaseLogger);
        }

        [StaFact]
        public void ConfigureViewModelLocatorShouldUserServiceLocatorAsResolver()
        {
            var bootstrapper = new DefaultBootstrapper();

            CreateAndConfigureServiceLocatorForViewModelLocator();

            bootstrapper.CallConfigureViewModelLocator();

            var view = new MockView();

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
                {
                    Assert.NotNull(v);
                    Assert.NotNull(vm);
                    Assert.IsType<MockViewModel>(vm);
                });
        }

        private static void CreateAndConfigureServiceLocatorForViewModelLocator()
        {
            var serviceLocator = new Prism.Wpf.Tests.ServiceLocatorExtensionsFixture.MockServiceLocator(() => new MockViewModel());
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }

        [Fact]
        public void CreateModuleCatalogShouldInitializeModuleCatalog()
        {
            var bootstrapper = new DefaultBootstrapper();

            bootstrapper.CallCreateModuleCatalog();

            Assert.NotNull(bootstrapper.BaseModuleCatalog);
        }

        [Fact]
        public void RegisterFrameworkExceptionTypesShouldRegisterActivationException()
        {
            var bootstrapper = new DefaultBootstrapper();

            bootstrapper.CallRegisterFrameworkExceptionTypes();

            Assert.True(ExceptionExtensions.IsFrameworkExceptionRegistered(
                typeof(ActivationException)));
        }

        [Fact]
        public void ConfigureRegionAdapterMappingsShouldRegisterItemsControlMapping()
        {
            var bootstrapper = new DefaultBootstrapper();

            CreateAndConfigureServiceLocatorWithRegionAdapters();

            var regionAdapterMappings = bootstrapper.CallConfigureRegionAdapterMappings();

            Assert.NotNull(regionAdapterMappings);
            Assert.NotNull(regionAdapterMappings.GetMapping(typeof(ItemsControl)));
        }

        [Fact]
        public void ConfigureRegionAdapterMappingsShouldRegisterContentControlMapping()
        {
            var bootstrapper = new DefaultBootstrapper();

            CreateAndConfigureServiceLocatorWithRegionAdapters();

            var regionAdapterMappings = bootstrapper.CallConfigureRegionAdapterMappings();

            Assert.NotNull(regionAdapterMappings);
            Assert.NotNull(regionAdapterMappings.GetMapping(typeof(ContentControl)));
        }

        [Fact]
        public void ConfigureRegionAdapterMappingsShouldRegisterSelectorMapping()
        {
            var bootstrapper = new DefaultBootstrapper();

            CreateAndConfigureServiceLocatorWithRegionAdapters();

            var regionAdapterMappings = bootstrapper.CallConfigureRegionAdapterMappings();

            Assert.NotNull(regionAdapterMappings);
            Assert.NotNull(regionAdapterMappings.GetMapping(typeof(Selector)));
        }

        private static void CreateAndConfigureServiceLocatorWithRegionAdapters()
        {
            Mock<ServiceLocatorImplBase> serviceLocator = new Mock<ServiceLocatorImplBase>();
            var regionBehaviorFactory = new RegionBehaviorFactory(serviceLocator.Object);
            serviceLocator.Setup(sl => sl.GetInstance<RegionAdapterMappings>()).Returns(new RegionAdapterMappings());
            serviceLocator.Setup(sl => sl.GetInstance<SelectorRegionAdapter>()).Returns(new SelectorRegionAdapter(regionBehaviorFactory));
            serviceLocator.Setup(sl => sl.GetInstance<ItemsControlRegionAdapter>()).Returns(new ItemsControlRegionAdapter(regionBehaviorFactory));
            serviceLocator.Setup(sl => sl.GetInstance<ContentControlRegionAdapter>()).Returns(new ContentControlRegionAdapter(regionBehaviorFactory));

            ServiceLocator.SetLocatorProvider(() => serviceLocator.Object);
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddSevenDefaultBehaviors()
        {
            var bootstrapper = new DefaultBootstrapper();

            CreateAndConfigureServiceLocatorWithDefaultRegionBehaviors();

            bootstrapper.CallConfigureDefaultRegionBehaviors();

            Assert.Equal(8, bootstrapper.DefaultRegionBehaviorTypes.Count());
        }

        private static void CreateAndConfigureServiceLocatorWithDefaultRegionBehaviors()
        {
            Mock<ServiceLocatorImplBase> serviceLocator = new Mock<ServiceLocatorImplBase>();
            var regionBehaviorFactory = new RegionBehaviorFactory(serviceLocator.Object);
            serviceLocator.Setup(sl => sl.GetInstance<IRegionBehaviorFactory>()).Returns(new RegionBehaviorFactory(serviceLocator.Object));

            ServiceLocator.SetLocatorProvider(() => serviceLocator.Object);
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddAutoPopulateRegionBehavior()
        {
            var bootstrapper = new DefaultBootstrapper();

            CreateAndConfigureServiceLocatorWithDefaultRegionBehaviors();

            bootstrapper.CallConfigureDefaultRegionBehaviors();

            Assert.True(bootstrapper.DefaultRegionBehaviorTypes.ContainsKey(AutoPopulateRegionBehavior.BehaviorKey));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldBindRegionContextToDependencyObjectBehavior()
        {
            var bootstrapper = new DefaultBootstrapper();

            CreateAndConfigureServiceLocatorWithDefaultRegionBehaviors();

            bootstrapper.CallConfigureDefaultRegionBehaviors();

            Assert.True(bootstrapper.DefaultRegionBehaviorTypes.ContainsKey(BindRegionContextToDependencyObjectBehavior.BehaviorKey));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddRegionActiveAwareBehavior()
        {
            var bootstrapper = new DefaultBootstrapper();

            CreateAndConfigureServiceLocatorWithDefaultRegionBehaviors();

            bootstrapper.CallConfigureDefaultRegionBehaviors();

            Assert.True(bootstrapper.DefaultRegionBehaviorTypes.ContainsKey(RegionActiveAwareBehavior.BehaviorKey));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddSyncRegionContextWithHostBehavior()
        {
            var bootstrapper = new DefaultBootstrapper();

            CreateAndConfigureServiceLocatorWithDefaultRegionBehaviors();

            bootstrapper.CallConfigureDefaultRegionBehaviors();

            Assert.True(bootstrapper.DefaultRegionBehaviorTypes.ContainsKey(SyncRegionContextWithHostBehavior.BehaviorKey));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddRegionManagerRegistrationBehavior()
        {
            var bootstrapper = new DefaultBootstrapper();

            CreateAndConfigureServiceLocatorWithDefaultRegionBehaviors();

            bootstrapper.CallConfigureDefaultRegionBehaviors();

            Assert.True(bootstrapper.DefaultRegionBehaviorTypes.ContainsKey(RegionManagerRegistrationBehavior.BehaviorKey));
        }

        [Fact]
        public void ConfigureDefaultRegionBehaviorsShouldAddRegionLifetimeBehavior()
        {
            var bootstrapper = new DefaultBootstrapper();

            CreateAndConfigureServiceLocatorWithDefaultRegionBehaviors();

            bootstrapper.CallConfigureDefaultRegionBehaviors();

            Assert.True(bootstrapper.DefaultRegionBehaviorTypes.ContainsKey(RegionMemberLifetimeBehavior.BehaviorKey));
        }

        [Fact]
        public void OnInitializedShouldRunLast()
        {
            var bootstrapper = new DefaultBootstrapper();

            bootstrapper.Run();

            Assert.True(bootstrapper.ExtraInitialization);
        }
    }

    internal class DefaultBootstrapper : Bootstrapper
    {
        public IRegionBehaviorFactory DefaultRegionBehaviorTypes;
        public bool ExtraInitialization;

        public ILoggerFacade BaseLogger
        {
            get
            {
                return base.Logger;
            }
        }

        public IModuleCatalog BaseModuleCatalog
        {
            get { return base.ModuleCatalog; }
            set { base.ModuleCatalog = value; }
        }

        public DependencyObject BaseShell
        {
            get { return base.Shell; }
            set { base.Shell = value; }
        }

        public void CallCreateLogger()
        {
            this.Logger = base.CreateLogger();
        }

        public void CallCreateModuleCatalog()
        {
            this.ModuleCatalog = base.CreateModuleCatalog();
        }

        public RegionAdapterMappings CallConfigureRegionAdapterMappings()
        {
            return base.ConfigureRegionAdapterMappings();
        }

        public void CallConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
        }

        public override void Run(bool runWithDefaultConfiguration)
        {
            Assert.False(this.ExtraInitialization);
        }

        protected override void OnInitialized()
        {
            this.ExtraInitialization = true;
        }

        protected override DependencyObject CreateShell()
        {
            throw new NotImplementedException();
        }

        protected override void InitializeShell()
        {
            throw new NotImplementedException();
        }

        protected override void ConfigureServiceLocator()
        {
            throw new NotImplementedException();
        }

        public void CallRegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();
        }

        public IRegionBehaviorFactory CallConfigureDefaultRegionBehaviors()
        {
            this.DefaultRegionBehaviorTypes = base.ConfigureDefaultRegionBehaviors();
            return this.DefaultRegionBehaviorTypes;
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            return null;
        }
    }

}
