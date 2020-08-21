using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.IocContainer.Wpf.Tests.Support.Mocks;
using Prism.Modularity;
using Prism.Regions;

namespace Prism.Container.Wpf.Mocks
{
    internal class MockBootstrapper : DryIocBootstrapper
    {
        public List<string> MethodCalls = new List<string>();
        public bool InitializeModulesCalled;
        public bool ConfigureRegionAdapterMappingsCalled;
        public RegionAdapterMappings DefaultRegionAdapterMappings;
        public bool CreateModuleCatalogCalled;
        public bool ConfigureContainerCalled;
        public bool CreateShellCalled;
        public bool CreateContainerCalled;
        public bool ConfigureModuleCatalogCalled;
        public bool InitializeShellCalled;
        public bool ConfigureViewModelLocatorCalled;
        public bool ConfigureDefaultRegionBehaviorsCalled;
        public DependencyObject ShellObject = new UserControl();

        public DependencyObject BaseShell => base.Shell;

        public IContainer BaseContainer
        {
            get => base.Container;
            set => base.Container = value;
        }

        public override void Run(bool runWithDefaultConfiguration)
        {
            ContainerLocator.ResetContainer();
            base.Run(runWithDefaultConfiguration);
        }

        public new IContainerExtension ContainerExtension => base.ContainerExtension;

        public IContainerRegistry ContainerRegistry => base.ContainerExtension;

        public IContainer CallCreateContainer()
        {
            return this.CreateContainer();
        }

        protected override IContainer CreateContainer()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.CreateContainerCalled = true;
            return base.CreateContainer();
        }

        protected override void ConfigureContainer()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.ConfigureContainerCalled = true;
            base.ConfigureContainer();
        }

        protected override DependencyObject CreateShell()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.CreateShellCalled = true;
            return ShellObject;
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

        protected override void ConfigureModuleCatalog()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.ConfigureModuleCatalogCalled = true;
            base.ConfigureModuleCatalog();
        }

        protected override void InitializeShell()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.InitializeShellCalled = true;
            // no op
        }

        protected override void InitializeModules()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.InitializeModulesCalled = true;
            base.InitializeModules();
        }

        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.ConfigureDefaultRegionBehaviorsCalled = true;
            return base.ConfigureDefaultRegionBehaviors();
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            ConfigureRegionAdapterMappingsCalled = true;
            var regionAdapterMappings = base.ConfigureRegionAdapterMappings();

            DefaultRegionAdapterMappings = regionAdapterMappings;

            return regionAdapterMappings;
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

        public List<string> Messages { get; } = new List<string>();

        protected override void Log(string message)
        {
            Messages.Add(message);
        }
    }
}
