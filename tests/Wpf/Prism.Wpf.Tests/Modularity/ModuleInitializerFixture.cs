using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Modularity
{
    /// <summary>
    /// Summary description for ModuleInitializerFixture
    /// </summary>
    
    public class ModuleInitializerFixture
    {
        [Fact]
        public void NullContainerThrows()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                ModuleInitializer loader = new ModuleInitializer(null, new MockLogger());
            });
            
        }

        [Fact]
        public void NullLoggerThrows()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                ModuleInitializer loader = new ModuleInitializer(new MockContainerAdapter(), null);
            });
            
        }

        [Fact]
        public void InitializationExceptionsAreWrapped()
        {
            var ex = Assert.Throws<ModuleInitializeException>(() =>
            {
                var moduleInfo = CreateModuleInfo(typeof(ExceptionThrowingModule));

                ModuleInitializer loader = new ModuleInitializer(new MockContainerAdapter(), new MockLogger());

                loader.Initialize(moduleInfo);
            });

        }


        [Fact]
        public void ShouldResolveModuleAndInitializeSingleModule()
        {
            IContainerExtension containerFacade = new MockContainerAdapter();
            var service = new ModuleInitializer(containerFacade, new MockLogger());
            FirstTestModule.wasInitializedOnce = false;
            var info = CreateModuleInfo(typeof(FirstTestModule));
            service.Initialize(info);
            Assert.True(FirstTestModule.wasInitializedOnce);
        }


        [Fact]
        public void ShouldLogModuleInitializeErrorsAndContinueLoading()
        {
            IContainerExtension containerFacade = new MockContainerAdapter();
            var logger = new MockLogger();
            var service = new CustomModuleInitializerService(containerFacade, logger);
            var invalidModule = CreateModuleInfo(typeof(InvalidModule));

            Assert.False(service.HandleModuleInitializerrorCalled);
            service.Initialize(invalidModule);
            Assert.True(service.HandleModuleInitializerrorCalled);
        }

        [Fact]
        public void ShouldLogModuleInitializationError()
        {
            IContainerExtension containerFacade = new MockContainerAdapter();
            var logger = new MockLogger();
            var service = new ModuleInitializer(containerFacade, logger);
            ExceptionThrowingModule.wasInitializedOnce = false;
            var exceptionModule = CreateModuleInfo(typeof(ExceptionThrowingModule));

            try
            {
                service.Initialize(exceptionModule);
            }
            catch (ModuleInitializeException)
            {
            }

            Assert.NotNull(logger.LastMessage);
            Assert.Contains("ExceptionThrowingModule", logger.LastMessage);
        }

        [Fact]
        public void ShouldThrowExceptionIfBogusType()
        {
            var moduleInfo = new ModuleInfo("TestModule", "BadAssembly.BadType");

            ModuleInitializer loader = new ModuleInitializer(new MockContainerAdapter(), new MockLogger());

            try
            {
                loader.Initialize(moduleInfo);
                //Assert.Fail("Did not throw exception");
            }
            catch (ModuleInitializeException ex)
            {
                Assert.Contains("BadAssembly.BadType", ex.Message);
            }
            catch(Exception)
            {
                //Assert.Fail();
            }

        }

        private static ModuleInfo CreateModuleInfo(Type type, params string[] dependsOn)
        {
            ModuleInfo moduleInfo = new ModuleInfo(type.Name, type.AssemblyQualifiedName);
            moduleInfo.DependsOn.AddRange(dependsOn);
            return moduleInfo;
        }

        public static class ModuleLoadTracker
        {
            public static readonly Stack<Type> ModuleLoadStack = new Stack<Type>();
        }

        public class FirstTestModule : IModule
        {
            public static bool wasInitializedOnce;

            public void OnInitialized(IContainerProvider containerProvider)
            {
                wasInitializedOnce = true;
                ModuleLoadTracker.ModuleLoadStack.Push(GetType());
            }

            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                
            }
        }

        public class SecondTestModule : IModule
        {
            public static bool wasInitializedOnce;
            public static long initializedOnTickCount;

            public void OnInitialized(IContainerProvider containerProvider)
            {
                wasInitializedOnce = true;
                ModuleLoadTracker.ModuleLoadStack.Push(GetType());
            }

            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                
            }
        }

        public class DependantModule : IModule
        {
            public static bool wasInitializedOnce;

            public void OnInitialized(IContainerProvider containerProvider)
            {
                wasInitializedOnce = true;
                ModuleLoadTracker.ModuleLoadStack.Push(GetType());
            }

            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                
            }
        }

        public class DependencyModule : IModule
        {
            public static bool wasInitializedOnce;
            public static long initializedOnTickCount;

            public void OnInitialized(IContainerProvider containerProvider)
            {
                wasInitializedOnce = true;
                ModuleLoadTracker.ModuleLoadStack.Push(GetType());
            }

            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                
            }
        }

        public class ExceptionThrowingModule : IModule
        {
            public static bool wasInitializedOnce;
            public static long initializedOnTickCount;

            public void OnInitialized(IContainerProvider containerProvider)
            {
                throw new InvalidOperationException("Intialization can't be performed");
            }

            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                throw new NotImplementedException();
            }
        }

        public class InvalidModule { }

        public class CustomModuleInitializerService : ModuleInitializer
        {
            public bool HandleModuleInitializerrorCalled;

            public CustomModuleInitializerService(IContainerExtension containerFacade, ILoggerFacade logger)
                : base(containerFacade, logger)
            {
            }

            public override void HandleModuleInitializationError(IModuleInfo moduleInfo, string assemblyName, Exception exception)
            {
                HandleModuleInitializerrorCalled = true;
            }
        }

        public class Module1 : IModule
        {
            void IModule.OnInitialized(IContainerProvider containerProvider) { }
            void IModule.RegisterTypes(IContainerRegistry containerRegistry) { }
        }
        public class Module2 : IModule
        {
            void IModule.OnInitialized(IContainerProvider containerProvider) { }
            void IModule.RegisterTypes(IContainerRegistry containerRegistry) { }
        }
        public class Module3 : IModule
        {
            void IModule.OnInitialized(IContainerProvider containerProvider) { }
            void IModule.RegisterTypes(IContainerRegistry containerRegistry) { }
        }
        public class Module4 : IModule
        {
            void IModule.OnInitialized(IContainerProvider containerProvider) { }
            void IModule.RegisterTypes(IContainerRegistry containerRegistry) { }
        }
    }
}