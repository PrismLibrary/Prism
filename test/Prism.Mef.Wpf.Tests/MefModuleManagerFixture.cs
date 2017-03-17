

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Logging;
using Prism.Mef.Modularity;
using Prism.Modularity;

namespace Prism.Mef.Wpf.Tests
{
    [TestClass]
    public partial class MefModuleManagerFixture
    {
        [TestMethod]
        public void ModuleNeedsRetrievalReturnsTrueWhenModulesAreNotImported()
        {
            TestableMefModuleManager moduleManager = new TestableMefModuleManager();
            bool result = moduleManager.CallModuleNeedsRetrieval(new ModuleInfo("name", "type"));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ModuleNeedsRetrievalReturnsTrueWhenNoModulesAreAvailable()
        {
            TestableMefModuleManager moduleManager = new TestableMefModuleManager
                                                         {
                                                             Modules = (IEnumerable<Lazy<IModule, IModuleExport>>)new List<Lazy<IModule, IModuleExport>>()
                                                         };

            bool result = moduleManager.CallModuleNeedsRetrieval(new ModuleInfo("name", "type"));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ModuleNeedsRetrievalReturnsTrueWhenModuleNotFound()
        {
            TestableMefModuleManager moduleManager = new TestableMefModuleManager
            {
                Modules = new List<Lazy<IModule, IModuleExport>>() { new Lazy<IModule, IModuleExport>(() => new TestModule(), new TestModuleMetadata()) }
            };

            bool result = moduleManager.CallModuleNeedsRetrieval(new ModuleInfo("name", "type"));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ModuleNeedsRetrievalReturnsFalseWhenModuleIsFound()
        {
            TestableMefModuleManager moduleManager = new TestableMefModuleManager
            {
                Modules = new List<Lazy<IModule, IModuleExport>>() { new Lazy<IModule, IModuleExport>(() => new TestModule(), new TestModuleMetadata()) }
            };

            bool result = moduleManager.CallModuleNeedsRetrieval(new ModuleInfo("TestModule", "Microsoft.Practices.Prism.MefExtensions.Tests.MefModuleManagerFixture.TestModule"));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DownloadCompletedRaisedForImportedModulesInCatalog()
        {
            var mockModuleInitializer = new Mock<IModuleInitializer>();
            var modules = new List<ModuleInfo>();

            var mockModuleCatalog = new Mock<IModuleCatalog>();
            mockModuleCatalog.Setup(x => x.Modules).Returns(modules);
            mockModuleCatalog
                .Setup(x => x.AddModule(It.IsAny<ModuleInfo>()))
                .Callback<ModuleInfo>(modules.Add);

            var mockLogger = new Mock<ILoggerFacade>();

            List<ModuleInfo> modulesCompleted = new List<ModuleInfo>();
            var moduleManager = new TestableMefModuleManager(mockModuleInitializer.Object, mockModuleCatalog.Object, mockLogger.Object)
                                    {
                                        Modules =
                                            new List<Lazy<IModule, IModuleExport>>()
                                                {
                                                    new Lazy<IModule, IModuleExport>(() => new TestModule(),
                                                                                     new TestModuleMetadata())
                                                }
                                    };

            moduleManager.LoadModuleCompleted += (o, e) =>
                                                     {
                                                         modulesCompleted.Add(e.ModuleInfo);
                                                     };

            moduleManager.OnImportsSatisfied();

            Assert.AreEqual(1, modulesCompleted.Count);
        }

        [TestMethod]
        public void DownloadCompletedNotRaisedForOnDemandImportedModules()
        {
            var mockModuleInitializer = new Mock<IModuleInitializer>();
            var modules = new List<ModuleInfo>();

            var mockModuleCatalog = new Mock<IModuleCatalog>();
            mockModuleCatalog.Setup(x => x.Modules).Returns(modules);
            mockModuleCatalog
                .Setup(x => x.AddModule(It.IsAny<ModuleInfo>()))
                .Callback<ModuleInfo>(modules.Add);

            var mockLogger = new Mock<ILoggerFacade>();

            List<ModuleInfo> modulesCompleted = new List<ModuleInfo>();
            var moduleManager = new TestableMefModuleManager(mockModuleInitializer.Object, mockModuleCatalog.Object, mockLogger.Object)
            {
                Modules =
                    new List<Lazy<IModule, IModuleExport>>()
                                                {
                                                    new Lazy<IModule, IModuleExport>(() => new TestModule(),
                                                                                     new TestModuleMetadata()
                                                                                         {
                                                                                             InitializationMode = InitializationMode.OnDemand
                                                                                         }),
                                                    new Lazy<IModule, IModuleExport>(() => new TestModule(),
                                                                                        new TestModuleMetadata()
                                                                                            {
                                                                                                ModuleName = "WhenAvailableModule"
                                                                                            })
                                                }
            };

            moduleManager.LoadModuleCompleted += (o, e) =>
            {
                modulesCompleted.Add(e.ModuleInfo);
            };

            moduleManager.OnImportsSatisfied();

            Assert.AreEqual(1, modulesCompleted.Count);
            Assert.AreEqual("WhenAvailableModule", modulesCompleted[0].ModuleName);
        }

        public class TestModule : IModule
        {
            public void Initialize()
            {
                //no-op
            }
        }

        public class TestModuleMetadata : IModuleExport
        {
            public TestModuleMetadata()
            {
                this.ModuleName = "TestModule";
                this.ModuleType = typeof(TestModule);
                this.InitializationMode = InitializationMode.WhenAvailable;
                this.DependsOnModuleNames = null;
            }

            public string ModuleName { get; set; }

            public Type ModuleType { get; set; }

            public InitializationMode InitializationMode { get; set; }

            public string[] DependsOnModuleNames { get; set; }
        }

        internal class TestableMefModuleManager : MefModuleManager
        {
            public TestableMefModuleManager()
                : this(new Mock<IModuleInitializer>().Object, new Mock<IModuleCatalog>().Object, new Mock<ILoggerFacade>().Object)
            {
            }

            public TestableMefModuleManager(IModuleInitializer moduleInitializer, IModuleCatalog moduleCatalog, ILoggerFacade loggerFacade)
                : base(moduleInitializer, moduleCatalog, loggerFacade)
            {
            }

            public IEnumerable<Lazy<IModule, IModuleExport>> Modules
            {
                get { return base.ImportedModules; }
                set { base.ImportedModules = value; }
            }

            public bool CallModuleNeedsRetrieval(ModuleInfo moduleInfo)
            {
                return base.ModuleNeedsRetrieval(moduleInfo);
            }
        }
    }
}