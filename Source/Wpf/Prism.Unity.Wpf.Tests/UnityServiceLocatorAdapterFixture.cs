

using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Prism.Unity.Wpf.Tests
{
    [TestClass]
    public class UnityServiceLocatorAdapterFixture
    {
        [TestMethod]
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

            Assert.AreSame(myInstance, containerAdapter.GetInstance(typeof (object)));

        }

        [TestMethod]
        public void ShouldForwardResolveAllToInnerContainer()
        {
            IEnumerable<object> list = new List<object> {new object(), new object()};

            IUnityContainer container = new MockUnityContainer()
            {
                ResolveAllMethod = delegate
                {
                    return list;
                }
            };

            IServiceLocator containerAdapter = new UnityServiceLocatorAdapter(container);

            Assert.AreSame(list, containerAdapter.GetAllInstances(typeof (object)));
        }

        private class MockUnityContainer : IUnityContainer
        {
            public Func<object> ResolveMethod { get; set; }

			public Func<IEnumerable<object>> ResolveAllMethod { get; set; }

            #region Implementation of IDisposable

            public void Dispose()
            {

            }

            #endregion

            #region Implementation of IUnityContainer

        	public IUnityContainer RegisterType(Type from, Type to, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        	{
        		throw new NotImplementedException();
        	}

        	public IUnityContainer RegisterInstance(Type t, string name, object instance, LifetimeManager lifetime)
            {
                throw new NotImplementedException();
            }

        	public object Resolve(Type t, string name, params ResolverOverride[] resolverOverrides)
        	{
        		return ResolveMethod();
        	}

        	public IEnumerable<object> ResolveAll(Type t, params ResolverOverride[] resolverOverrides)
        	{
        		return ResolveAllMethod();
        	}

        	public object BuildUp(Type t, object existing, string name, params ResolverOverride[] resolverOverrides)
        	{
        		throw new NotImplementedException();
        	}

            public void Teardown(object o)
            {
                throw new NotImplementedException();
            }

            public IUnityContainer AddExtension(UnityContainerExtension extension)
            {
                throw new NotImplementedException();
            }

            public object Configure(Type configurationInterface)
            {
                throw new NotImplementedException();
            }

            public IUnityContainer RemoveAllExtensions()
            {
                throw new NotImplementedException();
            }

            public IUnityContainer CreateChildContainer()
            {
                throw new NotImplementedException();
            }

            public IUnityContainer Parent
            {
                get { throw new NotImplementedException(); }
            }

        	public IEnumerable<ContainerRegistration> Registrations
        	{
        		get { throw new NotImplementedException(); }
        	}

            #endregion
        }
    }
}