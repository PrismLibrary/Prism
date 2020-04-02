

using System;
using CommonServiceLocator;

namespace Prism.Wpf.Tests.Mocks
{
    internal class MockServiceLocator : IServiceLocator
    {
        public Func<Type, object> GetInstance;
        public Func<Type, object> GetService;

        object IServiceLocator.GetInstance(Type serviceType)
        {
            if (this.GetInstance != null)
                return this.GetInstance(serviceType);

            return null;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            if (this.GetService != null)
                return this.GetService(serviceType);

            return null;
        }

        System.Collections.Generic.IEnumerable<TService> IServiceLocator.GetAllInstances<TService>()
        {
            throw new NotImplementedException();
        }

        System.Collections.Generic.IEnumerable<object> IServiceLocator.GetAllInstances(Type serviceType)
        {
            throw new NotImplementedException();
        }

        TService IServiceLocator.GetInstance<TService>(string key)
        {
            throw new NotImplementedException();
        }

        TService IServiceLocator.GetInstance<TService>()
        {
            return (TService)GetInstance(typeof(TService));
        }

        object IServiceLocator.GetInstance(Type serviceType, string key)
        {
            throw new NotImplementedException();
        }
    }
}