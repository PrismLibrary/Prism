using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.IocContainer.Wpf.Tests.Support;
using Prism.IocContainer.Wpf.Tests.Support.Mocks;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using CommonServiceLocator;
using Grace.DependencyInjection.Exceptions;
using Grace.DependencyInjection;

namespace Prism.Grace.Wpf.Tests
{
    [TestClass]
    public class GraceBootstrapperFixture: BootstrapperFixtureBase
    {
        [TestMethod]
        public void ContainerDefaultsToNull()
        {
            var bootstrapper = new DefaultGraceBootstrapper();
            var container = bootstrapper.BaseContainer;

            Assert.IsNull(container);
        }

        [TestMethod]
        public void CanCreateConcreteBootstrapper()
        {
            new DefaultGraceBootstrapper();
        }

        [TestMethod]
        public void CreateContainerShouldInitializeContainer()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            var container = bootstrapper.CallCreateContainer();

            Assert.IsNotNull(container);
            Assert.IsInstanceOfType(container, typeof(DependencyInjectionContainer));
        }

        [TestMethod]
        public void ConfigureContainerAddsModuleCatalogToContainer()
        {
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();

            var returnedCatalog = bootstrapper.BaseContainer.Locate<IModuleCatalog>();
            Assert.IsNotNull(returnedCatalog);
            Assert.IsTrue(returnedCatalog is ModuleCatalog);
        }

        [TestMethod]
        public void ConfigureContainerAddsLoggerFacadeToContainer()
        {
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();

            var returnedCatalog = bootstrapper.BaseContainer.Locate<ILoggerFacade>();
            Assert.IsNotNull(returnedCatalog);
        }

        [TestMethod]
        public void ConfigureContainerAddsRegionNavigationJournalEntryToContainer()
        {
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.Locate<IRegionNavigationJournalEntry>();
            var actual2 = bootstrapper.BaseContainer.Locate<IRegionNavigationJournalEntry>();

            Assert.IsNotNull(actual1);
            Assert.IsNotNull(actual2);
            Assert.AreNotSame(actual1, actual2);
        }

        [TestMethod]
        public void ConfigureContainerAddsRegionNavigationJournalToContainer()
        {
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.Locate<IRegionNavigationJournal>();
            var actual2 = bootstrapper.BaseContainer.Locate<IRegionNavigationJournal>();

            Assert.IsNotNull(actual1);
            Assert.IsNotNull(actual2);
            Assert.AreNotSame(actual1, actual2);
        }

        [TestMethod]
        public void ConfigureContainerAddsRegionNavigationServiceToContainer()
        {
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.Locate<IRegionNavigationService>();
            var actual2 = bootstrapper.BaseContainer.Locate<IRegionNavigationService>();

            Assert.IsNotNull(actual1);
            Assert.IsNotNull(actual2);
            Assert.AreNotSame(actual1, actual2);
        }

        [TestMethod]
        public void ConfigureContainerAddsNavigationTargetHandlerToContainer()
        {
            var bootstrapper = new DefaultGraceBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.BaseContainer.Locate<IRegionNavigationContentLoader>();
            var actual2 = bootstrapper.BaseContainer.Locate<IRegionNavigationContentLoader>();

            Assert.IsNotNull(actual1);
            Assert.IsNotNull(actual2);
            Assert.AreSame(actual1, actual2);
        }

        [TestMethod]
        public void RegisterFrameworkExceptionTypesShouldRegisterActivationException()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.CallRegisterFrameworkExceptionTypes();

            Assert.IsTrue(ExceptionExtensions.IsFrameworkExceptionRegistered(
                typeof(ActivationException)));
        }

        [TestMethod]
        public void RegisterFrameworkExceptionTypesShouldRegisterResolutionFailedException()
        {
            var bootstrapper = new DefaultGraceBootstrapper();

            bootstrapper.CallRegisterFrameworkExceptionTypes();

            Assert.IsTrue(ExceptionExtensions.IsFrameworkExceptionRegistered(
                typeof(LocateException)));
        }
    }

    internal class DefaultGraceBootstrapper : GraceBootstrapper
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
        public bool ConfigureViewModelLocatorCalled;
        public bool ConfigureDefaultRegionBehaviorsCalled;
        public DependencyObject ShellObject = new UserControl();

        public DependencyObject BaseShell
        {
            get { return base.Shell; }
        }
        public DependencyInjectionContainer BaseContainer
        {
            get { return base.Container; }
            set { base.Container = value; }
        }

        public MockLoggerAdapter BaseLogger
        { get { return base.Logger as MockLoggerAdapter; } }

        public DependencyInjectionContainer CallCreateContainer()
        {
            return this.CreateContainer();
        }

        protected override DependencyInjectionContainer CreateContainer()
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
    } 
}
