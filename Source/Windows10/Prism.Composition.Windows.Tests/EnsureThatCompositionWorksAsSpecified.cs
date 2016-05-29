namespace Prism.Composition.Windows.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Extensions;
    using Models;

    [TestClass]
    public class EnsureThatCompositionWorksAsSpecified
    {
        [TestMethod]
        public void EnsureThatMefCanFindAUnityDependency()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            unityContainer.RegisterType<IUnityOnly1Model, UnityOnly1Model>();

            var mefWithUnityDependencyModel = unityContainer.Resolve<IMefWithUnityDependencyModel>();

            Assert.IsInstanceOfType(mefWithUnityDependencyModel, typeof(IMefWithUnityDependencyModel));

            Assert.IsNotNull(mefWithUnityDependencyModel.UnityOnly1Model);

            mefWithUnityDependencyModel.UnityOnly1Model.ActiveInstances = 0;
        }

        [TestMethod]
        public void EnsureThatMefCanFindASpecificUnityDependency()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            unityContainer.RegisterType<IUnityOnlyModels, UnityOnly1Model>();

            unityContainer.RegisterType<IUnityOnlyModels, UnityOnly2Model>("Model2");

            var mefWithSpecificUnityDependencyModel = unityContainer.Resolve<IMefWithSpecificUnityDependencyModel>();

            Assert.IsInstanceOfType(mefWithSpecificUnityDependencyModel, typeof(IMefWithSpecificUnityDependencyModel));

            Assert.IsNotNull(mefWithSpecificUnityDependencyModel.UnityOnlyModel);

            Assert.IsInstanceOfType(mefWithSpecificUnityDependencyModel.UnityOnlyModel, typeof(UnityOnly2Model));

            mefWithSpecificUnityDependencyModel.UnityOnlyModel.ActiveInstances = 0;
        }

        [TestMethod]
        public void EnsureThatMefCanResolveAUnityDependencyWithWrongContractName()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            unityContainer.RegisterType<IUnityOnlyModels, UnityOnly1Model>();

            unityContainer.RegisterType<IUnityOnlyModels, UnityOnly2Model>("Model99");

            var mefWithSpecificUnityDependencyModel = unityContainer.Resolve<IMefWithSpecificUnityDependencyModel>();

            Assert.IsInstanceOfType(mefWithSpecificUnityDependencyModel, typeof(IMefWithSpecificUnityDependencyModel));

            Assert.IsNotNull(mefWithSpecificUnityDependencyModel.UnityOnlyModel);

            Assert.IsInstanceOfType(mefWithSpecificUnityDependencyModel.UnityOnlyModel, typeof(IUnityOnlyModels));

            mefWithSpecificUnityDependencyModel.UnityOnlyModel.ActiveInstances = 0;
        }

        [TestMethod]
        public void EnsureThatMefCanFindAllUnityDependencies()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            unityContainer.RegisterType<IUnityOnlyModels, UnityOnly1Model>();

            unityContainer.RegisterType<IUnityOnlyModels, UnityOnly2Model>("Model2");

            var mefWithUnityDependenciesModel = unityContainer.Resolve<IMefWithUnityDependenciesModel>();

            Assert.IsInstanceOfType(mefWithUnityDependenciesModel, typeof(IMefWithUnityDependenciesModel));

            Assert.IsNotNull(mefWithUnityDependenciesModel.UnityOnlyModels);

            Assert.AreEqual(1, mefWithUnityDependenciesModel.UnityOnlyModels.First().ActiveInstances);

            Assert.AreEqual(1, mefWithUnityDependenciesModel.UnityOnlyModels.Last().ActiveInstances);

            mefWithUnityDependenciesModel.UnityOnlyModels.First().ActiveInstances = 0;

            mefWithUnityDependenciesModel.UnityOnlyModels.Last().ActiveInstances = 0;
        }

        [TestMethod]
        public void EnsureThatUnityCanResolveAllMixedDependencies()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            unityContainer.RegisterType<IMixedModels, Mixed1WithNoExport>();

            unityContainer.RegisterType<IMixedModels, Mixed2WithNoExport>("Model2");

            var mixedModels = unityContainer.ResolveAll<IMixedModels>();

            Assert.IsInstanceOfType(mixedModels.ElementAt(0), typeof(Mixed1WithNoExport));

            Assert.AreEqual(1, mixedModels.ElementAt(0).ActiveInstances);

            Assert.IsInstanceOfType(mixedModels.ElementAt(1), typeof(Mixed2WithNoExport));

            Assert.AreEqual(1, mixedModels.ElementAt(1).ActiveInstances);

            Assert.IsInstanceOfType(mixedModels.ElementAt(2), typeof(Mixed1WithExport));

            Assert.AreEqual(1, mixedModels.ElementAt(2).ActiveInstances);

            mixedModels.ElementAt(0).ActiveInstances = 0;

            mixedModels.ElementAt(1).ActiveInstances = 0;

            mixedModels.ElementAt(2).ActiveInstances = 0;
        }

        [TestMethod]
        public void EnsureThatMefCanFindAllMixedDependencies()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            unityContainer.RegisterType<IMixedModels, Mixed1WithNoExport>();

            unityContainer.RegisterType<IMixedModels, Mixed2WithNoExport>("Model2");

            var mefWithMixedDependenciesModel = unityContainer.Resolve<IMefWithMixedDependenciesModel>();

            Assert.IsInstanceOfType(mefWithMixedDependenciesModel.MixedModels.ElementAt(0), typeof(Mixed1WithNoExport));

            Assert.AreEqual(1, mefWithMixedDependenciesModel.MixedModels.ElementAt(0).ActiveInstances);

            Assert.IsInstanceOfType(mefWithMixedDependenciesModel.MixedModels.ElementAt(1), typeof(Mixed2WithNoExport));

            Assert.AreEqual(1, mefWithMixedDependenciesModel.MixedModels.ElementAt(1).ActiveInstances);

            Assert.IsInstanceOfType(mefWithMixedDependenciesModel.MixedModels.ElementAt(2), typeof(Mixed1WithExport));

            Assert.AreEqual(1, mefWithMixedDependenciesModel.MixedModels.ElementAt(2).ActiveInstances);

            mefWithMixedDependenciesModel.MixedModels.ElementAt(0).ActiveInstances = 0;

            mefWithMixedDependenciesModel.MixedModels.ElementAt(1).ActiveInstances = 0;

            mefWithMixedDependenciesModel.MixedModels.ElementAt(2).ActiveInstances = 0;
        }

        [TestMethod]
        public void EnsureThatThereAreNoDifferencesBetweenResolveAllAndResolveIEnumerable()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            unityContainer.RegisterType<IMixedModels, Mixed1WithNoExport>();

            unityContainer.RegisterType<IMixedModels, Mixed2WithNoExport>("Model2");

            var mixedModelsFromResolveAll = unityContainer.ResolveAll<IMixedModels>();

            Assert.IsInstanceOfType(mixedModelsFromResolveAll.ElementAt(0), typeof(Mixed1WithNoExport));

            Assert.AreEqual(1, mixedModelsFromResolveAll.ElementAt(0).ActiveInstances);

            Assert.IsInstanceOfType(mixedModelsFromResolveAll.ElementAt(1), typeof(Mixed2WithNoExport));

            Assert.AreEqual(1, mixedModelsFromResolveAll.ElementAt(1).ActiveInstances);

            Assert.IsInstanceOfType(mixedModelsFromResolveAll.ElementAt(2), typeof(Mixed1WithExport));

            Assert.AreEqual(1, mixedModelsFromResolveAll.ElementAt(2).ActiveInstances);

            var mixedModelsFromResolve = unityContainer.Resolve<IEnumerable<IMixedModels>>();

            Assert.IsInstanceOfType(mixedModelsFromResolve.ElementAt(0), typeof(Mixed1WithNoExport));

            Assert.AreEqual(2, mixedModelsFromResolve.ElementAt(0).ActiveInstances);

            Assert.IsInstanceOfType(mixedModelsFromResolve.ElementAt(1), typeof(Mixed2WithNoExport));

            Assert.AreEqual(2, mixedModelsFromResolve.ElementAt(1).ActiveInstances);

            Assert.IsInstanceOfType(mixedModelsFromResolve.ElementAt(2), typeof(Mixed1WithExport));

            Assert.AreEqual(2, mixedModelsFromResolve.ElementAt(2).ActiveInstances);

            mixedModelsFromResolveAll.ElementAt(0).ActiveInstances = 0;

            mixedModelsFromResolveAll.ElementAt(1).ActiveInstances = 0;

            mixedModelsFromResolveAll.ElementAt(2).ActiveInstances = 0;

            mixedModelsFromResolve.ElementAt(0).ActiveInstances = 0;

            mixedModelsFromResolve.ElementAt(1).ActiveInstances = 0;

            mixedModelsFromResolve.ElementAt(2).ActiveInstances = 0;
        }

        [TestMethod]
        public void EnsureThatThereAreNoDifferencesBetweenMefAndUnityWhenResolvingWithEach()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            unityContainer.RegisterType<IMixedModels, Mixed1WithNoExport>();

            unityContainer.RegisterType<IMixedModels, Mixed2WithNoExport>("Model2");

            var mixedModelsFromResolveAll = unityContainer.ResolveAll<IMixedModels>();

            Assert.IsInstanceOfType(mixedModelsFromResolveAll.ElementAt(0), typeof(Mixed1WithNoExport));

            Assert.AreEqual(1, mixedModelsFromResolveAll.ElementAt(0).ActiveInstances);

            Assert.IsInstanceOfType(mixedModelsFromResolveAll.ElementAt(1), typeof(Mixed2WithNoExport));

            Assert.AreEqual(1, mixedModelsFromResolveAll.ElementAt(1).ActiveInstances);

            Assert.IsInstanceOfType(mixedModelsFromResolveAll.ElementAt(2), typeof(Mixed1WithExport));

            Assert.AreEqual(1, mixedModelsFromResolveAll.ElementAt(2).ActiveInstances);

            var compositionHost = unityContainer.Resolve<CompositionHost>();

            var mixedModelsFromExports = compositionHost.GetExports<IMixedModels>();

            Assert.IsInstanceOfType(mixedModelsFromExports.ElementAt(0), typeof(Mixed1WithNoExport));

            Assert.AreEqual(2, mixedModelsFromExports.ElementAt(0).ActiveInstances);

            Assert.IsInstanceOfType(mixedModelsFromExports.ElementAt(1), typeof(Mixed2WithNoExport));

            Assert.AreEqual(2, mixedModelsFromExports.ElementAt(1).ActiveInstances);

            Assert.IsInstanceOfType(mixedModelsFromExports.ElementAt(2), typeof(Mixed1WithExport));

            Assert.AreEqual(2, mixedModelsFromExports.ElementAt(2).ActiveInstances);

            mixedModelsFromResolveAll.ElementAt(0).ActiveInstances = 0;

            mixedModelsFromResolveAll.ElementAt(1).ActiveInstances = 0;

            mixedModelsFromResolveAll.ElementAt(2).ActiveInstances = 0;

            mixedModelsFromExports.ElementAt(0).ActiveInstances = 0;

            mixedModelsFromExports.ElementAt(1).ActiveInstances = 0;

            mixedModelsFromExports.ElementAt(2).ActiveInstances = 0;
        }

        [TestMethod]
        public void EnsureThatLazyIsResolvedRight()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            unityContainer.RegisterType<IMixedModels, Mixed1WithNoExport>();

            unityContainer.RegisterType<IMixedModels, Mixed2WithNoExport>("Model2");

            var lazyMefWithMixedDependenciesModel = unityContainer.Resolve<Lazy<IMefWithMixedDependenciesModel>>();

            Assert.IsInstanceOfType(lazyMefWithMixedDependenciesModel, typeof(Lazy<IMefWithMixedDependenciesModel>));

            var mefWithMixedDependenciesModel = lazyMefWithMixedDependenciesModel.Value;

            Assert.IsInstanceOfType(mefWithMixedDependenciesModel, typeof(IMefWithMixedDependenciesModel));

            mefWithMixedDependenciesModel.MixedModels.ElementAt(0).ActiveInstances = 0;

            mefWithMixedDependenciesModel.MixedModels.ElementAt(1).ActiveInstances = 0;

            mefWithMixedDependenciesModel.MixedModels.ElementAt(2).ActiveInstances = 0;
        }

        [TestMethod]
        public void EnsureThatLazyIsResolvedRightWhenAskedForMany()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            unityContainer.RegisterType<IMixedModels, Mixed1WithNoExport>();

            unityContainer.RegisterType<IMixedModels, Mixed2WithNoExport>("Model2");

            var mixedModelsFromResolveAll = unityContainer.ResolveAll<Lazy<IMixedModels>>();

            Assert.IsInstanceOfType(mixedModelsFromResolveAll, typeof(Lazy<IMixedModels>[]));

            foreach (var lazyMixedModel in mixedModelsFromResolveAll)
            {
                Assert.AreEqual(1, lazyMixedModel.Value.ActiveInstances);

                lazyMixedModel.Value.ActiveInstances = 0;
            }

            var mixedModelsFromResolve = unityContainer.Resolve<IEnumerable<Lazy<IMixedModels>>>();

            Assert.IsInstanceOfType(mixedModelsFromResolveAll, typeof(Lazy<IMixedModels>[]));

            foreach (var lazyMixedModel in mixedModelsFromResolve)
            {
                Assert.AreEqual(1, lazyMixedModel.Value.ActiveInstances);

                lazyMixedModel.Value.ActiveInstances = 0;
            }
        }
    }
}
