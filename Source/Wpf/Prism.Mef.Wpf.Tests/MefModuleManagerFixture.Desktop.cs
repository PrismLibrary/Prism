

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Logging;
using Prism.Mef.Modularity;
using Prism.Modularity;

namespace Prism.Mef.Wpf.Tests
{
    public partial class MefModuleManagerFixture
    {
        private const string WpfTestSupportAssemblyName = "Prism.Mef.Wpf.Tests.Support";
        private const string WpfTestSupportAssemblyNamespace = "Prism.Mef.Wpf.Tests.Support";
        private const string SupportAssemblyDebug = @"..\..\..\Prism.Mef.Wpf.Tests.Support\bin\debug\Prism.Mef.Wpf.Tests.Support.dll";
        private const string SupportAssemblyRelease = @"..\..\..\Prism.Mef.Wpf.Tests.Support\bin\release\Prism.Mef.Wpf.Tests.Support.dll";

        [TestMethod]
        public void ConstructorThrowsWithNullModuleInitializer()
        {
            try
            {
                new MefModuleManager(null, new Mock<IModuleCatalog>().Object, new Mock<ILoggerFacade>().Object);
                Assert.Fail("No exception thrown when expected");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("moduleInitializer", ex.ParamName);
            }
        }

        [TestMethod]
        public void ConstructorThrowsWithNullModuleCatalog()
        {
            try
            {
                new MefModuleManager(new Mock<IModuleInitializer>().Object, null, new Mock<ILoggerFacade>().Object);
                Assert.Fail("No exception thrown when expected");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("moduleCatalog", ex.ParamName);
            }
        }

        [TestMethod]
        public void ConstructorThrowsWithNullLogger()
        {
            try
            {
                new MefModuleManager(new Mock<IModuleInitializer>().Object, new Mock<IModuleCatalog>().Object, null);
                Assert.Fail("No exception thrown when expected");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("loggerFacade", ex.ParamName);
            }
        }

#if DEBUG
        [DeploymentItem(SupportAssemblyDebug)]
#else
        [DeploymentItem(SupportAssemblyRelease)]
#endif
        [TestMethod]
        public void ModuleInUnreferencedAssemblyInitializedByModuleInitializer()
        {
            AssemblyCatalog assemblyCatalog = new AssemblyCatalog(GetPathToModuleDll());
            CompositionContainer compositionContainer = new CompositionContainer(assemblyCatalog);

            ModuleCatalog moduleCatalog = new ModuleCatalog();

            Mock<MefFileModuleTypeLoader> mockFileTypeLoader = new Mock<MefFileModuleTypeLoader>();

            compositionContainer.ComposeExportedValue<IModuleCatalog>(moduleCatalog);
            compositionContainer.ComposeExportedValue<MefFileModuleTypeLoader>(mockFileTypeLoader.Object);

            bool wasInit = false;
            var mockModuleInitializer = new Mock<IModuleInitializer>();
            mockModuleInitializer.Setup(x => x.Initialize(It.IsAny<ModuleInfo>())).Callback(() => wasInit = true);

            var mockLoggerFacade = new Mock<ILoggerFacade>();

            MefModuleManager moduleManager = new MefModuleManager(
                mockModuleInitializer.Object,
                moduleCatalog,
                mockLoggerFacade.Object);

            compositionContainer.SatisfyImportsOnce(moduleManager);

            moduleManager.Run();

            Assert.IsTrue(wasInit);
        }

#if DEBUG
        [DeploymentItem(SupportAssemblyDebug)]
#else
        [DeploymentItem(SupportAssemblyRelease)]
#endif
        [TestMethod]
        public void DeclaredModuleWithoutTypeInUnreferencedAssemblyIsUpdatedWithTypeNameFromExportAttribute()
        {
            AggregateCatalog aggregateCatalog = new AggregateCatalog();
            CompositionContainer compositionContainer = new CompositionContainer(aggregateCatalog);

            var mockFileTypeLoader = new Mock<MefFileModuleTypeLoader>();
            mockFileTypeLoader.Setup(tl => tl.CanLoadModuleType(It.IsAny<ModuleInfo>())).Returns(true);


            ModuleCatalog moduleCatalog = new ModuleCatalog();
            ModuleInfo moduleInfo = new ModuleInfo { ModuleName = "MefModuleOne" };
            moduleCatalog.AddModule(moduleInfo);

            compositionContainer.ComposeExportedValue<IModuleCatalog>(moduleCatalog);
            compositionContainer.ComposeExportedValue<MefFileModuleTypeLoader>(mockFileTypeLoader.Object);

            bool wasInit = false;
            var mockModuleInitializer = new Mock<IModuleInitializer>();
            mockModuleInitializer.Setup(x => x.Initialize(It.IsAny<ModuleInfo>())).Callback(() => wasInit = true);

            var mockLoggerFacade = new Mock<ILoggerFacade>();

            MefModuleManager moduleManager = new MefModuleManager(
                mockModuleInitializer.Object,
                moduleCatalog,
                mockLoggerFacade.Object);

            compositionContainer.SatisfyImportsOnce(moduleManager);
            moduleManager.Run();

            Assert.IsFalse(wasInit);

            AssemblyCatalog assemblyCatalog = new AssemblyCatalog(GetPathToModuleDll());
            aggregateCatalog.Catalogs.Add(assemblyCatalog);

            compositionContainer.SatisfyImportsOnce(moduleManager);

            mockFileTypeLoader.Raise(tl => tl.LoadModuleCompleted += null, new LoadModuleCompletedEventArgs(moduleInfo, null));

            Assert.AreEqual(BuildMefSupportTypeName(), moduleInfo.ModuleType);
            Assert.IsTrue(wasInit);
        }

#if DEBUG
        [DeploymentItem(SupportAssemblyDebug)]
#else
        [DeploymentItem(SupportAssemblyRelease)]
#endif
        [TestMethod]
        public void DeclaredModuleWithTypeInUnreferencedAssemblyIsUpdatedWithTypeNameFromExportAttribute()
        {
            AggregateCatalog aggregateCatalog = new AggregateCatalog();
            CompositionContainer compositionContainer = new CompositionContainer(aggregateCatalog);

            var mockFileTypeLoader = new Mock<MefFileModuleTypeLoader>();
            mockFileTypeLoader.Setup(tl => tl.CanLoadModuleType(It.IsAny<ModuleInfo>())).Returns(true);


            ModuleCatalog moduleCatalog = new ModuleCatalog();
            ModuleInfo moduleInfo = new ModuleInfo { ModuleName = "MefModuleOne", ModuleType = "some type" };
            moduleCatalog.AddModule(moduleInfo);

            compositionContainer.ComposeExportedValue<IModuleCatalog>(moduleCatalog);
            compositionContainer.ComposeExportedValue<MefFileModuleTypeLoader>(mockFileTypeLoader.Object);

            bool wasInit = false;
            var mockModuleInitializer = new Mock<IModuleInitializer>();
            mockModuleInitializer.Setup(x => x.Initialize(It.IsAny<ModuleInfo>())).Callback(() => wasInit = true);

            var mockLoggerFacade = new Mock<ILoggerFacade>();

            MefModuleManager moduleManager = new MefModuleManager(
                mockModuleInitializer.Object,
                moduleCatalog,
                mockLoggerFacade.Object);

            compositionContainer.SatisfyImportsOnce(moduleManager);
            moduleManager.Run();

            Assert.IsFalse(wasInit);

            AssemblyCatalog assemblyCatalog = new AssemblyCatalog(GetPathToModuleDll());
            aggregateCatalog.Catalogs.Add(assemblyCatalog);

            compositionContainer.SatisfyImportsOnce(moduleManager);

            mockFileTypeLoader.Raise(tl => tl.LoadModuleCompleted += null, new LoadModuleCompletedEventArgs(moduleInfo, null));

            Assert.AreEqual(BuildMefSupportTypeName(), moduleInfo.ModuleType);
            Assert.IsTrue(wasInit);
        }

        // All test namespaces should be conform assembly name.
        // These tests help find typos in declared constants
        [TestMethod]
        public void SupportAssemblyDebugIsConformAssemblyName()
        {
            string debugDllPath = BuildMefSupportDllPath(false);

            Assert.AreEqual(SupportAssemblyDebug, debugDllPath);
        }

        [TestMethod]
        public void SupportAssemblyReleaseIsConformAssemblyName()
        {
            string debugDllPath = BuildMefSupportDllPath(true);

            Assert.AreEqual(SupportAssemblyRelease, debugDllPath);
        }

        [TestMethod]
        public void MefSupportTypeNameIsConformAssemblyName()
        {
            string typeName = BuildMefSupportTypeName();

            Assert.AreEqual("Prism.Mef.Wpf.Tests.Support.MefModuleOne, Prism.Mef.Wpf.Tests.Support, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", typeName);
        }

        // Due to different test runners and file locations, this helper function will help find the 
        // necessary DLL for tests to execute.
        private static string GetPathToModuleDll()
        {
            string fileLocation = null;
            if (File.Exists(WpfTestSupportAssemblyName + ".dll"))
            {
                fileLocation = WpfTestSupportAssemblyName + ".dll";
            }
            else if (File.Exists(SupportAssemblyDebug))
            {
                fileLocation = SupportAssemblyDebug;
            }
            else if (File.Exists(SupportAssemblyRelease))
            {
                fileLocation = SupportAssemblyRelease;
            }
            else
            {
                Assert.Fail("Cannot find module for testing");
            }

            return fileLocation;
        }

        private static string BuildMefSupportDllPath(bool release)
        {
            if (release)
            {
                return string.Format(@"..\..\..\{0}\bin\release\{0}.dll", WpfTestSupportAssemblyName);
            }
            else
            {
                return string.Format(@"..\..\..\{0}\bin\debug\{0}.dll", WpfTestSupportAssemblyName);
            }
        }

        private string BuildMefSupportTypeName()
        {
            return string.Format("{0}.MefModuleOne, {1}, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                WpfTestSupportAssemblyNamespace, WpfTestSupportAssemblyName);
        }
    }
}
