using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Modularity
{
    [TestClass]
    public class ModuleManagerFixture
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullLoaderThrows()
        {
            new ModuleManager(null, new MockModuleCatalog(), new MockLogger());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullCatalogThrows()
        {
            new ModuleManager(new MockModuleInitializer(), null, new MockLogger());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullLoggerThrows()
        {
            new ModuleManager(new MockModuleInitializer(), new MockModuleCatalog(), null);
        }       

        [TestMethod]
        public void ShouldInvokeRetrieverForModules()
        {
            var loader = new MockModuleInitializer();
            var moduleInfo = CreateModuleInfo("needsRetrieval", InitializationMode.WhenAvailable);
            var catalog = new MockModuleCatalog { Modules = { moduleInfo } };
            ModuleManager manager = new ModuleManager(loader, catalog, new MockLogger());
            var moduleTypeLoader = new MockModuleTypeLoader();
            manager.ModuleTypeLoaders = new List<IModuleTypeLoader> { moduleTypeLoader };

            manager.Run();

            Assert.IsTrue(moduleTypeLoader.LoadedModules.Contains(moduleInfo));
        }

        [TestMethod]
        public void ShouldInitializeModulesOnRetrievalCompleted()
        {
            var loader = new MockModuleInitializer();
            var backgroungModuleInfo = CreateModuleInfo("NeedsRetrieval", InitializationMode.WhenAvailable);
            var catalog = new MockModuleCatalog { Modules = { backgroungModuleInfo } };
            ModuleManager manager = new ModuleManager(loader, catalog, new MockLogger());
            var moduleTypeLoader = new MockModuleTypeLoader();
            manager.ModuleTypeLoaders = new List<IModuleTypeLoader> { moduleTypeLoader };            
            Assert.IsFalse(loader.InitializeCalled);

            manager.Run();

            Assert.IsTrue(loader.InitializeCalled);
            Assert.AreEqual(1, loader.InitializedModules.Count);
            Assert.AreEqual(backgroungModuleInfo, loader.InitializedModules[0]);
        }

        [TestMethod]
        public void ShouldInitializeModuleOnDemand()
        {
            var loader = new MockModuleInitializer();
            var onDemandModule = CreateModuleInfo("NeedsRetrieval", InitializationMode.OnDemand);
            var catalog = new MockModuleCatalog { Modules = { onDemandModule } };
            ModuleManager manager = new ModuleManager(loader, catalog, new MockLogger());
            var moduleRetriever = new MockModuleTypeLoader();
            manager.ModuleTypeLoaders = new List<IModuleTypeLoader> { moduleRetriever };
            manager.Run();

            Assert.IsFalse(loader.InitializeCalled);
            Assert.AreEqual(0, moduleRetriever.LoadedModules.Count);

            manager.LoadModule("NeedsRetrieval");

            Assert.AreEqual(1, moduleRetriever.LoadedModules.Count);
            Assert.IsTrue(loader.InitializeCalled);
            Assert.AreEqual(1, loader.InitializedModules.Count);
            Assert.AreEqual(onDemandModule, loader.InitializedModules[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ModuleNotFoundException))]
        public void InvalidOnDemandModuleNameThrows()
        {
            var loader = new MockModuleInitializer();

            var catalog = new MockModuleCatalog { Modules = new List<IModuleInfo> { CreateModuleInfo("Missing", InitializationMode.OnDemand) } };

            ModuleManager manager = new ModuleManager(loader, catalog, new MockLogger());
            var moduleTypeLoader = new MockModuleTypeLoader();

            manager.ModuleTypeLoaders = new List<IModuleTypeLoader> { moduleTypeLoader };
            manager.Run();

            manager.LoadModule("NonExistent");
        }

        [TestMethod]
        [ExpectedException(typeof(ModuleNotFoundException))]
        public void EmptyOnDemandModuleReturnedThrows()
        {
            var loader = new MockModuleInitializer();

            var catalog = new MockModuleCatalog { CompleteListWithDependencies = modules => new List<ModuleInfo>() };
            ModuleManager manager = new ModuleManager(loader, catalog, new MockLogger());
            var moduleRetriever = new MockModuleTypeLoader();
            manager.ModuleTypeLoaders = new List<IModuleTypeLoader> { moduleRetriever };
            manager.Run();

            manager.LoadModule("NullModule");
        }

        [TestMethod]
        public void ShouldNotLoadTypeIfModuleInitialized()
        {
            var loader = new MockModuleInitializer();
            var alreadyPresentModule = CreateModuleInfo(typeof(MockModule), InitializationMode.WhenAvailable);
            alreadyPresentModule.State = ModuleState.ReadyForInitialization;
            var catalog = new MockModuleCatalog { Modules = { alreadyPresentModule } };
            var manager = new ModuleManager(loader, catalog, new MockLogger());
            var moduleTypeLoader = new MockModuleTypeLoader();
            manager.ModuleTypeLoaders = new List<IModuleTypeLoader> { moduleTypeLoader };

            manager.Run();

            Assert.IsFalse(moduleTypeLoader.LoadedModules.Contains(alreadyPresentModule));
            Assert.IsTrue(loader.InitializeCalled);
            Assert.AreEqual(1, loader.InitializedModules.Count);
            Assert.AreEqual(alreadyPresentModule, loader.InitializedModules[0]);
        }

        [TestMethod]
        public void ShouldNotLoadSameModuleTwice()
        {
            var loader = new MockModuleInitializer();
            var onDemandModule = CreateModuleInfo(typeof(MockModule), InitializationMode.OnDemand);
            var catalog = new MockModuleCatalog { Modules = { onDemandModule } };
            var manager = new ModuleManager(loader, catalog, new MockLogger());
            manager.Run();
            manager.LoadModule("MockModule");
            loader.InitializeCalled = false;
            manager.LoadModule("MockModule");

            Assert.IsFalse(loader.InitializeCalled);
        }

        [TestMethod]
        public void ShouldNotLoadModuleThatNeedsRetrievalTwice()
        {
            var loader = new MockModuleInitializer();
            var onDemandModule = CreateModuleInfo("ModuleThatNeedsRetrieval", InitializationMode.OnDemand);
            var catalog = new MockModuleCatalog { Modules = { onDemandModule } };
            var manager = new ModuleManager(loader, catalog, new MockLogger());
            var moduleTypeLoader = new MockModuleTypeLoader();
            manager.ModuleTypeLoaders = new List<IModuleTypeLoader> { moduleTypeLoader };
            manager.Run();
            manager.LoadModule("ModuleThatNeedsRetrieval");
            moduleTypeLoader.RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(onDemandModule, null));
            loader.InitializeCalled = false;

            manager.LoadModule("ModuleThatNeedsRetrieval");

            Assert.IsFalse(loader.InitializeCalled);
        }

        [TestMethod]
        public void ShouldCallValidateCatalogBeforeGettingGroupsFromCatalog()
        {
            var loader = new MockModuleInitializer();
            var catalog = new MockModuleCatalog();
            var manager = new ModuleManager(loader, catalog, new MockLogger());
            bool validateCatalogCalled = false;
            bool getModulesCalledBeforeValidate = false;

            catalog.ValidateCatalog = () => validateCatalogCalled = true;
            catalog.CompleteListWithDependencies = f =>
                                                     {
                                                         if (!validateCatalogCalled)
                                                         {
                                                             getModulesCalledBeforeValidate = true;
                                                         }

                                                         return null;
                                                     };
            manager.Run();

            Assert.IsTrue(validateCatalogCalled);
            Assert.IsFalse(getModulesCalledBeforeValidate);
        }

        [TestMethod]
        public void ShouldNotInitializeIfDependenciesAreNotMet()
        {
            var loader = new MockModuleInitializer();
            var requiredModule = CreateModuleInfo("ModuleThatNeedsRetrieval1", InitializationMode.WhenAvailable);
            requiredModule.ModuleName = "RequiredModule";
            var dependantModuleInfo = CreateModuleInfo("ModuleThatNeedsRetrieval2", InitializationMode.WhenAvailable, "RequiredModule");

            var catalog = new MockModuleCatalog { Modules = { requiredModule, dependantModuleInfo } };
            catalog.GetDependentModules = m => new[] { requiredModule };

            ModuleManager manager = new ModuleManager(loader, catalog, new MockLogger());
            var moduleTypeLoader = new MockModuleTypeLoader();
            manager.ModuleTypeLoaders = new List<IModuleTypeLoader> { moduleTypeLoader };

            manager.Run();

            moduleTypeLoader.RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(dependantModuleInfo, null));            

            Assert.IsFalse(loader.InitializeCalled);
            Assert.AreEqual(0, loader.InitializedModules.Count);
        }

        [TestMethod]
        public void ShouldInitializeIfDependenciesAreMet()
        {
            var initializer = new MockModuleInitializer();
            var requiredModule = CreateModuleInfo("ModuleThatNeedsRetrieval1", InitializationMode.WhenAvailable);
            requiredModule.ModuleName = "RequiredModule";
            var dependantModuleInfo = CreateModuleInfo("ModuleThatNeedsRetrieval2", InitializationMode.WhenAvailable, "RequiredModule");

            var catalog = new MockModuleCatalog { Modules = { requiredModule, dependantModuleInfo } };
            catalog.GetDependentModules = delegate(IModuleInfo module)
                                              {
                                                  if (module == dependantModuleInfo)
                                                      return new[] { requiredModule };
                                                  else
                                                      return null;
                                              };

            ModuleManager manager = new ModuleManager(initializer, catalog, new MockLogger());
            var moduleTypeLoader = new MockModuleTypeLoader();
            manager.ModuleTypeLoaders = new List<IModuleTypeLoader> { moduleTypeLoader };

            manager.Run();

            Assert.IsTrue(initializer.InitializeCalled);
            Assert.AreEqual(2, initializer.InitializedModules.Count);
        }

        [TestMethod]
        public void ShouldThrowOnRetrieverErrorAndWrapException()
        {
            var loader = new MockModuleInitializer();
            var moduleInfo = CreateModuleInfo("NeedsRetrieval", InitializationMode.WhenAvailable);
            var catalog = new MockModuleCatalog { Modules = { moduleInfo } };
            ModuleManager manager = new ModuleManager(loader, catalog, new MockLogger());
            var moduleTypeLoader = new MockModuleTypeLoader();

            Exception retrieverException = new Exception();
            moduleTypeLoader.LoadCompletedError = retrieverException;

            manager.ModuleTypeLoaders = new List<IModuleTypeLoader> { moduleTypeLoader };            
            Assert.IsFalse(loader.InitializeCalled);

            try
            {
                manager.Run();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ModuleTypeLoadingException));
                Assert.AreEqual(moduleInfo.ModuleName, ((ModularityException)ex).ModuleName);
                StringAssert.Contains(ex.Message, moduleInfo.ModuleName);
                Assert.AreSame(retrieverException, ex.InnerException);
                return;
            }

            Assert.Fail("Exception not thrown.");
        }

        [TestMethod]
        [ExpectedException(typeof(ModuleTypeLoaderNotFoundException))]
        public void ShouldThrowIfNoRetrieverCanRetrieveModule()
        {
            var loader = new MockModuleInitializer();
            var catalog = new MockModuleCatalog { Modules = { CreateModuleInfo("ModuleThatNeedsRetrieval", InitializationMode.WhenAvailable) } };
            ModuleManager manager = new ModuleManager(loader, catalog, new MockLogger())
            {
                ModuleTypeLoaders = new List<IModuleTypeLoader> { new MockModuleTypeLoader() { canLoadModuleTypeReturnValue = false } }
            };
            manager.Run();
        }

        [TestMethod]
        public void ShouldLogMessageOnModuleRetrievalError()
        {
            var loader = new MockModuleInitializer();
            var moduleInfo = CreateModuleInfo("ModuleThatNeedsRetrieval", InitializationMode.WhenAvailable);
            var catalog = new MockModuleCatalog { Modules = { moduleInfo } };
            var logger = new MockLogger();
            ModuleManager manager = new ModuleManager(loader, catalog, logger);
            var moduleTypeLoader = new MockModuleTypeLoader
            {
                LoadCompletedError = new Exception()
            };
            manager.ModuleTypeLoaders = new List<IModuleTypeLoader> { moduleTypeLoader };

            try
            {
                manager.Run();
            }
            catch
            {
                // Ignore all errors to make sure logger is called even if errors thrown.
            }

            Assert.IsNotNull(logger.LastMessage);
            StringAssert.Contains(logger.LastMessage, "ModuleThatNeedsRetrieval");
            Assert.AreEqual<Category>(Category.Exception, logger.LastMessageCategory);
        }

        [TestMethod]
        public void ShouldWorkIfModuleLoadsAnotherOnDemandModuleWhenInitializing()
        {
            var initializer = new StubModuleInitializer();
            var onDemandModule = CreateModuleInfo(typeof(MockModule), InitializationMode.OnDemand);
            onDemandModule.ModuleName = "OnDemandModule";
            var moduleThatLoadsOtherModule = CreateModuleInfo(typeof(MockModule), InitializationMode.WhenAvailable);
            var catalog = new MockModuleCatalog { Modules = { moduleThatLoadsOtherModule, onDemandModule } };
            ModuleManager manager = new ModuleManager(initializer, catalog, new MockLogger());
            
            bool onDemandModuleWasInitialized = false;
            initializer.Initialize = m =>
                                     {
                                         if (m == moduleThatLoadsOtherModule)
                                         {
                                             manager.LoadModule("OnDemandModule");
                                         }
                                         else if (m == onDemandModule)
                                         {
                                             onDemandModuleWasInitialized = true;
                                         }
                                     };

            manager.Run();

            Assert.IsTrue(onDemandModuleWasInitialized);
        }

        
        [TestMethod]
        public void ModuleManagerIsDisposable()
        {
            Mock<IModuleInitializer> mockInit = new Mock<IModuleInitializer>(); 
            var moduleInfo = CreateModuleInfo("needsRetrieval", InitializationMode.WhenAvailable);
            var catalog = new Mock<IModuleCatalog>();
            ModuleManager manager = new ModuleManager(mockInit.Object, catalog.Object, new MockLogger());

            IDisposable disposableManager = manager as IDisposable;
            Assert.IsNotNull(disposableManager);
        }
        
        [TestMethod]
        public void DisposeDoesNotThrowWithNonDisposableTypeLoaders()
        {
            Mock<IModuleInitializer> mockInit = new Mock<IModuleInitializer>();
            var moduleInfo = CreateModuleInfo("needsRetrieval", InitializationMode.WhenAvailable);
            var catalog = new Mock<IModuleCatalog>();
            ModuleManager manager = new ModuleManager(mockInit.Object, catalog.Object, new MockLogger());

            var mockTypeLoader = new Mock<IModuleTypeLoader>();
            manager.ModuleTypeLoaders = new List<IModuleTypeLoader> {mockTypeLoader.Object};

            try
            {
                manager.Dispose();
            }
            catch(Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void DisposeCleansUpDisposableTypeLoaders()
        {
            Mock<IModuleInitializer> mockInit = new Mock<IModuleInitializer>();
            var moduleInfo = CreateModuleInfo("needsRetrieval", InitializationMode.WhenAvailable);
            var catalog = new Mock<IModuleCatalog>();
            ModuleManager manager = new ModuleManager(mockInit.Object, catalog.Object, new MockLogger());

            var mockTypeLoader = new Mock<IModuleTypeLoader>();
            var disposableMockTypeLoader = mockTypeLoader.As<IDisposable>();
            disposableMockTypeLoader.Setup(loader => loader.Dispose());

            manager.ModuleTypeLoaders = new List<IModuleTypeLoader> { mockTypeLoader.Object };

            manager.Dispose();

            disposableMockTypeLoader.Verify(loader => loader.Dispose(), Times.Once());
        }

        [TestMethod]
        public void DisposeDoesNotThrowWithMixedTypeLoaders()
        {
            Mock<IModuleInitializer> mockInit = new Mock<IModuleInitializer>();
            var moduleInfo = CreateModuleInfo("needsRetrieval", InitializationMode.WhenAvailable);
            var catalog = new Mock<IModuleCatalog>();
            ModuleManager manager = new ModuleManager(mockInit.Object, catalog.Object, new MockLogger());

            var mockTypeLoader1 = new Mock<IModuleTypeLoader>();

            var mockTypeLoader = new Mock<IModuleTypeLoader>();
            var disposableMockTypeLoader = mockTypeLoader.As<IDisposable>();
            disposableMockTypeLoader.Setup(loader => loader.Dispose());

            manager.ModuleTypeLoaders = new List<IModuleTypeLoader>() { mockTypeLoader1.Object, mockTypeLoader.Object };
            
            try
            {
                manager.Dispose();
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            disposableMockTypeLoader.Verify(loader => loader.Dispose(), Times.Once());
        }
        private static ModuleInfo CreateModuleInfo(string name, InitializationMode initializationMode, params string[] dependsOn)
        {
            ModuleInfo moduleInfo = new ModuleInfo(name, name)
            {
                InitializationMode = initializationMode
            };
            moduleInfo.DependsOn.AddRange(dependsOn);
            return moduleInfo;
        }

        private static ModuleInfo CreateModuleInfo(Type type, InitializationMode initializationMode, params string[] dependsOn)
        {
            ModuleInfo moduleInfo = new ModuleInfo(type.Name, type.AssemblyQualifiedName)
            {
                InitializationMode = initializationMode
            };
            moduleInfo.DependsOn.AddRange(dependsOn);
            return moduleInfo;
        }
    }

    internal class MockModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            throw new NotImplementedException();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            throw new NotImplementedException();
        }
    }

    internal class MockModuleCatalog : IModuleCatalog
    {
        public List<IModuleInfo> Modules = new List<IModuleInfo>();
        public Func<IModuleInfo, IEnumerable<IModuleInfo>> GetDependentModules;

        public Func<IEnumerable<IModuleInfo>, IEnumerable<IModuleInfo>> CompleteListWithDependencies;
        public Action ValidateCatalog;

        public void Initialize()
        {
            this.ValidateCatalog?.Invoke();
        }

        IEnumerable<IModuleInfo> IModuleCatalog.Modules => Modules;

        IEnumerable<IModuleInfo> IModuleCatalog.GetDependentModules(IModuleInfo moduleInfo)
        {
            if (GetDependentModules == null)
                return new List<IModuleInfo>();

            return GetDependentModules(moduleInfo);
        }

        IEnumerable<IModuleInfo> IModuleCatalog.CompleteListWithDependencies(IEnumerable<IModuleInfo> modules)
        {
            if (CompleteListWithDependencies != null)
                return CompleteListWithDependencies(modules);
            return modules;
        }


        public IModuleCatalog AddModule(IModuleInfo moduleInfo)
        {
            this.Modules.Add(moduleInfo);
            return this;
        }
    }

    internal class MockModuleInitializer : IModuleInitializer
    {
        public bool InitializeCalled;
        public List<IModuleInfo> InitializedModules = new List<IModuleInfo>();

        public void Initialize(IModuleInfo moduleInfo)
        {
            InitializeCalled = true;            
            this.InitializedModules.Add(moduleInfo);
        }
    }

    internal class StubModuleInitializer : IModuleInitializer
    {
        public Action<ModuleInfo> Initialize;

        void IModuleInitializer.Initialize(IModuleInfo moduleInfo)
        {
            this.Initialize((ModuleInfo)moduleInfo);
        }
    }

    internal class MockDelegateModuleInitializer : IModuleInitializer
    {
        public Action<ModuleInfo> LoadBody;

        public void Initialize(IModuleInfo moduleInfo)
        {
            LoadBody((ModuleInfo)moduleInfo);
        }
    }
}
