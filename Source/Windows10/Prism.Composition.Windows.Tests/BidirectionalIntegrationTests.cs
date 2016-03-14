using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Composition.Hosting;
using System.Reflection;

namespace Prism.Composition.Windows.Tests
{
    [TestClass]
    public class BidirectionalIntegrationTests
    {
        [TestMethod]
        public void UnityCanResolveMefComponentThatHasUnityDependenciesTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();

            var mefComponent = unityContainer.Resolve<IMefComponentWithUnityDependencies>();

            Assert.IsNotNull(mefComponent);

            Assert.AreEqual(typeof(MefComponent1), mefComponent.MefOnlyComponent.GetType());

            Assert.AreEqual(typeof(UnityOnlyComponent1), mefComponent.UnityOnlyComponent.GetType());
        }

        [TestMethod]
        public void UnityCanResolveMefComponentThatHasUnityDependenciesThatHaveMefDependenciesTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();

            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            var mefComponent = unityContainer.Resolve<IMefComponentWithUnityDependencies>("component2");

            Assert.IsNotNull(mefComponent);

            Assert.AreEqual(typeof(MefComponentWithUnityDependencies2), mefComponent.GetType());

            Assert.AreEqual(typeof(MefComponent1), mefComponent.MefOnlyComponent.GetType());

            Assert.AreEqual(typeof(UnityOnlyComponent1), mefComponent.UnityOnlyComponent.GetType());

            var mefComponentWithUnityDependencies2 = (MefComponentWithUnityDependencies2) mefComponent;

            Assert.AreEqual(typeof(UnityComponent1), mefComponentWithUnityDependencies2.MixedUnityMefComponent.GetType());

            Assert.AreEqual(typeof(MefComponent1), mefComponentWithUnityDependencies2.MixedUnityMefComponent.MefComponent.GetType());
        }

        [TestMethod]
        public void UnityCircularDependencyIsDetectedTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            unityContainer.RegisterType<UnityOnlyComponent1>();
            
            var unityOnlyComponent1 = unityContainer.Resolve<UnityOnlyComponent1>();

            Assert.IsNotNull(unityOnlyComponent1);
        }

        [TestMethod]
        public void UnityCanResolveUnityComponentThatHasUnityAndMefDependenciesTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();

            unityContainer.RegisterType<UnityMixedComponent>();

            var unityMixedComponent = unityContainer.Resolve<UnityMixedComponent>();

            Assert.IsNotNull(unityMixedComponent);

            Assert.AreEqual(typeof(UnityMixedComponent), unityMixedComponent.GetType());

            Assert.AreEqual(typeof(MefComponent1), unityMixedComponent.MefComponent.GetType());

            Assert.AreEqual(typeof(UnityOnlyComponent1), unityMixedComponent.UnityComponent.GetType());
        }

        
        [TestMethod]
        public void UnityContainerCanBeResolvedByMefTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            var compositionContainer1 = unityContainer.Resolve<CompositionHost>();

            var compositionContainer2 = unityContainer.Resolve<CompositionHost>();

            Assert.IsNotNull(compositionContainer1);

            Assert.IsNotNull(compositionContainer2);

            Assert.AreSame(compositionContainer1, compositionContainer2);

            var unityContainerFromMef1 = compositionContainer1.GetExport<IUnityContainer>();

            var unityContainerFromMef2 = compositionContainer1.GetExport<IUnityContainer>();
            
            Assert.IsNotNull(unityContainerFromMef1);

            Assert.IsNotNull(unityContainerFromMef2);

            Assert.AreSame(unityContainerFromMef1, unityContainerFromMef2);

            Assert.AreSame(unityContainer, unityContainerFromMef1);
        }
        
        [TestMethod]
        public void MefResolvesServiceRegisteredInUnityByTypeTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>(new ContainerControlledLifetimeManager());

            var container = unityContainer.Resolve<CompositionHost>();

            var unityOnlyComponent = container.GetExport<IUnityOnlyComponent>();

            var unityOnlyComponent2 = unityContainer.Resolve<IUnityOnlyComponent>();

            Assert.IsNotNull(unityOnlyComponent);

            Assert.AreEqual(typeof(UnityOnlyComponent1), unityOnlyComponent.GetType());

            Assert.IsNotNull(unityOnlyComponent2);

            Assert.AreEqual(typeof(UnityOnlyComponent1), unityOnlyComponent2.GetType());

            Assert.AreEqual(unityOnlyComponent, unityOnlyComponent2);
        }
        
        [TestMethod]
        public void MefCanResolveMefComponentThatHasUnityAndMefDependenciesTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            unityContainer.RegisterType<IUnityOnlyComponent, UnityOnlyComponent1>();

            var container = unityContainer.Resolve<CompositionHost>();

            var mefMixedComponent = container.GetExport<MefMixedComponent>();

            Assert.IsNotNull(mefMixedComponent);

            Assert.AreEqual(typeof(MefMixedComponent), mefMixedComponent.GetType());

            Assert.AreEqual(typeof(MefComponent1), mefMixedComponent.MefComponent.GetType());

            Assert.AreEqual(typeof(UnityOnlyComponent1), mefMixedComponent.UnityComponent.GetType());
        }
        
        [TestMethod]
        public void UnityResolvesUnityComponentRegisteredWithoutInterfaceTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            unityContainer.RegisterType<UnityComponent3>();

            var component2 = unityContainer.Resolve<UnityComponent2>();

            Assert.IsNotNull(component2);

            Assert.IsNotNull(component2.ImportedMefComponent);

            Assert.AreEqual(typeof(MefComponent2), component2.ImportedMefComponent.GetType());

            Assert.AreEqual(typeof(MefComponent2), component2.MefComponent.GetType());
        }
    }
}