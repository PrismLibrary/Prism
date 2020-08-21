using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xunit;
using Moq;
using Prism.Modularity;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Mvvm;
using Prism.Wpf.Tests.Mocks.Views;
using Prism.Wpf.Tests.Mocks.ViewModels;
using Prism.Ioc;
using Prism.Wpf.Tests.Mocks;
using System.Runtime.Serialization;

namespace Prism.Wpf.Tests
{

    public class BootstrapperFixture
    {
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

        private static void CreateAndConfigureServiceLocatorForViewModelLocator()
        {
            var container = new MockContainerAdapter();
            container.ResolvedInstances.Add(typeof(MockViewModel), new MockViewModel());
            ContainerLocator.ResetContainer();
            ContainerLocator.SetContainerExtension(() => container);
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
            var container = new Mock<IContainerExtension>();
            var regionBehaviorFactory = new RegionBehaviorFactory(container.Object);
            container.Setup(sl => sl.Resolve(typeof(RegionAdapterMappings))).Returns(new RegionAdapterMappings());
            container.Setup(sl => sl.Resolve(typeof(SelectorRegionAdapter))).Returns(new SelectorRegionAdapter(regionBehaviorFactory));
            container.Setup(sl => sl.Resolve(typeof(ItemsControlRegionAdapter))).Returns(new ItemsControlRegionAdapter(regionBehaviorFactory));
            container.Setup(sl => sl.Resolve(typeof(ContentControlRegionAdapter))).Returns(new ContentControlRegionAdapter(regionBehaviorFactory));

            ContainerLocator.ResetContainer();
            ContainerLocator.SetContainerExtension(() => container.Object);
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
            var containerExtension = new Mock<IContainerExtension>();
            var regionBehaviorFactory = new RegionBehaviorFactory(containerExtension.Object);
            containerExtension.Setup(sl => sl.Resolve(typeof(IRegionBehaviorFactory))).Returns(new RegionBehaviorFactory(containerExtension.Object));

            ContainerLocator.ResetContainer();
            ContainerLocator.SetContainerExtension(() => containerExtension.Object);
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

        public void CallRegisterFrameworkExceptionTypes()
        {
            RegisterFrameworkExceptionTypes();
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ActivationException));
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

    public class ActivationException : Exception
    {
        public ActivationException()
        {
        }
    }
}
