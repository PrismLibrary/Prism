namespace Prism.Composition.Windows.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Models;

    [TestClass]
    public class EnsureThatMefWorksAsNormalTests
    {
        [TestMethod]
        public void EnsureThatMefExportNormally()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            var mefOnly1Model = unityContainer.Resolve<IMefOnly1Model>();

            Assert.IsInstanceOfType(mefOnly1Model, typeof(MefOnly1Model));

            Assert.AreEqual(1, mefOnly1Model.ActiveInstances);

            mefOnly1Model.ActiveInstances = 0;
        }

        [TestMethod]
        public void EnsureThatMefExportNormallyWhenAskedForOneOfMany()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            var mefOnly1Model = unityContainer.Resolve<IMefOnlyModels>();

            Assert.IsInstanceOfType(mefOnly1Model, typeof(MefOnly1Model));

            Assert.AreEqual(1, mefOnly1Model.ActiveInstances);

            mefOnly1Model.ActiveInstances = 0;
        }

        [TestMethod]
        public void EnsureThatMefExportNormallyWhenAskedForOneOfManyWithTag()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            var mefOnly2Model = unityContainer.Resolve<IMefOnlyModels>("model2");

            Assert.IsInstanceOfType(mefOnly2Model, typeof(MefOnly2Model));

            Assert.AreEqual(1, mefOnly2Model.ActiveInstances);

            mefOnly2Model.ActiveInstances = 0;
        }

        [TestMethod]
        public void EnsureThatMefExportsNormallyAsIEnumerableType()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            var mefOnlyModels = unityContainer.Resolve<IEnumerable<IMefOnlyModels>>();

            var mefOnly1Model = mefOnlyModels.ElementAt(0);

            var mefOnly2Model = mefOnlyModels.ElementAt(1);

            Assert.IsInstanceOfType(mefOnly1Model, typeof(MefOnly1Model));

            Assert.IsInstanceOfType(mefOnly2Model, typeof(MefOnly2Model));

            Assert.AreEqual(1, mefOnly1Model.ActiveInstances);

            Assert.AreEqual(1, mefOnly2Model.ActiveInstances);

            mefOnly1Model.ActiveInstances = 0;

            mefOnly2Model.ActiveInstances = 0;
        }

        [TestMethod]
        public void EnsureThatMefExportsNormallyWithResolveAll()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            var mefOnlyModels = unityContainer.ResolveAll<IMefOnlyModels>();

            var mefOnly1Model = mefOnlyModels.ElementAt(0);

            var mefOnly2Model = mefOnlyModels.ElementAt(1);

            Assert.IsInstanceOfType(mefOnly1Model, typeof(MefOnly1Model));

            Assert.IsInstanceOfType(mefOnly2Model, typeof(MefOnly2Model));

            Assert.AreEqual(1, mefOnly1Model.ActiveInstances);

            Assert.AreEqual(1, mefOnly2Model.ActiveInstances);

            mefOnly1Model.ActiveInstances = 0;

            mefOnly2Model.ActiveInstances = 0;
        }

        [TestMethod]
        public void EnsureThatMefSatisfyImportsNormally()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            var mefOnlyWithMefOnlyDependenciesModel = unityContainer.Resolve<IMefOnlyWithMefOnlyDependenciesModel>();

            var mefOnly1Model = mefOnlyWithMefOnlyDependenciesModel.MefOnly1Model;

            var mefOnlyModels = mefOnlyWithMefOnlyDependenciesModel.MefOnlyModels;

            Assert.IsNotNull(mefOnly1Model);

            Assert.IsNotNull(mefOnlyModels);

            Assert.IsInstanceOfType(mefOnly1Model, typeof(MefOnly1Model));

            Assert.IsInstanceOfType(mefOnlyModels, typeof(IEnumerable<IMefOnlyModels>));

            Assert.AreEqual(2, mefOnly1Model.ActiveInstances);

            Assert.AreEqual(1, mefOnlyModels.ElementAt(1).ActiveInstances);

            mefOnly1Model.ActiveInstances = 0;

            mefOnlyModels.ElementAt(1).ActiveInstances = 0;
        }
    }
}