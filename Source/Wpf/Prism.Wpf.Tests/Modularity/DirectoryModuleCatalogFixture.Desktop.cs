

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Modularity;

namespace Prism.Wpf.Tests.Modularity
{
    [TestClass]
    public class DirectoryModuleCatalogFixture
    {
        private const string ModulesDirectory1 = @".\DynamicModules\MocksModules1";
        private const string ModulesDirectory2 = @".\DynamicModules\AttributedModules";
        private const string ModulesDirectory3 = @".\DynamicModules\DependantModules";
        private const string ModulesDirectory4 = @".\DynamicModules\MocksModules2";
        private const string ModulesDirectory5 = @".\DynamicModules\ModulesMainDomain\";
        private const string ModulesDirectory6 = @".\DynamicModules\Special char #";
        private const string InvalidModulesDirectory = @".\Modularity";

        public DirectoryModuleCatalogFixture()
        {
        }

        [TestInitialize]
        [TestCleanup]
        public void CleanUpDirectories()
        {
            CompilerHelper.CleanUpDirectory(ModulesDirectory1);
            CompilerHelper.CleanUpDirectory(ModulesDirectory2);
            CompilerHelper.CleanUpDirectory(ModulesDirectory3);
            CompilerHelper.CleanUpDirectory(ModulesDirectory4);
            CompilerHelper.CleanUpDirectory(ModulesDirectory5);
            CompilerHelper.CleanUpDirectory(InvalidModulesDirectory);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NullPathThrows()
        {
            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog();
            catalog.Load();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EmptyPathThrows()
        {
            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = string.Empty
            };
            catalog.Load();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NonExistentPathThrows()
        {
            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = "NonExistentPath"
            };
            catalog.Load();
        }

        [TestMethod]
        public void ShouldReturnAListOfModuleInfo()
        {
            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockModuleA.cs",
                                       ModulesDirectory1 + @"\MockModuleA.dll");


            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory1
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.IsNotNull(modules);
            Assert.AreEqual(1, modules.Length);
            Assert.IsNotNull(modules[0].Ref);
            StringAssert.StartsWith(modules[0].Ref, "file://");
            Assert.IsTrue(modules[0].Ref.Contains(@"MockModuleA.dll"));
            Assert.IsNotNull(modules[0].ModuleType);
            StringAssert.Contains(modules[0].ModuleType, "Prism.Wpf.Tests.Mocks.Modules.MockModuleA");
        }

        [TestMethod]
        public void ShouldCorrectlyEscapeRef()
        {
            string assemblyPath = ModulesDirectory6 + @"\Mock Module #.dll";
            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockModuleA.cs", assemblyPath);
            string fullAssemblyPath = Path.GetFullPath(assemblyPath);

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory6
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.IsNotNull(modules);
            Assert.AreEqual(1, modules.Length);
            Assert.IsNotNull(modules[0].Ref);

            string moduleRef = modules[0].Ref;
            // = new Uri(moduleRef);
            Assert.IsTrue(Uri.TryCreate(moduleRef, UriKind.Absolute, out Uri moduleUri));

            Assert.AreEqual(fullAssemblyPath, moduleUri.LocalPath);
        }

        [TestMethod]
        [DeploymentItem(@"Modularity\NotAValidDotNetDll.txt.dll", @".\Modularity")]
        public void ShouldNotThrowWithNonValidDotNetAssembly()
        {
            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = InvalidModulesDirectory
            };
            try
            {
                catalog.Load();
            }
            catch (Exception)
            {
                Assert.Fail("Should not have thrown.");
            }
            
            var modules = catalog.Modules.ToArray();
            Assert.IsNotNull(modules);
            Assert.AreEqual(0, modules.Length);
        }

        [TestMethod]
        [DeploymentItem(@"Modularity\NotAValidDotNetDll.txt.dll", InvalidModulesDirectory)]
        public void LoadsValidAssembliesWhenInvalidDllsArePresent()
        {
            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockModuleA.cs",
                                       InvalidModulesDirectory + @"\MockModuleA.dll");

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = InvalidModulesDirectory
            };
            try
            {
                catalog.Load();
            }
            catch (Exception)
            {
                Assert.Fail("Should not have thrown.");
            }

            var modules = catalog.Modules.ToArray();

            Assert.IsNotNull(modules);
            Assert.AreEqual(1, modules.Length);
            Assert.IsNotNull(modules[0].Ref);
            StringAssert.StartsWith(modules[0].Ref, "file://");
            Assert.IsTrue(modules[0].Ref.Contains(@"MockModuleA.dll"));
            Assert.IsNotNull(modules[0].ModuleType);
            StringAssert.Contains(modules[0].ModuleType, "Prism.Wpf.Tests.Mocks.Modules.MockModuleA");
        }

        [TestMethod]
        public void ShouldNotThrowWithLoadFromByteAssemblies()
        {
            CompilerHelper.CleanUpDirectory(@".\CompileOutput\");
            CompilerHelper.CleanUpDirectory(@".\IgnoreLoadFromByteAssembliesTestDir\");
            var results = CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockModuleA.cs",
                                                     @".\CompileOutput\MockModuleA.dll");

            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockAttributedModule.cs",
                                       @".\IgnoreLoadFromByteAssembliesTestDir\MockAttributedModule.dll");

            string path = @".\IgnoreLoadFromByteAssembliesTestDir";

            AppDomain testDomain = null;
            try
            {
                testDomain = CreateAppDomain();
                RemoteDirectoryLookupCatalog remoteEnum = CreateRemoteDirectoryModuleCatalogInAppDomain(testDomain);

                remoteEnum.LoadDynamicEmittedModule();

                remoteEnum.LoadAssembliesByByte(@".\CompileOutput\MockModuleA.dll");

                var infos = remoteEnum.DoEnumeration(path);

                Assert.IsNotNull(
                    infos.FirstOrDefault(x => x.ModuleType.IndexOf("Prism.Wpf.Tests.Mocks.Modules.MockAttributedModule") >= 0)
                    );
            }
            finally
            {
                if (testDomain != null)
                    AppDomain.Unload(testDomain);
            }
        }

        [TestMethod]
        public void ShouldGetModuleNameFromAttribute()
        {
            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockAttributedModule.cs",
                                       ModulesDirectory2 + @"\MockAttributedModule.dll");


            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory2
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.AreEqual(1, modules.Length);
            Assert.AreEqual("TestModule", modules[0].ModuleName);
        }

        [TestMethod]
        public void ShouldGetDependantModulesFromAttribute()
        {
            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockDependencyModule.cs",
                                       ModulesDirectory3 + @"\DependencyModule.dll");

            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockDependantModule.cs",
                                       ModulesDirectory3 + @"\DependantModule.dll");

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory3
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.AreEqual(2, modules.Length);
            var dependantModule = modules.First(module => module.ModuleName == "DependantModule");
            var dependencyModule = modules.First(module => module.ModuleName == "DependencyModule");
            Assert.IsNotNull(dependantModule);
            Assert.IsNotNull(dependencyModule);
            Assert.IsNotNull(dependantModule.DependsOn);
            Assert.AreEqual(1, dependantModule.DependsOn.Count);
            Assert.AreEqual(dependencyModule.ModuleName, dependantModule.DependsOn[0]);
        }

        [TestMethod]
        public void UseClassNameAsModuleNameWhenNotSpecifiedInAttribute()
        {
            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockModuleA.cs",
                                       ModulesDirectory1 + @"\MockModuleA.dll");

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory1
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.IsNotNull(modules);
            Assert.AreEqual("MockModuleA", modules[0].ModuleName);
        }

        [TestMethod]
        public void ShouldDefaultInitializationModeToWhenAvailable()
        {
            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockModuleA.cs",
                                       ModulesDirectory1 + @"\MockModuleA.dll");

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory1
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.IsNotNull(modules);
            Assert.AreEqual(InitializationMode.WhenAvailable, modules[0].InitializationMode);
        }

        [TestMethod]
        public void ShouldGetOnDemandFromAttribute()
        {
            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockAttributedModule.cs",
                                       ModulesDirectory3 + @"\MockAttributedModule.dll");

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory3
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.AreEqual(1, modules.Length);
            Assert.AreEqual(InitializationMode.OnDemand, modules[0].InitializationMode);

        }

        [TestMethod]
        public void ShouldNotLoadAssembliesInCurrentAppDomain()
        {
            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockModuleA.cs",
                                       ModulesDirectory4 + @"\MockModuleA.dll");

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory4
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            // filtering out dynamic assemblies due to using a dynamic mocking framework.
            Assembly loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.IsDynamic)
                .Where(assembly => assembly.Location.Equals(modules[0].Ref, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
            Assert.IsNull(loadedAssembly);
        }

        [TestMethod]
        public void ShouldNotGetModuleInfoForAnAssemblyAlreadyLoadedInTheMainDomain()
        {
            var assemblyPath = Assembly.GetCallingAssembly().Location;
            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory5
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.AreEqual(0, modules.Length);
        }

        [TestMethod]
        public void ShouldLoadAssemblyEvenIfTheyAreReferencingEachOther()
        {
            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockModuleA.cs",
                                       ModulesDirectory4 + @"\MockModuleZZZ.dll");

            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockModuleReferencingOtherModule.cs",
                                       ModulesDirectory4 + @"\MockModuleReferencingOtherModule.dll", ModulesDirectory4 + @"\MockModuleZZZ.dll");

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory4
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.AreEqual(2, modules.Count());
        }
        //Disabled Warning	
        // 'System.Security.Policy.Evidence.Count' is obsolete: '
        // "Evidence should not be treated as an ICollection. Please use GetHostEnumerator and GetAssemblyEnumerator to 
        // iterate over the evidence to collect a count."'
#pragma warning disable 0618
        [TestMethod]
        public void CreateChildAppDomainHasParentEvidenceAndSetup()
        {
            TestableDirectoryModuleCatalog catalog = new TestableDirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory4
            };
            catalog.Load();
            Evidence parentEvidence = new Evidence();
            AppDomainSetup parentSetup = new AppDomainSetup
            {
                ApplicationName = "Test Parent"
            };
            AppDomain parentAppDomain = AppDomain.CreateDomain("Parent", parentEvidence, parentSetup);
            AppDomain childDomain = catalog.BuildChildDomain(parentAppDomain);

            Assert.AreEqual(parentEvidence.Count, childDomain.Evidence.Count);
            Assert.AreEqual("Test Parent", childDomain.SetupInformation.ApplicationName);
            Assert.AreNotEqual(AppDomain.CurrentDomain.Evidence.Count, childDomain.Evidence.Count);
            Assert.AreNotEqual(AppDomain.CurrentDomain.SetupInformation.ApplicationName, childDomain.SetupInformation.ApplicationName);
        }
#pragma warning restore 0618

        [TestMethod]
        public void ShouldLoadFilesEvenIfDynamicAssemblyExists()
        {
            CompilerHelper.CleanUpDirectory(@".\CompileOutput\");
            CompilerHelper.CleanUpDirectory(@".\IgnoreDynamicGeneratedFilesTestDir\");
            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockAttributedModule.cs",
                                       @".\IgnoreDynamicGeneratedFilesTestDir\MockAttributedModule.dll");

            string path = @".\IgnoreDynamicGeneratedFilesTestDir";

            AppDomain testDomain = null;
            try
            {
                testDomain = CreateAppDomain();
                RemoteDirectoryLookupCatalog remoteEnum = CreateRemoteDirectoryModuleCatalogInAppDomain(testDomain);

                remoteEnum.LoadDynamicEmittedModule();

                var infos = remoteEnum.DoEnumeration(path);

                Assert.IsNotNull(
                    infos.FirstOrDefault(x => x.ModuleType.IndexOf("Prism.Wpf.Tests.Mocks.Modules.MockAttributedModule") >= 0)
                    );
            }
            finally
            {
                if (testDomain != null)
                    AppDomain.Unload(testDomain);
            }
        }

        [TestMethod]
        public void ShouldLoadAssemblyEvenIfIsExposingTypesFromAnAssemblyInTheGac()
        {
            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockExposingTypeFromGacAssemblyModule.cs",
                                       ModulesDirectory4 + @"\MockExposingTypeFromGacAssemblyModule.dll", @"System.Transactions.dll");

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory4
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.AreEqual(1, modules.Count());
        }

        [TestMethod]
        public void ShouldNotFailWhenAlreadyLoadedAssembliesAreAlsoFoundOnTargetDirectory()
        {
            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockModuleA.cs",
                                       ModulesDirectory1 + @"\MockModuleA.dll");

            string filename = typeof(DirectoryModuleCatalog).Assembly.Location;
            string destinationFileName = Path.Combine(ModulesDirectory1, Path.GetFileName(filename));
            File.Copy(filename, destinationFileName);

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory1
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();
            Assert.AreEqual(1, modules.Length);
        }

        [TestMethod]
        public void ShouldIgnoreAbstractClassesThatImplementIModule()
        {
            CompilerHelper.CleanUpDirectory(ModulesDirectory1);
            CompilerHelper.CompileFile(@"Prism.Wpf.Tests.Mocks.Modules.MockAbstractModule.cs",
                                     ModulesDirectory1 + @"\MockAbstractModule.dll");

            string filename = typeof(DirectoryModuleCatalog).Assembly.Location;
            string destinationFileName = Path.Combine(ModulesDirectory1, Path.GetFileName(filename));
            File.Copy(filename, destinationFileName);

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory1
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();
            Assert.AreEqual(1, modules.Length);
            Assert.AreEqual("MockInheritingModule", modules[0].ModuleName);

            CompilerHelper.CleanUpDirectory(ModulesDirectory1);
        }




        private AppDomain CreateAppDomain()
        {
            Evidence evidence = AppDomain.CurrentDomain.Evidence;
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;

            return AppDomain.CreateDomain("TestDomain", evidence, setup);
        }

        private RemoteDirectoryLookupCatalog CreateRemoteDirectoryModuleCatalogInAppDomain(AppDomain testDomain)
        {
            RemoteDirectoryLookupCatalog remoteEnum;
            Type remoteEnumType = typeof(RemoteDirectoryLookupCatalog);

            remoteEnum = (RemoteDirectoryLookupCatalog)testDomain.CreateInstanceFrom(
                                               remoteEnumType.Assembly.Location, remoteEnumType.FullName).Unwrap();
            return remoteEnum;
        }

        private class TestableDirectoryModuleCatalog : DirectoryModuleCatalog
        {
            public new AppDomain BuildChildDomain(AppDomain currentDomain)
            {
                return base.BuildChildDomain(currentDomain);
            }
        }


        private class RemoteDirectoryLookupCatalog : MarshalByRefObject
        {

            public void LoadAssembliesByByte(string assemblyPath)
            {
                byte[] assemblyBytes = File.ReadAllBytes(assemblyPath);
                AppDomain.CurrentDomain.Load(assemblyBytes);
            }

            public IModuleInfo[] DoEnumeration(string path)
            {
                DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
                {
                    ModulePath = path
                };
                catalog.Load();
                return catalog.Modules.ToArray();
            }

            public void LoadDynamicEmittedModule()
            {
                // create a dynamic assembly and module 
                AssemblyName assemblyName = new AssemblyName
                {
                    Name = "DynamicBuiltAssembly"
                };
                AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
                ModuleBuilder module = assemblyBuilder.DefineDynamicModule("DynamicBuiltAssembly.dll");

                // create a new type
                TypeBuilder typeBuilder = module.DefineType("DynamicBuiltType", TypeAttributes.Public | TypeAttributes.Class);

                // Create the type
                Type helloWorldType = typeBuilder.CreateType();

            }
        }
    }
}
