using System;
using System.Collections.Generic;
using Prism.Ioc;

namespace Prism.Wpf.Tests.Mocks
{
    internal class MockContainerAdapter : IContainerExtension
    {
        public Dictionary<Type, object> ResolvedInstances = new Dictionary<Type, object>();

        public bool SupportsModules => true;

        public void FinalizeExtension()
        {
            
        }

        public void Register(Type from, Type to)
        {
            throw new NotImplementedException();
        }

        public void Register(Type from, Type to, string name)
        {
            throw new NotImplementedException();
        }

        public void RegisterInstance(Type type, object instance)
        {
            throw new NotImplementedException();
        }

        public void RegisterSingleton(Type from, Type to)
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type)
        {
            object resolvedInstance;
            if (!this.ResolvedInstances.ContainsKey(type))
            {
                resolvedInstance = Activator.CreateInstance(type);
                this.ResolvedInstances.Add(type, resolvedInstance);
            }
            else
            {
                resolvedInstance = this.ResolvedInstances[type];
            }

            return resolvedInstance;
        }

        public object Resolve(Type type, string name)
        {
            throw new NotImplementedException();
        }

        public object ResolveViewModelForView(object view, Type viewModelType)
        {
            throw new NotImplementedException();
        }
    }
}