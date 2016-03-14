using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Composition.Hosting;
using System.Reflection;

namespace Prism.Composition.Windows.Tests
{
    [TestClass]
    public class CompositionExtensionTest
    {
        [TestMethod]
        public void UnityCanResolveMefComponentRegisteredByTypeTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            var mefComponent = unityContainer.Resolve<IMefComponent>();

            Assert.IsNotNull(mefComponent);

            Assert.AreEqual(typeof(MefComponent1), mefComponent.GetType());

            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();

            Assert.IsNotNull(unityComponent);

            Assert.AreEqual(typeof(MefComponent1), unityComponent.MefComponent.GetType());
        }

        [TestMethod]
        public void UnityCanResolveMefComponentRegisteredByTypeAndRegistrationNameTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            var mefComponent = unityContainer.Resolve<IMefComponent>("component2");

            Assert.IsNotNull(mefComponent);

            Assert.AreEqual(typeof(MefComponent2), mefComponent.GetType());

            unityContainer.RegisterType<IUnityComponent, UnityComponent2>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();

            Assert.IsNotNull(unityComponent);

            Assert.AreEqual(typeof(MefComponent2), unityComponent.MefComponent.GetType());
        }

        [TestMethod]
        public void UnityCanResolveMefComponentRegisteredByTypeUsingConstructorInjectionTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();

            Assert.IsNotNull(unityComponent);

            Assert.AreEqual(typeof(MefComponent1), unityComponent.MefComponent.GetType());
        }

        [TestMethod]
        public void UnityCanResolveMefComponentRegisteredByTypeAndRegistrationNameUsingConstructorInjectionTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            unityContainer.RegisterType<IUnityComponent, UnityComponent2>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();

            Assert.IsNotNull(unityComponent);

            Assert.AreEqual(typeof(MefComponent2), unityComponent.MefComponent.GetType());
        }

        [TestMethod]
        public void UnitySatisfiesMefImportsByTypeOnUnityComponentsTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();

            Assert.IsNotNull(unityComponent);

            Assert.AreEqual(typeof(MefComponent1), unityComponent.ImportedMefComponent.GetType());
        }

        [TestMethod]
        public void UnityLazySatisfiesMefImportsByTypeOnUnityComponentsTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            unityContainer.RegisterType<IUnityComponent, UnityComponent11>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();

            Assert.IsNotNull(unityComponent);

            Assert.AreEqual(typeof(UnityComponent11), unityComponent.GetType());

            Assert.AreEqual(typeof(MefComponent1), unityComponent.ImportedMefComponent.GetType());

            Assert.AreEqual(typeof(MefComponent1), unityComponent.MefComponent.GetType());

            var unityComponent11 = (UnityComponent11)unityComponent;

            var mefComponent = unityComponent11.MefComponentFactory();
            
            Assert.AreSame(mefComponent, unityComponent.MefComponent);
        }

        [TestMethod]
        public void UnitySatisfiesMefImportsByTypeAndRegistrationNameOnUnityComponentsTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            unityContainer.RegisterType<IUnityComponent, UnityComponent2>();

            var unityComponent = unityContainer.Resolve<IUnityComponent>();

            Assert.IsNotNull(unityComponent);

            Assert.AreEqual(typeof(MefComponent2), unityComponent.ImportedMefComponent.GetType());
        }

        [TestMethod]
        public void UnityCanResolveCompositionContainerTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            var compositionContainer = unityContainer.Resolve<CompositionHost>();

            Assert.IsNotNull(compositionContainer);
        }

        [TestMethod]
        public void UnityCannotResolveMultipleMefInstancesTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            Assert.ThrowsException<ResolutionFailedException>(delegate
            {
                unityContainer.Resolve<IMultipleMefComponent>();
            });
        }
    }
}
