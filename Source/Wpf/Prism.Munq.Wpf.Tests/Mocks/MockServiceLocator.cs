using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;
using global::Munq;
using Prism.Munq.Regions;

namespace Prism.Munq.Wpf.Tests.Mocks
{
    public class MockServiceLocator : ServiceLocatorImplBase
    {
        private readonly IDependecyRegistrar _container;

        public MockServiceLocator(IDependecyRegistrar container)
        {
            _container = container;
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            throw new NotImplementedException();
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (serviceType == typeof(IRegionNavigationService))
            {
                MunqRegionNavigationContentLoader loader = new MunqRegionNavigationContentLoader(this, _container);
                return new RegionNavigationService(this, loader, new RegionNavigationJournal());
            }

            return null;
        }
    }
}