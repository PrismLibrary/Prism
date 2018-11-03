using System;
using System.Collections.Generic;
using System.Reflection;
using CommonServiceLocator;
using Xunit;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Blocks;
using Ninject.Components;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace Prism.Ninject.Wpf.Tests
{
    
    public class NinjectServiceLocatorAdapterFixture
    {
        [Fact]
        public void ShouldForwardResolveToInnerKernel()
        {
            var myInstance = new object();

            IKernel kernel = new MockNinjectKernel
            {
                ResolveMethod = delegate { return new[] {myInstance}; }
            };

            IServiceLocator kernelAdapter = new NinjectServiceLocatorAdapter(kernel);

            Assert.Same(myInstance, kernelAdapter.GetInstance(typeof(object)));
        }

        [Fact]
        public void ShouldForwardResolveAllToInnerKernel()
        {
            IEnumerable<object> list = new List<object> {new object(), new object()};

            IKernel kernel = new MockNinjectKernel
            {
                ResolveMethod = () => list
            };

            IServiceLocator kernelAdapter = new NinjectServiceLocatorAdapter(kernel);

            Assert.Same(list, kernelAdapter.GetAllInstances(typeof(object)));
        }

        private class MockNinjectKernel : IKernel
        {
            public Func<IEnumerable<object>> ResolveMethod { get; set; }

            public void Dispose()
            {
            }

            public INinjectSettings Settings => throw new NotImplementedException();
            public IComponentContainer Components => throw new NotImplementedException();

            public IEnumerable<INinjectModule> GetModules()
            {
                throw new NotImplementedException();
            }

            public bool HasModule(string name)
            {
                throw new NotImplementedException();
            }

            public void Load(IEnumerable<INinjectModule> m)
            {
                throw new NotImplementedException();
            }

            public void Load(IEnumerable<string> filePatterns)
            {
                throw new NotImplementedException();
            }

            public void Load(IEnumerable<Assembly> assemblies)
            {
                throw new NotImplementedException();
            }

            public void Unload(string name)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IBinding> GetBindings(Type service)
            {
                throw new NotImplementedException();
            }

            public IActivationBlock BeginBlock()
            {
                throw new NotImplementedException();
            }

            public IBindingToSyntax<T> Bind<T>()
            {
                throw new NotImplementedException();
            }

            public IBindingToSyntax<T1, T2> Bind<T1, T2>()
            {
                throw new NotImplementedException();
            }

            public IBindingToSyntax<T1, T2, T3> Bind<T1, T2, T3>()
            {
                throw new NotImplementedException();
            }

            public IBindingToSyntax<T1, T2, T3, T4> Bind<T1, T2, T3, T4>()
            {
                throw new NotImplementedException();
            }

            public IBindingToSyntax<object> Bind(params Type[] services)
            {
                throw new NotImplementedException();
            }

            public void Unbind<T>()
            {
                throw new NotImplementedException();
            }

            public void Unbind(Type service)
            {
                throw new NotImplementedException();
            }

            public IBindingToSyntax<T1> Rebind<T1>()
            {
                throw new NotImplementedException();
            }

            public IBindingToSyntax<T1, T2> Rebind<T1, T2>()
            {
                throw new NotImplementedException();
            }

            public IBindingToSyntax<T1, T2, T3> Rebind<T1, T2, T3>()
            {
                throw new NotImplementedException();
            }

            public IBindingToSyntax<T1, T2, T3, T4> Rebind<T1, T2, T3, T4>()
            {
                throw new NotImplementedException();
            }

            public IBindingToSyntax<object> Rebind(params Type[] services)
            {
                throw new NotImplementedException();
            }

            public void AddBinding(IBinding binding)
            {
                throw new NotImplementedException();
            }

            public void RemoveBinding(IBinding binding)
            {
                throw new NotImplementedException();
            }

            public void Inject(object instance, params IParameter[] parameters)
            {
                throw new NotImplementedException();
            }

            public bool CanResolve(IRequest request)
            {
                throw new NotImplementedException();
            }

            public bool CanResolve(IRequest request, bool ignoreImplicitBindings)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<object> Resolve(IRequest request)
            {
                return ResolveMethod();
            }

            public IRequest CreateRequest(Type service, Func<IBindingMetadata, bool> constraint,
                IEnumerable<IParameter> parameters, bool isOptional, bool isUnique)
            {
                return new Request(service, constraint, parameters, null, isOptional, isUnique);
            }

            public bool Release(object instance)
            {
                throw new NotImplementedException();
            }

            public object GetService(Type serviceType)
            {
                throw new NotImplementedException();
            }

            public bool IsDisposed { get; }
            public event EventHandler Disposed;
        }
    }
}