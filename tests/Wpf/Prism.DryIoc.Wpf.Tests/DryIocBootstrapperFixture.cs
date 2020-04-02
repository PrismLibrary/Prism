using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using Xunit;
using Prism.IocContainer.Wpf.Tests.Support;
using Prism.IocContainer.Wpf.Tests.Support.Mocks;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using DryIoc;
using Prism.DryIoc;

namespace Prism.DryIoc.Wpf.Tests
{
    [Collection("ServiceLocator")]
    public class DryIocBootstrapperFixture : BootstrapperFixtureBase
    {
        [StaFact]
        public void ContainerDefaultsToNull()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();
            var container = bootstrapper.BaseContainer;

            Assert.Null(container);
        }

        [StaFact]
        public void CanCreateConcreteBootstrapper()
        {
            new DefaultDryIocBootstrapper();
        }

        [StaFact]
        public void CreateContainerShouldInitializeContainer()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            IContainer container = bootstrapper.CallCreateContainer();

            Assert.NotNull(container);
            Assert.IsAssignableFrom<IContainer>(container);
        }

        [StaFact]
        public void ConfigureContainerAddsModuleCatalogToContainer()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();

            var returnedCatalog = bootstrapper.BaseContainer.Resolve<IModuleCatalog>();
            Assert.NotNull(returnedCatalog);
            Assert.True(returnedCatalog is ModuleCatalog);
        }

        [StaFact]
        public void ConfigureContainerAddsLoggerFacadeToContainer()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();

            var returnedCatalog = bootstrapper.BaseContainer.Resolve<ILoggerFacade>();
            Assert.NotNull(returnedCatalog);
        }

        [StaFact]
        public void ConfigureContainerAddsRegionNavigationJournalEntryToContainer()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.Resolve<IRegionNavigationJournalEntry>();
            var actual2 = bootstrapper.BaseContainer.Resolve<IRegionNavigationJournalEntry>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotSame(actual1, actual2);
        }

        [StaFact]
        public void ConfigureContainerAddsRegionNavigationJournalToContainer()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.Resolve<IRegionNavigationJournal>();
            var actual2 = bootstrapper.BaseContainer.Resolve<IRegionNavigationJournal>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotSame(actual1, actual2);
        }

        [StaFact]
        public void ConfigureContainerAddsRegionNavigationServiceToContainer()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.Resolve<IRegionNavigationService>();
            var actual2 = bootstrapper.BaseContainer.Resolve<IRegionNavigationService>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotSame(actual1, actual2);
        }

        [StaFact]
        public void ConfigureContainerAddsNavigationTargetHandlerToContainer()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.Resolve<IRegionNavigationContentLoader>();
            var actual2 = bootstrapper.BaseContainer.Resolve<IRegionNavigationContentLoader>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.Same(actual1, actual2);
        }

        [StaFact]
        public void RegisterFrameworkExceptionTypesShouldRegisterActivationException()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.CallRegisterFrameworkExceptionTypes();

            Assert.True(ExceptionExtensions.IsFrameworkExceptionRegistered(
                typeof(ContainerException)));
        }

        [StaFact]
        public void RegisterFrameworkExceptionTypesShouldRegisterResolutionFailedException()
        {
            var bootstrapper = new DefaultDryIocBootstrapper();

            bootstrapper.CallRegisterFrameworkExceptionTypes();

            Assert.True(ExceptionExtensions.IsFrameworkExceptionRegistered(
                typeof(ContainerException)));
        }
    }

    internal class DefaultDryIocBootstrapper : DryIocBootstrapper
    {
        public List<string> MethodCalls = new List<string>();
        public bool InitializeModulesCalled;
        public bool ConfigureRegionAdapterMappingsCalled;
        public RegionAdapterMappings DefaultRegionAdapterMappings;
        public bool CreateLoggerCalled;
        public bool CreateModuleCatalogCalled;
        public bool CreateShellCalled;
        public bool ConfigureContainerCalled;
        public bool CreateContainerCalled;
        public bool ConfigureModuleCatalogCalled;
        public bool InitializeShellCalled;
        public bool ConfigureServiceLocatorCalled;
        public bool ConfigureDefaultRegionBehaviorsCalled;
        public DependencyObject ShellObject = new UserControl();

        public DependencyObject BaseShell
        {
            get { return base.Shell; }
        }
        public IContainer BaseContainer
        {
            get { return base.Container; }
            set { base.Container = value; }
        }

        public MockLoggerAdapter BaseLogger
        {
            get { return base.Logger as MockLoggerAdapter; } 
        }

        public IContainer CallCreateContainer()
        {
            return CreateContainer();
        }

        protected override void ConfigureContainer()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            ConfigureContainerCalled = true;
            base.ConfigureContainer();
        }
        
        protected override IContainer CreateContainer()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            CreateContainerCalled = true;
            return base.CreateContainer();
        }

        protected override ILoggerFacade CreateLogger()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            CreateLoggerCalled = true;
            return new MockLoggerAdapter();
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

        protected override void InitializeShell()
        {
            MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            InitializeShellCalled = true;
            // no op
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
