using System;
using System.Collections.Generic;
using CommonServiceLocator;
using Unity;
using Prism.Regions;
using Prism.Unity.Regions;

namespace Prism.Unity.Wpf.Tests.Mocks
{
    public class MockServiceLocator : ServiceLocatorImplBase
    {
        private readonly IUnityContainer _container;

        public MockServiceLocator(IUnityContainer container)
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
                UnityRegionNavigationContentLoader loader = new UnityRegionNavigationContentLoader(this, _container);
                return new RegionNavigationService(this, loader, new RegionNavigationJournal());
            }

            return null;
        }
    }
}