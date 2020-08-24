using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Prism.Container.Wpf.Mocks
{
    internal class MockBootstrapper : PrismBootstrapper
    {
        public List<string> MethodCalls = new List<string>();
        public bool InitializeModulesCalled;
        public bool ConfigureRegionAdapterMappingsCalled;
        public RegionAdapterMappings DefaultRegionAdapterMappings;
        public bool CreateModuleCatalogCalled;
        public bool RegisterRequiredTypesCalled;
        public bool RegisterTypesCalled;
        public bool CreateShellCalled;
        public bool CreateContainerCalled;
        public bool ConfigureModuleCatalogCalled;
        public bool InitializeShellCalled;
        public bool OnInitializeCalled;
        public bool ConfigureViewModelLocatorCalled;
        public bool ConfigureDefaultRegionBehaviorsCalled;
        public UserControl ShellObject = new UserControl();

        public DependencyObject BaseShell => base.Shell;

        public IContainer BaseContainer
        {
            get => base.Container?.GetContainer();
        }

        public IContainerExtension ContainerExtension => (IContainerExtension)base.Container;

        public IContainerRegistry ContainerRegistry => (IContainerRegistry)base.Container;

        public IContainer CallCreateContainer()
        {
            var containerExt = this.CreateContainerExtension();
            return ((IContainerExtension<IContainer>)containerExt).Instance;
        }

        protected override DependencyObject CreateShell()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.CreateShellCalled = true;
            return ShellObject;
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.RegisterRequiredTypesCalled = true;
            base.RegisterRequiredTypes(containerRegistry);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.RegisterTypesCalled = true;
        }

        protected override void Initialize()
        {
            ContainerLocator.ResetContainer();
            base.Initialize();
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.CreateContainerCalled = true;
            return base.CreateContainerExtension();
        }

        protected override void ConfigureViewModelLocator()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.ConfigureViewModelLocatorCalled = true;
            base.ConfigureViewModelLocator();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.CreateModuleCatalogCalled = true;
            return base.CreateModuleCatalog();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.ConfigureModuleCatalogCalled = true;
            base.ConfigureModuleCatalog(moduleCatalog);
        }

        protected override void InitializeShell(DependencyObject shell)
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.InitializeShellCalled = true;
            base.InitializeShell(shell);
        }

        protected override void OnInitialized()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.OnInitializeCalled = true;
            base.OnInitialized();
        }

        protected override void InitializeModules()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.InitializeModulesCalled = true;
            base.InitializeModules();
        }

        protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.ConfigureDefaultRegionBehaviorsCalled = true;
            base.ConfigureDefaultRegionBehaviors(regionBehaviors);
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            ConfigureRegionAdapterMappingsCalled = true;

            base.ConfigureRegionAdapterMappings(regionAdapterMappings);

            DefaultRegionAdapterMappings = regionAdapterMappings;
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            base.RegisterFrameworkExceptionTypes();
        }

        public void CallRegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();
        }
    }
}
