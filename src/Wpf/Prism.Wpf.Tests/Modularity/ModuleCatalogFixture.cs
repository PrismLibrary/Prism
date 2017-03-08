

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Modularity;

namespace Prism.Wpf.Tests.Modularity
{
    [TestClass]
    public class ModuleCatalogFixture
    {
        [TestMethod]
        public void CanCreateCatalogFromList()
        {
            var moduleInfo = new ModuleInfo("MockModule", "type");
            List<ModuleInfo> moduleInfos = new List<ModuleInfo> { moduleInfo };

            var moduleCatalog = new ModuleCatalog(moduleInfos);

            Assert.AreEqual(1, moduleCatalog.Modules.Count());
            Assert.AreEqual(moduleInfo, moduleCatalog.Modules.ElementAt(0));
        }

        [TestMethod]
        public void CanGetDependenciesForModule()
        {
            // A <- B
            var moduleInfoA = CreateModuleInfo("A");
            var moduleInfoB = CreateModuleInfo("B", "A");
            List<ModuleInfo> moduleInfos = new List<ModuleInfo>
                                               {
                                                   moduleInfoA
                                                   , moduleInfoB
                                               };
            var moduleCatalog = new ModuleCatalog(moduleInfos);

            IEnumerable<ModuleInfo> dependentModules = moduleCatalog.GetDependentModules(moduleInfoB);

            Assert.AreEqual(1, dependentModules.Count());
            Assert.AreEqual(moduleInfoA, dependentModules.ElementAt(0));
        }

        [TestMethod]
        public void CanCompleteListWithTheirDependencies()
        {
            // A <- B <- C
            var moduleInfoA = CreateModuleInfo("A");
            var moduleInfoB = CreateModuleInfo("B", "A");
            var moduleInfoC = CreateModuleInfo("C", "B");
            var moduleInfoOrphan = CreateModuleInfo("X", "B");

            List<ModuleInfo> moduleInfos = new List<ModuleInfo>
                                               {
                                                   moduleInfoA
                                                   , moduleInfoB
                                                   , moduleInfoC
                                                   , moduleInfoOrphan
                                               };
            var moduleCatalog = new ModuleCatalog(moduleInfos);

            IEnumerable<ModuleInfo> dependantModules = moduleCatalog.CompleteListWithDependencies(new[] { moduleInfoC });

            Assert.AreEqual(3, dependantModules.Count());
            Assert.IsTrue(dependantModules.Contains(moduleInfoA));
            Assert.IsTrue(dependantModules.Contains(moduleInfoB));
            Assert.IsTrue(dependantModules.Contains(moduleInfoC));
        }

        [TestMethod]
        [ExpectedException(typeof(CyclicDependencyFoundException))]
        public void ShouldThrowOnCyclicDependency()
        {
            // A <- B <- C <- A
            var moduleInfoA = CreateModuleInfo("A", "C");
            var moduleInfoB = CreateModuleInfo("B", "A");
            var moduleInfoC = CreateModuleInfo("C", "B");

            List<ModuleInfo> moduleInfos = new List<ModuleInfo>
                                               {
                                                   moduleInfoA
                                                   , moduleInfoB
                                                   , moduleInfoC
                                               };
            new ModuleCatalog(moduleInfos).Validate();
        }


        [TestMethod]
        [ExpectedException(typeof(DuplicateModuleException))]
        public void ShouldThrowOnDuplicateModule()
        {

            var moduleInfoA1 = CreateModuleInfo("A");
            var moduleInfoA2 = CreateModuleInfo("A");

            List<ModuleInfo> moduleInfos = new List<ModuleInfo>
                                               {
                                                   moduleInfoA1
                                                   , moduleInfoA2
                                               };
            new ModuleCatalog(moduleInfos).Validate();
        }

        [TestMethod]
        [ExpectedException(typeof(ModularityException))]
        public void ShouldThrowOnMissingDependency()
        {
            var moduleInfoA = CreateModuleInfo("A", "B");

            List<ModuleInfo> moduleInfos = new List<ModuleInfo>
                                               {
                                                   moduleInfoA
                                               };
            new ModuleCatalog(moduleInfos).Validate();
        }

        [TestMethod]
        public void CanAddModules()
        {
            var catalog = new ModuleCatalog();

            catalog.AddModule(typeof(MockModule));

            Assert.AreEqual(1, catalog.Modules.Count());
            Assert.AreEqual("MockModule", catalog.Modules.First().ModuleName);
        }

        [TestMethod]
        public void CanAddGroups()
        {
            var catalog = new ModuleCatalog();

            ModuleInfo moduleInfo = new ModuleInfo();
            ModuleInfoGroup group = new ModuleInfoGroup { moduleInfo };
            catalog.Items.Add(group);

            Assert.AreEqual(1, catalog.Modules.Count());
            Assert.AreSame(moduleInfo, catalog.Modules.ElementAt(0));
        }

        [TestMethod]
        public void ShouldAggregateGroupsAndLooseModuleInfos()
        {
            var catalog = new ModuleCatalog();
            ModuleInfo moduleInfo1 = new ModuleInfo();
            ModuleInfo moduleInfo2 = new ModuleInfo();
            ModuleInfo moduleInfo3 = new ModuleInfo();

            catalog.Items.Add(new ModuleInfoGroup() { moduleInfo1 });
            catalog.Items.Add(new ModuleInfoGroup() { moduleInfo2 });
            catalog.AddModule(moduleInfo3);

            Assert.AreEqual(3, catalog.Modules.Count());
            Assert.IsTrue(catalog.Modules.Contains(moduleInfo1));
            Assert.IsTrue(catalog.Modules.Contains(moduleInfo2));
            Assert.IsTrue(catalog.Modules.Contains(moduleInfo3));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompleteListWithDependenciesThrowsWithNull()
        {
            var catalog = new ModuleCatalog();
            catalog.CompleteListWithDependencies(null);
        }

        [TestMethod]
        public void LooseModuleIfDependentOnModuleInGroupThrows()
        {
            var catalog = new ModuleCatalog();
            catalog.Items.Add(new ModuleInfoGroup() { CreateModuleInfo("ModuleA") });
            catalog.AddModule(CreateModuleInfo("ModuleB", "ModuleA"));

            try
            {
                catalog.Validate();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ModularityException));
                Assert.AreEqual("ModuleB", ((ModularityException)ex).ModuleName);

                return;
            }

            Assert.Fail("Exception not thrown.");
        }

        [TestMethod]
        public void ModuleInGroupDependsOnModuleInOtherGroupThrows()
        {
            var catalog = new ModuleCatalog();
            catalog.Items.Add(new ModuleInfoGroup() { CreateModuleInfo("ModuleA") });
            catalog.Items.Add(new ModuleInfoGroup() { CreateModuleInfo("ModuleB", "ModuleA") });

            try
            {
                catalog.Validate();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ModularityException));
                Assert.AreEqual("ModuleB", ((ModularityException)ex).ModuleName);

                return;
            }

            Assert.Fail("Exception not thrown.");
        }

        [TestMethod]
        public void ShouldRevalidateWhenAddingNewModuleIfValidated()
        {
            var testableCatalog = new TestableModuleCatalog();
            testableCatalog.Items.Add(new ModuleInfoGroup() { CreateModuleInfo("ModuleA") });
            testableCatalog.Validate();
            testableCatalog.Items.Add(new ModuleInfoGroup() { CreateModuleInfo("ModuleB") });
            Assert.IsTrue(testableCatalog.ValidateCalled);
        }

        [TestMethod]
        public void ModuleInGroupCanDependOnModuleInSameGroup()
        {
            var catalog = new ModuleCatalog();
            var moduleA = CreateModuleInfo("ModuleA");
            var moduleB = CreateModuleInfo("ModuleB", "ModuleA");
            catalog.Items.Add(new ModuleInfoGroup()
                                  {
                                      moduleA,
                                      moduleB
                                  });

            var moduleBDependencies = catalog.GetDependentModules(moduleB);

            Assert.AreEqual(1, moduleBDependencies.Count());
            Assert.AreEqual(moduleA, moduleBDependencies.First());

        }

        [TestMethod]
        public void StartupModuleDependentOnAnOnDemandModuleThrows()
        {
            var catalog = new ModuleCatalog();
            var moduleOnDemand = CreateModuleInfo("ModuleA");
            moduleOnDemand.InitializationMode = InitializationMode.OnDemand;
            catalog.AddModule(moduleOnDemand);
            catalog.AddModule(CreateModuleInfo("ModuleB", "ModuleA"));

            try
            {
                catalog.Validate();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ModularityException));
                Assert.AreEqual("ModuleB", ((ModularityException)ex).ModuleName);

                return;
            }

            Assert.Fail("Exception not thrown.");
        }

        [TestMethod]
        public void ShouldReturnInCorrectRetrieveOrderWhenCompletingListWithDependencies()
        {
            // A <- B <- C <- D,    C <- X
            var moduleA = CreateModuleInfo("A");
            var moduleB = CreateModuleInfo("B", "A");
            var moduleC = CreateModuleInfo("C", "B");
            var moduleD = CreateModuleInfo("D", "C");
            var moduleX = CreateModuleInfo("X", "C");

            var moduleCatalog = new ModuleCatalog();
            // Add the modules in random order
            moduleCatalog.AddModule(moduleB);
            moduleCatalog.AddModule(moduleA);
            moduleCatalog.AddModule(moduleD);
            moduleCatalog.AddModule(moduleX);
            moduleCatalog.AddModule(moduleC);

            var dependantModules = moduleCatalog.CompleteListWithDependencies(new[] { moduleD, moduleX }).ToList();

            Assert.AreEqual(5, dependantModules.Count);
            Assert.IsTrue(dependantModules.IndexOf(moduleA) < dependantModules.IndexOf(moduleB));
            Assert.IsTrue(dependantModules.IndexOf(moduleB) < dependantModules.IndexOf(moduleC));
            Assert.IsTrue(dependantModules.IndexOf(moduleC) < dependantModules.IndexOf(moduleD));
            Assert.IsTrue(dependantModules.IndexOf(moduleC) < dependantModules.IndexOf(moduleX));
        }

        [TestMethod]
        public void CanLoadCatalogFromXaml()
        {
            Stream stream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "Prism.Wpf.Tests.Modularity.ModuleCatalogXaml.SimpleModuleCatalog.xaml");

            var catalog = ModuleCatalog.CreateFromXaml(stream);
            Assert.IsNotNull(catalog);

            Assert.AreEqual(4, catalog.Modules.Count());
        }


        [TestMethod]
        public void ShouldLoadAndValidateOnInitialize()
        {
            var catalog = new TestableModuleCatalog();

            var testableCatalog = new TestableModuleCatalog();
            Assert.IsFalse(testableCatalog.LoadCalled);
            Assert.IsFalse(testableCatalog.ValidateCalled);

            testableCatalog.Initialize();
            Assert.IsTrue(testableCatalog.LoadCalled);
            Assert.IsTrue(testableCatalog.ValidateCalled);
            Assert.IsTrue(testableCatalog.LoadCalledFirst);
        }

        [TestMethod]
        public void ShouldNotLoadAgainIfInitializedCalledMoreThanOnce()
        {
            var catalog = new TestableModuleCatalog();

            var testableCatalog = new TestableModuleCatalog();
            Assert.IsFalse(testableCatalog.LoadCalled);
            Assert.IsFalse(testableCatalog.ValidateCalled);

            testableCatalog.Initialize();
            Assert.AreEqual<int>(1, testableCatalog.LoadCalledCount);
            testableCatalog.Initialize();
            Assert.AreEqual<int>(1, testableCatalog.LoadCalledCount);
        }

        [TestMethod]
        public void ShouldNotLoadAgainDuringInitialize()
        {
            var catalog = new TestableModuleCatalog();

            var testableCatalog = new TestableModuleCatalog();
            Assert.IsFalse(testableCatalog.LoadCalled);
            Assert.IsFalse(testableCatalog.ValidateCalled);

            testableCatalog.Load();
            Assert.AreEqual<int>(1, testableCatalog.LoadCalledCount);
            testableCatalog.Initialize();
            Assert.AreEqual<int>(1, testableCatalog.LoadCalledCount);
        }


        [TestMethod]
        public void ShouldAllowLoadToBeInvokedTwice()
        {
            var catalog = new TestableModuleCatalog();

            var testableCatalog = new TestableModuleCatalog();
            testableCatalog.Load();
            Assert.AreEqual<int>(1, testableCatalog.LoadCalledCount);
            testableCatalog.Load();
            Assert.AreEqual<int>(2, testableCatalog.LoadCalledCount);
        }

        [TestMethod]
        public void CanAddModule1()
        {
            ModuleCatalog catalog = new ModuleCatalog();

            catalog.AddModule("Module", "ModuleType", InitializationMode.OnDemand, "DependsOn1", "DependsOn2");

            Assert.AreEqual(1, catalog.Modules.Count());
            Assert.AreEqual("Module", catalog.Modules.First().ModuleName);
            Assert.AreEqual("ModuleType", catalog.Modules.First().ModuleType);
            Assert.AreEqual(InitializationMode.OnDemand, catalog.Modules.First().InitializationMode);
            Assert.AreEqual(2, catalog.Modules.First().DependsOn.Count);
            Assert.AreEqual("DependsOn1", catalog.Modules.First().DependsOn[0]);
            Assert.AreEqual("DependsOn2", catalog.Modules.First().DependsOn[1]);

        }

        [TestMethod]
        public void CanAddModule2()
        {
            ModuleCatalog catalog = new ModuleCatalog();

            catalog.AddModule("Module", "ModuleType", "DependsOn1", "DependsOn2");

            Assert.AreEqual(1, catalog.Modules.Count());
            Assert.AreEqual("Module", catalog.Modules.First().ModuleName);
            Assert.AreEqual("ModuleType", catalog.Modules.First().ModuleType);
            Assert.AreEqual(InitializationMode.WhenAvailable, catalog.Modules.First().InitializationMode);
            Assert.AreEqual(2, catalog.Modules.First().DependsOn.Count);
            Assert.AreEqual("DependsOn1", catalog.Modules.First().DependsOn[0]);
            Assert.AreEqual("DependsOn2", catalog.Modules.First().DependsOn[1]);

        }
        [TestMethod]
        public void CanAddModule3()
        {
            ModuleCatalog catalog = new ModuleCatalog();

            catalog.AddModule(typeof(MockModule), InitializationMode.OnDemand, "DependsOn1", "DependsOn2");

            Assert.AreEqual(1, catalog.Modules.Count());
            Assert.AreEqual("MockModule", catalog.Modules.First().ModuleName);
            Assert.AreEqual(typeof(MockModule).AssemblyQualifiedName, catalog.Modules.First().ModuleType);
            Assert.AreEqual(InitializationMode.OnDemand, catalog.Modules.First().InitializationMode);
            Assert.AreEqual(2, catalog.Modules.First().DependsOn.Count);
            Assert.AreEqual("DependsOn1", catalog.Modules.First().DependsOn[0]);
            Assert.AreEqual("DependsOn2", catalog.Modules.First().DependsOn[1]);

        }

        [TestMethod]
        public void CanAddModule4()
        {
            ModuleCatalog catalog = new ModuleCatalog();

            catalog.AddModule(typeof(MockModule), "DependsOn1", "DependsOn2");

            Assert.AreEqual(1, catalog.Modules.Count());
            Assert.AreEqual("MockModule", catalog.Modules.First().ModuleName);
            Assert.AreEqual(typeof(MockModule).AssemblyQualifiedName, catalog.Modules.First().ModuleType);
            Assert.AreEqual(InitializationMode.WhenAvailable, catalog.Modules.First().InitializationMode);
            Assert.AreEqual(2, catalog.Modules.First().DependsOn.Count);
            Assert.AreEqual("DependsOn1", catalog.Modules.First().DependsOn[0]);
            Assert.AreEqual("DependsOn2", catalog.Modules.First().DependsOn[1]);

        }

        [TestMethod]
        public void CanAddGroup()
        {
            ModuleCatalog catalog = new ModuleCatalog();

            catalog.Items.Add(new ModuleInfoGroup());

            catalog.AddGroup(InitializationMode.OnDemand, "Ref1",
                             new ModuleInfo("M1", "T1"),
                             new ModuleInfo("M2", "T2", "M1"));

            Assert.AreEqual(2, catalog.Modules.Count());

            var module1 = catalog.Modules.First();
            var module2 = catalog.Modules.Skip(1).First();


            Assert.AreEqual("M1", module1.ModuleName);
            Assert.AreEqual("T1", module1.ModuleType);
            Assert.AreEqual("Ref1", module1.Ref);
            Assert.AreEqual(InitializationMode.OnDemand, module1.InitializationMode);

            Assert.AreEqual("M2", module2.ModuleName);
            Assert.AreEqual("T2", module2.ModuleType);
            Assert.AreEqual("Ref1", module2.Ref);
            Assert.AreEqual(InitializationMode.OnDemand, module2.InitializationMode);
        }


        private class TestableModuleCatalog : ModuleCatalog
        {
            public bool ValidateCalled { get; set; }
            public bool LoadCalledFirst { get; set; }
            public bool LoadCalled
            {
                get { return LoadCalledCount > 0; }
            }
            public int LoadCalledCount { get; set; }

            public override void Validate()
            {
                ValidateCalled = true;
                Validated = true;
            }

            protected override void InnerLoad()
            {
                if (ValidateCalled == false && !LoadCalled)
                    LoadCalledFirst = true;

                LoadCalledCount++;
            }

        }

        private static ModuleInfo CreateModuleInfo(string name, params string[] dependsOn)
        {
            ModuleInfo moduleInfo = new ModuleInfo(name, name);
            moduleInfo.DependsOn.AddRange(dependsOn);
            return moduleInfo;
        }
    }
}
