using System;
using System.Collections.Generic;
using CommonServiceLocator;
using Unity;
using Xunit;
using Unity.Resolution;
using Unity.Extension;
using Unity.Registration;
using Unity.Lifetime;

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

            IServiceLocator containerAdapter = new UnityServiceLocatorAdapter(container);

            Assert.Same(myInstance, containerAdapter.GetInstance(typeof (object)));

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

            IServiceLocator containerAdapter = new UnityServiceLocatorAdapter(container);

            Assert.Same(list, containerAdapter.GetAllInstances(typeof (object)));
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

            #endregion
        }
    }
}