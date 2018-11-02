

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Prism.Modularity;

namespace Prism.Wpf.Tests.Modularity
{
    public class ModuleCatalogFixture
    {
        [Fact]
        public void CanCreateCatalogFromList()
        {
            var moduleInfo = new ModuleInfo("MockModule", "type");
            List<ModuleInfo> moduleInfos = new List<ModuleInfo> { moduleInfo };

            var moduleCatalog = new ModuleCatalog(moduleInfos);

            Assert.Single(moduleCatalog.Modules);
            Assert.Equal(moduleInfo, moduleCatalog.Modules.ElementAt(0));
        }

        [Fact]
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

            var dependentModules = moduleCatalog.GetDependentModules(moduleInfoB);

            Assert.Single(dependentModules);
            Assert.Equal(moduleInfoA, dependentModules.ElementAt(0));
        }

        [Fact]
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

            var dependantModules = moduleCatalog.CompleteListWithDependencies(new[] { moduleInfoC });

            Assert.Equal(3, dependantModules.Count());
            Assert.Contains(moduleInfoA, dependantModules);
            Assert.Contains(moduleInfoB, dependantModules);
            Assert.Contains(moduleInfoC, dependantModules);
        }

        [Fact]
        public void ShouldThrowOnCyclicDependency()
        {
            var ex = Assert.Throws<CyclicDependencyFoundException>(() =>
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
            });

        }


        [Fact]
        public void ShouldThrowOnDuplicateModule()
        {
            var ex = Assert.Throws<DuplicateModuleException>(() =>
            {
                var moduleInfoA1 = CreateModuleInfo("A");
                var moduleInfoA2 = CreateModuleInfo("A");

                List<ModuleInfo> moduleInfos = new List<ModuleInfo>
                                               {
                                                   moduleInfoA1
                                                   , moduleInfoA2
                                               };
                new ModuleCatalog(moduleInfos).Validate();
            });
        }

        [Fact]
        public void ShouldThrowOnMissingDependency()
        {
            var ex = Assert.Throws<ModularityException>(() =>
            {
                var moduleInfoA = CreateModuleInfo("A", "B");

                List<ModuleInfo> moduleInfos = new List<ModuleInfo>
                                               {
                                                   moduleInfoA
                                               };
                new ModuleCatalog(moduleInfos).Validate();
            });
        }

        [Fact]
        public void CanAddModules()
        {
            var catalog = new ModuleCatalog();

            catalog.AddModule(typeof(MockModule));

            Assert.Single(catalog.Modules);
            Assert.Equal("MockModule", catalog.Modules.First().ModuleName);
        }

        [Fact]
        public void CanAddGroups()
        {
            var catalog = new ModuleCatalog();

            ModuleInfo moduleInfo = new ModuleInfo();
            ModuleInfoGroup group = new ModuleInfoGroup { moduleInfo };
            catalog.Items.Add(group);

            Assert.Single(catalog.Modules);
            Assert.Same(moduleInfo, catalog.Modules.ElementAt(0));
        }

        [Fact]
        public void ShouldAggregateGroupsAndLooseModuleInfos()
        {
            var catalog = new ModuleCatalog();
            ModuleInfo moduleInfo1 = new ModuleInfo();
            ModuleInfo moduleInfo2 = new ModuleInfo();
            ModuleInfo moduleInfo3 = new ModuleInfo();

            catalog.Items.Add(new ModuleInfoGroup() { moduleInfo1 });
            catalog.Items.Add(new ModuleInfoGroup() { moduleInfo2 });
            catalog.AddModule(moduleInfo3);

            Assert.Equal(3, catalog.Modules.Count());
            Assert.Contains(moduleInfo1, catalog.Modules);
            Assert.Contains(moduleInfo2, catalog.Modules);
            Assert.Contains(moduleInfo3, catalog.Modules);
        }

        [Fact]
        public void CompleteListWithDependenciesThrowsWithNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var catalog = new ModuleCatalog();
                catalog.CompleteListWithDependencies(null);
            });

        }

        [Fact]
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
                Assert.IsType<ModularityException>(ex);
                Assert.Equal("ModuleB", ((ModularityException)ex).ModuleName);

                return;
            }

            //Assert.Fail("Exception not thrown.");
        }

        [Fact]
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
                Assert.IsType<ModularityException>(ex);
                Assert.Equal("ModuleB", ((ModularityException)ex).ModuleName);

                return;
            }

            //Assert.Fail("Exception not thrown.");
        }

        [Fact]
        public void ShouldRevalidateWhenAddingNewModuleIfValidated()
        {
            var testableCatalog = new TestableModuleCatalog();
            testableCatalog.Items.Add(new ModuleInfoGroup() { CreateModuleInfo("ModuleA") });
            testableCatalog.Validate();
            testableCatalog.Items.Add(new ModuleInfoGroup() { CreateModuleInfo("ModuleB") });
            Assert.True(testableCatalog.ValidateCalled);
        }

        [Fact]
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

            Assert.Single(moduleBDependencies);
            Assert.Equal(moduleA, moduleBDependencies.First());

        }

        [Fact]
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
                Assert.IsType<ModularityException>(ex);
                Assert.Equal("ModuleB", ((ModularityException)ex).ModuleName);

                return;
            }

            //Assert.Fail("Exception not thrown.");
        }

        [Fact]
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

            Assert.Equal(5, dependantModules.Count);
            Assert.True(dependantModules.IndexOf(moduleA) < dependantModules.IndexOf(moduleB));
            Assert.True(dependantModules.IndexOf(moduleB) < dependantModules.IndexOf(moduleC));
            Assert.True(dependantModules.IndexOf(moduleC) < dependantModules.IndexOf(moduleD));
            Assert.True(dependantModules.IndexOf(moduleC) < dependantModules.IndexOf(moduleX));
        }

        [Fact]
        public void CanLoadCatalogFromXaml()
        {
            Stream stream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "Prism.Wpf.Tests.Modularity.ModuleCatalogXaml.SimpleModuleCatalog.xaml");

            var catalog = ModuleCatalog.CreateFromXaml(stream);
            Assert.NotNull(catalog);

            Assert.Equal(4, catalog.Modules.Count());
        }


        [Fact]
        public void ShouldLoadAndValidateOnInitialize()
        {
            var catalog = new TestableModuleCatalog();

            var testableCatalog = new TestableModuleCatalog();
            Assert.False(testableCatalog.LoadCalled);
            Assert.False(testableCatalog.ValidateCalled);

            testableCatalog.Initialize();
            Assert.True(testableCatalog.LoadCalled);
            Assert.True(testableCatalog.ValidateCalled);
            Assert.True(testableCatalog.LoadCalledFirst);
        }

        [Fact]
        public void ShouldNotLoadAgainIfInitializedCalledMoreThanOnce()
        {
            var catalog = new TestableModuleCatalog();

            var testableCatalog = new TestableModuleCatalog();
            Assert.False(testableCatalog.LoadCalled);
            Assert.False(testableCatalog.ValidateCalled);

            testableCatalog.Initialize();
            Assert.Equal<int>(1, testableCatalog.LoadCalledCount);
            testableCatalog.Initialize();
            Assert.Equal<int>(1, testableCatalog.LoadCalledCount);
        }

        [Fact]
        public void ShouldNotLoadAgainDuringInitialize()
        {
            var catalog = new TestableModuleCatalog();

            var testableCatalog = new TestableModuleCatalog();
            Assert.False(testableCatalog.LoadCalled);
            Assert.False(testableCatalog.ValidateCalled);

            testableCatalog.Load();
            Assert.Equal<int>(1, testableCatalog.LoadCalledCount);
            testableCatalog.Initialize();
            Assert.Equal<int>(1, testableCatalog.LoadCalledCount);
        }


        [Fact]
        public void ShouldAllowLoadToBeInvokedTwice()
        {
            var catalog = new TestableModuleCatalog();

            var testableCatalog = new TestableModuleCatalog();
            testableCatalog.Load();
            Assert.Equal<int>(1, testableCatalog.LoadCalledCount);
            testableCatalog.Load();
            Assert.Equal<int>(2, testableCatalog.LoadCalledCount);
        }

        [Fact]
        public void CanAddModule1()
        {
            ModuleCatalog catalog = new ModuleCatalog();

            catalog.AddModule("Module", "ModuleType", InitializationMode.OnDemand, "DependsOn1", "DependsOn2");

            Assert.Single(catalog.Modules);
            Assert.Equal("Module", catalog.Modules.First().ModuleName);
            Assert.Equal("ModuleType", catalog.Modules.First().ModuleType);
            Assert.Equal(InitializationMode.OnDemand, catalog.Modules.First().InitializationMode);
            Assert.Equal(2, catalog.Modules.First().DependsOn.Count);
            Assert.Equal("DependsOn1", catalog.Modules.First().DependsOn[0]);
            Assert.Equal("DependsOn2", catalog.Modules.First().DependsOn[1]);

        }

        [Fact]
        public void CanAddModule2()
        {
            ModuleCatalog catalog = new ModuleCatalog();

            catalog.AddModule("Module", "ModuleType", "DependsOn1", "DependsOn2");

            Assert.Single(catalog.Modules);
            Assert.Equal("Module", catalog.Modules.First().ModuleName);
            Assert.Equal("ModuleType", catalog.Modules.First().ModuleType);
            Assert.Equal(InitializationMode.WhenAvailable, catalog.Modules.First().InitializationMode);
            Assert.Equal(2, catalog.Modules.First().DependsOn.Count);
            Assert.Equal("DependsOn1", catalog.Modules.First().DependsOn[0]);
            Assert.Equal("DependsOn2", catalog.Modules.First().DependsOn[1]);

        }
        [Fact]
        public void CanAddModule3()
        {
            ModuleCatalog catalog = new ModuleCatalog();

            catalog.AddModule(typeof(MockModule), InitializationMode.OnDemand, "DependsOn1", "DependsOn2");

            Assert.Single(catalog.Modules);
            Assert.Equal("MockModule", catalog.Modules.First().ModuleName);
            Assert.Equal(typeof(MockModule).AssemblyQualifiedName, catalog.Modules.First().ModuleType);
            Assert.Equal(InitializationMode.OnDemand, catalog.Modules.First().InitializationMode);
            Assert.Equal(2, catalog.Modules.First().DependsOn.Count);
            Assert.Equal("DependsOn1", catalog.Modules.First().DependsOn[0]);
            Assert.Equal("DependsOn2", catalog.Modules.First().DependsOn[1]);

        }

        [Fact]
        public void CanAddModule4()
        {
            ModuleCatalog catalog = new ModuleCatalog();

            catalog.AddModule(typeof(MockModule), "DependsOn1", "DependsOn2");

            Assert.Single(catalog.Modules);
            Assert.Equal("MockModule", catalog.Modules.First().ModuleName);
            Assert.Equal(typeof(MockModule).AssemblyQualifiedName, catalog.Modules.First().ModuleType);
            Assert.Equal(InitializationMode.WhenAvailable, catalog.Modules.First().InitializationMode);
            Assert.Equal(2, catalog.Modules.First().DependsOn.Count);
            Assert.Equal("DependsOn1", catalog.Modules.First().DependsOn[0]);
            Assert.Equal("DependsOn2", catalog.Modules.First().DependsOn[1]);

        }

        [Fact]
        public void CanAddGroup()
        {
            ModuleCatalog catalog = new ModuleCatalog();

            catalog.Items.Add(new ModuleInfoGroup());

            catalog.AddGroup(InitializationMode.OnDemand, "Ref1",
                             new ModuleInfo("M1", "T1"),
                             new ModuleInfo("M2", "T2", "M1"));

            Assert.Equal(2, catalog.Modules.Count());

            var module1 = catalog.Modules.First();
            var module2 = catalog.Modules.Skip(1).First();


            Assert.Equal("M1", module1.ModuleName);
            Assert.Equal("T1", module1.ModuleType);
            Assert.Equal("Ref1", module1.Ref);
            Assert.Equal(InitializationMode.OnDemand, module1.InitializationMode);

            Assert.Equal("M2", module2.ModuleName);
            Assert.Equal("T2", module2.ModuleType);
            Assert.Equal("Ref1", module2.Ref);
            Assert.Equal(InitializationMode.OnDemand, module2.InitializationMode);
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
