using CommonServiceLocator;
using Grace.DependencyInjection;
using Prism.Grace.Wpf.Regions;
using Prism.Regions;
using System;
using System.Collections.Generic;

namespace Prism.Grace.Wpf.Tests.Mocks
{
    public class MockServiceLocator : ServiceLocatorImplBase
    {
        private readonly DependencyInjectionContainer _container;

        public MockServiceLocator(DependencyInjectionContainer container)
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
                GraceRegionNavigationContentLoader loader = new GraceRegionNavigationContentLoader(this, _container);
                return new RegionNavigationService(this, loader, new RegionNavigationJournal());
            }

            return null;
        }
    }
}