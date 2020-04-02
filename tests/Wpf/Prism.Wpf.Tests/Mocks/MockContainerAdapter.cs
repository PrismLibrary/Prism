using System;
using System.Collections.Generic;
using Prism.Ioc;

namespace Prism.Wpf.Tests.Mocks
{
    internal class MockContainerAdapter : IContainerExtension
    {
        public Dictionary<Type, object> ResolvedInstances = new Dictionary<Type, object>();

        public void FinalizeExtension()
        {
            
        }

        public bool IsRegistered(Type type)
        {
            throw new NotImplementedException();
        }

        public bool IsRegistered(Type type, string name)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry Register(Type from, Type to)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry Register(Type from, Type to, string name)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterInstance(Type type, object instance, string name)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to)
        {
            throw new NotImplementedException();
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to, string name)
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

        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}