//using System;
//using System.Collections.Generic;
//using Autofac;
//using Microsoft.Practices.ServiceLocation;
//using Prism.Regions;

//namespace Prism.Autofac.Tests.Mocks
//{
//    public class MockServiceLocator : ServiceLocatorImplBase
//    {
//        IContainer container;

//        public MockServiceLocator(IContainer container)
//        {
//            this.container = container;
//        }

//        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
//        {
//            throw new NotImplementedException();
//        }

//        protected override object DoGetInstance(Type serviceType, string key)
//        {
//            if (serviceType == typeof(IRegionNavigationService))
//            {
//                UnityRegionNavigationContentLoader loader = new UnityRegionNavigationContentLoader(this, this.container);
//                return new RegionNavigationService(this, loader, new RegionNavigationJournal());
//            }

//            return null;
//        }
//    }
//}