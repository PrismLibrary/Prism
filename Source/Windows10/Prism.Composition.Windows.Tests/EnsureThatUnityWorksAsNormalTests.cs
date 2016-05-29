namespace Prism.Composition.Windows.Tests
{
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Models;

    [TestClass]
    public class EnsureThatUnityWorksAsNormalTests
    {
        [TestMethod]
        public void EnsureThatUnityResolveNormally()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            unityContainer.RegisterType<IUnityOnly1Model, UnityOnly1Model>();

            var unityOnly1Model = unityContainer.Resolve<IUnityOnly1Model>();

            Assert.AreEqual(1, unityOnly1Model.ActiveInstances);

            unityOnly1Model.ActiveInstances = 0;
        }

        [TestMethod]
        public void EnsureThatUnityResolveAllNormally()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssemblyConfiguration(new AssemblyConfiguration { Assembly = this.GetType().GetTypeInfo().Assembly });

            unityContainer.RegisterType<IUnityOnly1Model, UnityOnly1Model>("1");

            var unityOnly1Models = unityContainer.ResolveAll<IUnityOnly1Model>();

            Assert.AreEqual(1, unityOnly1Models.Count());

            Assert.AreEqual(1, unityOnly1Models.First().ActiveInstances);

            unityOnly1Models.First().ActiveInstances = 0;
        }
    }
}