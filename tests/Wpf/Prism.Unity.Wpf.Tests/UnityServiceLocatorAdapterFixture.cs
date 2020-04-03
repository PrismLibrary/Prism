using System;
using System.Collections.Generic;
using Prism.Ioc;
using Unity;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;
using Xunit;

namespace Prism.Unity.Wpf.Tests
{

    public class UnityServiceLocatorAdapterFixture
    {
        [Fact]
        public void ShouldForwardResolveToInnerContainer()
        {
            object myInstance = new object();

            IUnityContainer container = new MockUnityContainer()
                                            {
                                                ResolveMethod = delegate
                                                                    {
                                                                        return myInstance;
                                                                    }
                                            };
            var containerExtension = new Ioc.UnityContainerExtension(container);
            ContainerLocator.SetCurrent(containerExtension);

            Assert.Same(myInstance, ContainerLocator.Container.Resolve(typeof(object)));

        }

        [Fact]
        public void ShouldForwardResolveAllToInnerContainer()
        {
            IEnumerable<object> list = new List<object> {new object(), new object()};

            IUnityContainer container = new MockUnityContainer()
            {
                ResolveMethod = delegate
                {
                    return list;
                }
            };
            var containerExtension = new Ioc.UnityContainerExtension(container);
            ContainerLocator.SetCurrent(containerExtension);

            Assert.Same(list, ContainerLocator.Container.GetContainer().ResolveAll(typeof (object)));
        }

        private class MockUnityContainer : IUnityContainer
        {
            public Func<object> ResolveMethod { get; set; }

            #region Implementation of IDisposable

            public void Dispose()
            {

            }

            #endregion

            #region Implementation of IUnityContainer

            public IUnityContainer Parent => throw new NotImplementedException();

            public IEnumerable<IContainerRegistration> Registrations => throw new NotImplementedException();

            public IUnityContainer AddExtension(UnityContainerExtension extension)
            {
                throw new NotImplementedException();
            }

            public object BuildUp(Type type, object existing, string name, params ResolverOverride[] resolverOverrides)
            {
                throw new NotImplementedException();
            }

            public object Configure(Type configurationInterface)
            {
                throw new NotImplementedException();
            }

            public IUnityContainer CreateChildContainer()
            {
                throw new NotImplementedException();
            }

            public IUnityContainer RegisterInstance(Type type, string name, object instance, LifetimeManager lifetime)
            {
                throw new NotImplementedException();
            }

            public IUnityContainer RegisterType(Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
            {
                throw new NotImplementedException();
            }

            public IUnityContainer RemoveAllExtensions()
            {
                throw new NotImplementedException();
            }

            public object Resolve(Type type, string name, params ResolverOverride[] resolverOverrides)
            {
                return ResolveMethod();
            }

            public bool IsRegistered(Type type, string name)
            {
                throw new NotImplementedException();
            }

            public IUnityContainer RegisterType(Type registeredType, Type mappedToType, string name, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
            {
                throw new NotImplementedException();
            }

            public IUnityContainer RegisterInstance(Type type, string name, object instance, IInstanceLifetimeManager lifetimeManager)
            {
                throw new NotImplementedException();
            }

            public IUnityContainer RegisterFactory(Type type, string name, Func<IUnityContainer, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}