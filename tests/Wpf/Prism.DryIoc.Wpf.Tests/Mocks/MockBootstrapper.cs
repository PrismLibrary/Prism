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
            var containerExt = CreateContainerExtension();
            return ((IContainerExtension<IContainer>)containerExt).Instance;
        }

        protected override DependencyObject CreateShell()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            CreateShellCalled = true;
            return ShellObject;
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            RegisterRequiredTypesCalled = true;
            base.RegisterRequiredTypes(containerRegistry);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            RegisterTypesCalled = true;
        }

        protected override void Initialize()
        {
            ContainerLocator.ResetContainer();
            base.Initialize();
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            CreateContainerCalled = true;
            return base.CreateContainerExtension();
        }

        protected override void ConfigureViewModelLocator()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            ConfigureViewModelLocatorCalled = true;
            base.ConfigureViewModelLocator();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            CreateModuleCatalogCalled = true;
            return base.CreateModuleCatalog();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            ConfigureModuleCatalogCalled = true;
            base.ConfigureModuleCatalog(moduleCatalog);
        }

        protected override void InitializeShell(DependencyObject shell)
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            InitializeShellCalled = true;
            base.InitializeShell(shell);
        }

        protected override void OnInitialized()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            OnInitializeCalled = true;
            base.OnInitialized();
        }

        protected override void InitializeModules()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            InitializeModulesCalled = true;
            base.InitializeModules();
        }

        protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            ConfigureDefaultRegionBehaviorsCalled = true;
            base.ConfigureDefaultRegionBehaviors(regionBehaviors);
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            ConfigureRegionAdapterMappingsCalled = true;

            base.ConfigureRegionAdapterMappings(regionAdapterMappings);

            DefaultRegionAdapterMappings = regionAdapterMappings;
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            base.RegisterFrameworkExceptionTypes();
        }

        public void CallRegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();
        }
    }
}
