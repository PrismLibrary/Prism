﻿using System;
using System.Collections.Generic;
using CommonServiceLocator;
using Ninject;
using Prism.Ninject.Regions;
using Prism.Regions;

namespace Prism.Ninject.Wpf.Tests.Mocks
{
    public class MockServiceLocator : ServiceLocatorImplBase
    {
        private readonly IKernel _kernel;

        public MockServiceLocator(IKernel container)
        {
            _kernel = container;
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            throw new NotImplementedException();
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (serviceType == typeof(IRegionNavigationService))
            {
                NinjectRegionNavigationContentLoader loader = new NinjectRegionNavigationContentLoader(this, _kernel);
                return new RegionNavigationService(this, loader, new RegionNavigationJournal());
            }

            return null;
        }
    }
}