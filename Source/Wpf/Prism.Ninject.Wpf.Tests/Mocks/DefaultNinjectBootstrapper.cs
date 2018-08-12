using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Ninject;
using Prism.IocContainer.Wpf.Tests.Support.Mocks;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;

namespace Prism.Ninject.Wpf.Tests.Mocks
{
    internal class DefaultNinjectBootstrapper : NinjectBootstrapper
    {
        public bool ConfigureDefaultRegionBehaviorsCalled;
        public bool ConfigureKernelCalled;
        public bool ConfigureModuleCatalogCalled;
        public bool ConfigureRegionAdapterMappingsCalled;
        public bool ConfigureServiceLocatorCalled;
        public bool ConfigureViewModelLocatorCalled;
        public bool CreateKernelCalled;
        public bool CreateLoggerCalled;
        public bool CreateModuleCatalogCalled;
        public bool CreateShellCalled;
        public RegionAdapterMappings DefaultRegionAdapterMappings;
        public bool InitializeModulesCalled;
        public bool InitializeShellCalled;
        public List<string> MethodCalls = new List<string>();
        public DependencyObject ShellObject = new UserControl();

        public MockLoggerAdapter BaseLogger => Logger as MockLoggerAdapter;

        public DependencyObject BaseShell => Shell;

        protected override void InitializeShell()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            InitializeShellCalled = true;
        }

        protected override IKernel CreateKernel()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            CreateKernelCalled = true;
            return base.CreateKernel();
        }

        public IKernel CallCreateKernel()
        {
            return CreateKernel();
        }

        protected override void ConfigureKernel()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            ConfigureKernelCalled = true;
            base.ConfigureKernel();
        }

        protected override DependencyObject CreateShell()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            CreateShellCalled = true;
            return ShellObject;
        }

        protected override void ConfigureServiceLocator()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            ConfigureServiceLocatorCalled = true;
            base.ConfigureServiceLocator();
        }

        protected override void ConfigureViewModelLocator()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            ConfigureViewModelLocatorCalled = true;
            base.ConfigureViewModelLocator();
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

        protected override void InitializeModules()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            InitializeModulesCalled = true;
            base.InitializeModules();
        }

        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            ConfigureDefaultRegionBehaviorsCalled = true;
            return base.ConfigureDefaultRegionBehaviors();
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            ConfigureRegionAdapterMappingsCalled = true;
            var regionAdapterMappings = base.ConfigureRegionAdapterMappings();

            DefaultRegionAdapterMappings = regionAdapterMappings;

            return regionAdapterMappings;
        }

        protected override ILoggerFacade CreateLogger()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            CreateLoggerCalled = true;
            return new MockLoggerAdapter();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            CreateModuleCatalogCalled = true;
            return base.CreateModuleCatalog();
        }

        protected override void ConfigureModuleCatalog()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            ConfigureModuleCatalogCalled = true;
            base.ConfigureModuleCatalog();
        }
    }
}