// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Logging;
using Prism.Mef.Modularity;
using Prism.Modularity;

namespace Prism.Wpf.Mef.Tests
{
    public partial class MefModuleManagerFixture
    {
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
        [DeploymentItem(@"..\..\..\Prism.Wpf.Mef.Tests.Support\bin\debug\Prism.Wpf.Mef.Tests.Support.dll")]
#else
        [DeploymentItem(@"..\..\..\Prism.Wpf.Mef.Tests.Support\bin\release\Prism.Wpf.Mef.Tests.Support.dll")]
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
        [DeploymentItem(@"..\..\..\Prism.Wpf.Mef.Tests.Support\bin\debug\Prism.Wpf.Mef.Tests.Support.dll")]
#else
        [DeploymentItem(@"..\..\..\Prism.Wpf.Mef.Tests.Support\bin\release\Prism.Wpf.Mef.Tests.Support.dll")]
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

            Assert.AreEqual("Prism.Wpf.Mef.Tests.Support.MefModuleOne, Prism.Wpf.Mef.Tests.Support, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", moduleInfo.ModuleType);
            Assert.IsTrue(wasInit);
        }

#if DEBUG
        [DeploymentItem(@"..\..\..\Prism.Wpf.Mef.Tests.Support\bin\debug\Prism.Wpf.Mef.Tests.Support.dll")]
#else
        [DeploymentItem(@"..\..\..\Prism.Wpf.Mef.Tests.Support\bin\release\Prism.Wpf.Mef.Tests.Support.dll")]
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

            Assert.AreEqual("Prism.Wpf.Mef.Tests.Support.MefModuleOne, Prism.Wpf.Mef.Tests.Support, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", moduleInfo.ModuleType);
            Assert.IsTrue(wasInit);
        }

        // Due to different test runners and file locations, this helper function will help find the 
        // necessary DLL for tests to execute.
        private static string GetPathToModuleDll()
        {
            const string debugDll = @"..\..\..\Prism.Wpf.Mef.Tests.Support\bin\debug\Prism.Wpf.Mef.Tests.Support.dll";
            const string releaseDll = @"..\..\..\Prism.Wpf.Mef.Tests.Support\bin\release\Prism.Wpf.Mef.Tests.Support.dll";
            string fileLocation = null;
            if (File.Exists("Prism.Wpf.Mef.Tests.Support.dll"))
            {
                fileLocation = "Prism.Wpf.Mef.Tests.Support.dll";
            }
            else if (File.Exists(debugDll))
            {
                fileLocation = debugDll;
            }
            else if (File.Exists(releaseDll))
            {
                fileLocation = releaseDll;
            }
            else
            {
                Assert.Fail("Cannot find module for testing");
            }

            return fileLocation;
        }
    }
}
