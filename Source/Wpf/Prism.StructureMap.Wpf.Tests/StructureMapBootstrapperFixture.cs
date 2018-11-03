using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Xunit;
using Prism.IocContainer.Wpf.Tests.Support;
using Prism.IocContainer.Wpf.Tests.Support.Mocks;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using StructureMap;
using CommonServiceLocator;

namespace Prism.StructureMap.Wpf.Tests
{
    [Collection("ServiceLocator")]
    public class StructureMapBootstrapperFixture: BootstrapperFixtureBase
    {
        [StaFact]
        public void ContainerDefaultsToNull()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();
            var container = bootstrapper.BaseContainer;

            Assert.Null(container);
        }

        [StaFact]
        public void CanCreateConcreteBootstrapper()
        {
            new DefaultStructureMapBootstrapper();
        }

        [StaFact]
        public void CreateContainerShouldInitializeContainer()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            IContainer container = bootstrapper.CallCreateContainer();

            Assert.NotNull(container);
            Assert.IsAssignableFrom<IContainer>(container);
        }

        [StaFact]
        public void ConfigureContainerAddsModuleCatalogToContainer()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();

            var returnedCatalog = bootstrapper.BaseContainer.GetInstance<IModuleCatalog>();
            Assert.NotNull(returnedCatalog);
            Assert.True(returnedCatalog is ModuleCatalog);
        }

        [StaFact]
        public void ConfigureContainerAddsLoggerFacadeToContainer()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();

            var returnedCatalog = bootstrapper.BaseContainer.GetInstance<ILoggerFacade>();
            Assert.NotNull(returnedCatalog);
        }

        [StaFact]
        public void ConfigureContainerAddsRegionNavigationJournalEntryToContainer()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.GetInstance<IRegionNavigationJournalEntry>();
            var actual2 = bootstrapper.BaseContainer.GetInstance<IRegionNavigationJournalEntry>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotSame(actual1, actual2);
        }

        [StaFact]
        public void ConfigureContainerAddsRegionNavigationJournalToContainer()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.GetInstance<IRegionNavigationJournal>();
            var actual2 = bootstrapper.BaseContainer.GetInstance<IRegionNavigationJournal>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotSame(actual1, actual2);
        }

        [StaFact]
        public void ConfigureContainerAddsRegionNavigationServiceToContainer()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.GetInstance<IRegionNavigationService>();
            var actual2 = bootstrapper.BaseContainer.GetInstance<IRegionNavigationService>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotSame(actual1, actual2);
        }

        [StaFact]
        public void ConfigureContainerAddsNavigationTargetHandlerToContainer()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.GetInstance<IRegionNavigationContentLoader>();
            var actual2 = bootstrapper.BaseContainer.GetInstance<IRegionNavigationContentLoader>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.Same(actual1, actual2);
        }

        [StaFact]
        public void RegisterFrameworkExceptionTypesShouldRegisterActivationException()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.CallRegisterFrameworkExceptionTypes();

            Assert.True(ExceptionExtensions.IsFrameworkExceptionRegistered(
                typeof(ActivationException)));
        }

        [StaFact]
        public void RegisterFrameworkExceptionTypesShouldRegisterResolutionFailedException()
        {
            var bootstrapper = new DefaultStructureMapBootstrapper();

            bootstrapper.CallRegisterFrameworkExceptionTypes();

            Assert.True(ExceptionExtensions.IsFrameworkExceptionRegistered(
                typeof(StructureMapBuildPlanException)));
            Assert.True(ExceptionExtensions.IsFrameworkExceptionRegistered(
                typeof(StructureMapConfigurationException)));
            Assert.True(ExceptionExtensions.IsFrameworkExceptionRegistered(
                typeof(StructureMapException)));
        }
    }

    internal class DefaultStructureMapBootstrapper : StructureMapBootstrapper
    {
        public List<string> MethodCalls = new List<string>();
        public bool InitializeModulesCalled;
        public bool ConfigureRegionAdapterMappingsCalled;
        public RegionAdapterMappings DefaultRegionAdapterMappings;
        public bool CreateLoggerCalled;
        public bool CreateModuleCatalogCalled;
        public bool ConfigureContainerCalled;
        public bool CreateShellCalled;
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
        { get { return base.Logger as MockLoggerAdapter; } }

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

        protected override ILoggerFacade CreateLogger()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.CreateLoggerCalled = true;
            return new MockLoggerAdapter();
        }

        protected override DependencyObject CreateShell()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.CreateShellCalled = true;
            return ShellObject;
        }

        protected override void ConfigureServiceLocator()
        {
            this.MethodCalls.Add(MethodBase.GetCurrentMethod().Name);
            this.ConfigureServiceLocatorCalled = true;
            base.ConfigureServiceLocator();
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
    }
    
}
