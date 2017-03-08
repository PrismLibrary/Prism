

using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Modularity;

namespace Prism.Wpf.Tests.Modularity
{
    [TestClass]
    public class FileModuleTypeLoaderFixture
    {
        [TestMethod]
        public void CanRetrieveModule()
        {
            var assemblyResolver = new MockAssemblyResolver();
            var retriever = new FileModuleTypeLoader(assemblyResolver);
            string assembly = CompilerHelper.GenerateDynamicModule("FileModuleA", null);
            string assemblyRef = "file://" + assembly;
            var fileModuleInfo = CreateModuleInfo(assemblyRef, "TestModules.FileModuleAClass", "ModuleA", true, null);

            bool loadCompleted = false;
            retriever.LoadModuleCompleted += delegate(object sender, LoadModuleCompletedEventArgs e)
            {
                loadCompleted = true;
            };

            retriever.LoadModuleType(fileModuleInfo);

            Assert.IsTrue(loadCompleted);
            Assert.AreEqual(assemblyRef, assemblyResolver.LoadAssemblyFromArgument);
        }

        [TestMethod]
        public void ShouldReturnErrorToCallback()
        {
            var assemblyResolver = new MockAssemblyResolver();
            var retriever = new FileModuleTypeLoader(assemblyResolver);
            var fileModuleInfo = CreateModuleInfo("NonExistentFile.dll", "NonExistentModule", "NonExistent", true, null);

            assemblyResolver.ThrowOnLoadAssemblyFrom = true;
            Exception resultException = null;

            bool loadCompleted = false;
            retriever.LoadModuleCompleted += delegate(object sender, LoadModuleCompletedEventArgs e)
            {
                loadCompleted = true;
                resultException = e.Error;
            };

            retriever.LoadModuleType(fileModuleInfo);

            Assert.IsTrue(loadCompleted);
            Assert.IsNotNull(resultException);
        }

        [TestMethod]
        public void CanRetrieveWithCorrectRef()
        {
            var retriever = new FileModuleTypeLoader();
            var moduleInfo = new ModuleInfo() { Ref = "file://somefile" };

            Assert.IsTrue(retriever.CanLoadModuleType(moduleInfo));
        }

        [TestMethod]
        public void CannotRetrieveWithIncorrectRef()
        {
            var retriever = new FileModuleTypeLoader();
            var moduleInfo = new ModuleInfo() { Ref = "NotForLocalRetrieval" };

            Assert.IsFalse(retriever.CanLoadModuleType(moduleInfo));
        }

        
        [TestMethod]
        public void FileModuleTypeLoaderCanBeDisposed()
        {
            var typeLoader = new FileModuleTypeLoader();
            var disposable = typeLoader as IDisposable;

            Assert.IsNotNull(disposable);
        }

        [TestMethod]
        public void FileModuleTypeLoaderDisposeNukesAssemblyResolver()
        {
            Mock<IAssemblyResolver> mockResolver = new Mock<IAssemblyResolver>();
            var disposableMockResolver = mockResolver.As<IDisposable>();
            disposableMockResolver.Setup(resolver => resolver.Dispose());

            var typeLoader = new FileModuleTypeLoader(mockResolver.Object);
            
            typeLoader.Dispose();

            disposableMockResolver.Verify(resolver => resolver.Dispose(), Times.Once());
        }

        [TestMethod]
        public void FileModuleTypeLoaderDisposeDoesNotThrowWithNonDisposableAssemblyResolver()
        {
            Mock<IAssemblyResolver> mockResolver = new Mock<IAssemblyResolver>();
            var typeLoader = new FileModuleTypeLoader(mockResolver.Object);
            try
            {
                typeLoader.Dispose();
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        private static ModuleInfo CreateModuleInfo(string assemblyFile, string moduleType, string moduleName, bool startupLoaded, params string[] dependsOn)
        {
            ModuleInfo moduleInfo = new ModuleInfo(moduleName, moduleType)
            {
                InitializationMode = startupLoaded ? InitializationMode.WhenAvailable : InitializationMode.OnDemand,
                Ref = assemblyFile,
            };
            if (dependsOn != null)
            {
                moduleInfo.DependsOn.AddRange(dependsOn);
            }

            return moduleInfo;
        }
    }

    internal class MockAssemblyResolver : IAssemblyResolver
    {
        public string LoadAssemblyFromArgument;
        public bool ThrowOnLoadAssemblyFrom;

        public void LoadAssemblyFrom(string assemblyFilePath)
        {
            LoadAssemblyFromArgument = assemblyFilePath;
            if (ThrowOnLoadAssemblyFrom)
                throw new Exception();
        }
    }
}
