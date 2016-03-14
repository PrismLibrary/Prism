using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;

namespace Prism.Composition.Windows.Tests
{
    [TestClass]
    public class LazyResolutionTests
    {
        public interface IMixedComponent { }

        public class MixedComponent1 : IMixedComponent
        {
            public static int InstanceCount;

            public MixedComponent1()
            {
                InstanceCount++;
            }
        }

        public class MixedComponent2 : IMixedComponent
        {
            public static int InstanceCount;

            public MixedComponent2()
            {
                InstanceCount++;
            }
        }

        public class MixedComponent3 : IMixedComponent { }

        [Export(typeof(IMixedComponent))]
        public class MixedComponent4 : IMixedComponent { }

        [Export(typeof(IMixedComponent))]
        public class MixedComponent5 : IMixedComponent
        {
            public static int InstanceCount;

            public MixedComponent5()
            {
                InstanceCount++;
            }
        }

        [TestMethod]
        public void UnityCanResolveLazyTypeRegisteredInMefTest()
        {
            MefComponent1.InstanceCount = 0;

            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            Assert.AreEqual(0, MefComponent1.InstanceCount);

            var lazyMefComponent = unityContainer.Resolve<Lazy<IMefComponent>>();
            
            Assert.AreEqual(0, MefComponent1.InstanceCount);

            Assert.IsNotNull(lazyMefComponent);

            Assert.IsNotNull(lazyMefComponent.Value);

            Assert.AreEqual(1, MefComponent1.InstanceCount);

            Assert.AreEqual(typeof(MefComponent1), lazyMefComponent.Value.GetType());
        }
        
        [TestMethod]
        public void UnityCanResolveLazyTypeRegisteredInUnityTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            UnityComponent1.InstanceCount = 0;

            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            var lazyUnityComponent = unityContainer.Resolve<Lazy<IUnityComponent>>();

            Assert.IsNotNull(lazyUnityComponent);

            Assert.AreEqual(0, UnityComponent1.InstanceCount);

            Assert.IsNotNull(lazyUnityComponent.Value);

            Assert.AreEqual(typeof(UnityComponent1), lazyUnityComponent.Value.GetType());

            Assert.AreEqual(1, UnityComponent1.InstanceCount);
        }
        
        [TestMethod]
        public void UnityCanResolveLazyEnumerableOfTypesRegisteredInUnityTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            UnityComponent1.InstanceCount = 0;

            unityContainer.RegisterType<IUnityComponent, UnityComponent1>("component1");

            unityContainer.RegisterType<IUnityComponent, UnityComponent2>("component2");

            var collectionOfLazyUnityComponents = unityContainer.Resolve<Lazy<IEnumerable<IUnityComponent>>>();

            Assert.IsNotNull(collectionOfLazyUnityComponents);

            Assert.AreEqual(0, UnityComponent1.InstanceCount);

            var list = new List<IUnityComponent>(collectionOfLazyUnityComponents.Value);

            Assert.AreEqual(1, UnityComponent1.InstanceCount);

            Assert.AreEqual(2, list.Count);
        }
        
        [TestMethod]
        public void UnityCanResolveEnumerableOfLazyTypesRegisteredInUnityAndMefTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            MixedComponent1.InstanceCount = 0;

            MixedComponent2.InstanceCount = 0;

            MixedComponent5.InstanceCount = 0;

            unityContainer.RegisterType<IMixedComponent, MixedComponent1>("component1");

            unityContainer.RegisterType<IMixedComponent, MixedComponent2>("component2");

            unityContainer.RegisterType<IMixedComponent, MixedComponent3>();

            var collectionOfLazyUnityComponents = unityContainer.Resolve<IEnumerable<Lazy<IMixedComponent>>>();

            Assert.IsNotNull(collectionOfLazyUnityComponents);

            Assert.AreEqual(0, MixedComponent1.InstanceCount);

            Assert.AreEqual(0, MixedComponent2.InstanceCount);

            Assert.AreEqual(0, MixedComponent5.InstanceCount);

            var list = new List<Lazy<IMixedComponent>>(collectionOfLazyUnityComponents);

            Assert.AreEqual(0, MixedComponent1.InstanceCount);

            Assert.AreEqual(0, MixedComponent2.InstanceCount);

            Assert.AreEqual(0, MixedComponent5.InstanceCount);

            Assert.IsNotNull(list[0].Value);

            Assert.IsNotNull(list[1].Value);

            Assert.IsNotNull(list[2].Value);

            Assert.IsNotNull(list[3].Value);

            Assert.IsNotNull(list[4].Value);

            Assert.AreEqual(1, MixedComponent1.InstanceCount);

            Assert.AreEqual(1, MixedComponent2.InstanceCount);

            Assert.AreEqual(1, MixedComponent5.InstanceCount);

            Assert.AreEqual(5, list.Count);

            Assert.AreEqual(1, list.Select(t => t.Value).OfType<MixedComponent1>().Count());

            Assert.AreEqual(1, list.Select(t => t.Value).OfType<MixedComponent2>().Count());

            Assert.AreEqual(1, list.Select(t => t.Value).OfType<MixedComponent3>().Count());

            Assert.AreEqual(1, list.Select(t => t.Value).OfType<MixedComponent4>().Count());

            Assert.AreEqual(1, list.Select(t => t.Value).OfType<MixedComponent5>().Count());
        }
        
        
        [TestMethod]
        public void UnityCanResolveEnumerableOfTypesRegisteredInUnityAndMefTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            MixedComponent1.InstanceCount = 0;

            MixedComponent2.InstanceCount = 0;

            MixedComponent5.InstanceCount = 0;

            unityContainer.RegisterType<IMixedComponent, MixedComponent1>("component1");

            unityContainer.RegisterType<IMixedComponent, MixedComponent2>("component2");

            unityContainer.RegisterType<IMixedComponent, MixedComponent3>();

            var collectionOfLazyUnityComponents = unityContainer.Resolve<IEnumerable<IMixedComponent>>();

            Assert.IsNotNull(collectionOfLazyUnityComponents);

            Assert.AreEqual(1, MixedComponent1.InstanceCount);

            Assert.AreEqual(1, MixedComponent2.InstanceCount);

            Assert.AreEqual(1, MixedComponent5.InstanceCount);

            var list = new List<IMixedComponent>(collectionOfLazyUnityComponents);

            Assert.AreEqual(5, list.Count);

            Assert.AreEqual(1, list.OfType<MixedComponent1>().Count());

            Assert.AreEqual(1, list.OfType<MixedComponent2>().Count());

            Assert.AreEqual(1, list.OfType<MixedComponent3>().Count());

            Assert.AreEqual(1, list.OfType<MixedComponent4>().Count());

            Assert.AreEqual(1, list.OfType<MixedComponent5>().Count());
        }

        
        [TestMethod]
        public void UnityCanResolveEnumerableOfLazyTypesRegisteredInUnityTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            UnityComponent1.InstanceCount = 0;

            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            unityContainer.RegisterType<IUnityComponent, UnityComponent2>("component2");

            var collectionOfLazyUnityComponents = unityContainer.Resolve<IEnumerable<Lazy<IUnityComponent>>>();

            Assert.IsNotNull(collectionOfLazyUnityComponents);

            Assert.AreEqual(0, UnityComponent1.InstanceCount);

            var list = new List<Lazy<IUnityComponent>>(collectionOfLazyUnityComponents);

            Assert.AreEqual(0, UnityComponent1.InstanceCount);

            Assert.IsNotNull(list[0].Value);

            Assert.IsNotNull(list[1].Value);

            Assert.AreEqual(1, UnityComponent1.InstanceCount);

            Assert.AreEqual(2, list.Count);
        }
        
        [TestMethod]
        public void UnityCanResolveEnumerableOfTypesRegisteredInUnityTest()
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            UnityComponent1.InstanceCount = 0;

            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            unityContainer.RegisterType<IUnityComponent, UnityComponent2>("component2");

            var collectionOfLazyUnityComponents = unityContainer.Resolve<IEnumerable<IUnityComponent>>();

            Assert.IsNotNull(collectionOfLazyUnityComponents);

            Assert.AreEqual(1, UnityComponent1.InstanceCount);

            var list = new List<IUnityComponent>(collectionOfLazyUnityComponents);

            Assert.AreEqual(2, list.Count);
        }
        
        public interface IModule { }

        [Export(typeof(IModule))]
        public class Module1 : IModule { }

        public class Module2 : IModule { }

        [TestMethod]
        public void UnityCanResolveEnumerableOfTypesRegisteredInUnityAndMefEvenIfBothMefAndUnityRegisterTheSameTypeTest()
        {
            // Setup
            var unityContainer = new UnityContainer();

            unityContainer.RegisterAssembly(this.GetType().GetTypeInfo().Assembly);

            unityContainer.RegisterType<IModule, Module1>();

            unityContainer.RegisterType<IModule, Module2>("module2");

            var modules1 = unityContainer.Resolve<IEnumerable<IModule>>();

            Assert.AreEqual(2, modules1.Count());

            Assert.AreEqual(1, modules1.OfType<Module1>().Count());

            var modules2 = unityContainer.Resolve<IEnumerable<IModule>>();

            Assert.AreEqual(2, modules2.Count());

            Assert.AreEqual(1, modules1.OfType<Module1>().Count());
        }
    }
}
