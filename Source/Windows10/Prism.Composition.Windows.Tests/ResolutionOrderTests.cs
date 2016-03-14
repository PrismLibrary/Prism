using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Composition;
using System.Composition.Hosting;
using System.Reflection;

namespace Prism.Composition.Windows.Tests
{
    [TestClass]
    public class ResolutionOrderTests
    {
        [TestMethod]
        public void UnityRegisteredComponentsTakePrecedenceOverMefRegisteredComponentsIfQueryingForASingleComponentRegisteredInBothContainers()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            unityContainer.RegisterType<ISingleton, Singleton>(new ContainerControlledLifetimeManager());

            Singleton.Count = 0;

            Assert.AreEqual(0, Singleton.Count);

            var singleton = unityContainer.Resolve<ISingleton>();

            Assert.IsNotNull(singleton);

            Assert.AreEqual(1, Singleton.Count);

            var mef = unityContainer.Resolve<CompositionHost>();

            var mefSingleton = mef.GetExport<ISingleton>();

            var instance = mefSingleton;
            
            Assert.AreEqual(1, Singleton.Count);

            Assert.AreSame(singleton, instance);
            
        }

        [TestMethod]
        public void UnityRegisteredComponentsTakePrecedenceOverMefRegisteredComponentsIfQueryingForMultipleComponentsRegisteredInBothContainers()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            unityContainer.RegisterType<ISingleton, Singleton>(new ContainerControlledLifetimeManager());

            Singleton.Count = 0;

            Assert.AreEqual(0, Singleton.Count);

            var mef = unityContainer.Resolve<CompositionHost>();

            mef.GetExports<ISingleton>();

            Assert.AreEqual(1, Singleton.Count);
        }
    }

    [Export(typeof(ISingleton))]
    [Shared]
    public class Singleton : ISingleton
    {
        public static int Count;

        public Singleton()
        {
            Count++;
        }
    }

    public interface ISingleton { }
}
