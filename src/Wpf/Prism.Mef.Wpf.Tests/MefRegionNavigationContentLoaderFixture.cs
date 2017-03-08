

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Mef.Regions;
using Prism.Regions;

namespace Prism.Mef.Wpf.Tests
{
    [TestClass]
    public class MefRegionNavigationContentLoaderFixture
    {
        [TestMethod]
        public void ShouldFindCandidateViewInRegion()
        {
            this.ConfigureMockServiceLocator();

            // We cannot access the MefRegionNavigationContentLoader directly so we need to call its
            // GetCandidatesFromRegion method through a navigation request.
            IRegion testRegion = new Region();

            MockView1 view = new MockView1();
            testRegion.Add(view);
            testRegion.Deactivate(view);

            testRegion.RequestNavigate("MockView1");

            Assert.IsTrue(testRegion.Views.Contains(view));
            Assert.IsTrue(testRegion.Views.Count() == 1);
            Assert.IsTrue(testRegion.ActiveViews.Count() == 1);
            Assert.IsTrue(testRegion.ActiveViews.Contains(view));
        }

        [TestMethod]
        public void ShouldFindCandidateViewWithFriendlyNameInRegion()
        {
            this.ConfigureMockServiceLocator();

            // We cannot access the MefRegionNavigationContentLoader directly so we need to call its
            // GetCandidatesFromRegion method through a navigation request.
            IRegion testRegion = new Region();

            MockView2 view = new MockView2();
            testRegion.Add(view);
            testRegion.Deactivate(view);

            testRegion.RequestNavigate("SomeView");

            Assert.IsTrue(testRegion.Views.Contains(view));
            Assert.IsTrue(testRegion.Views.Count() == 1);
            Assert.IsTrue(testRegion.ActiveViews.Count() == 1);
            Assert.IsTrue(testRegion.ActiveViews.Contains(view));
        }

        [TestMethod]
        public void MissingCandidateViewCreatesViewInRegion()
        {
            this.ConfigureMockServiceLocator();

            // We cannot access the UnityRegionNavigationContentLoader directly so we need to call its
            // GetCandidatesFromRegion method through a navigation request.
            IRegion testRegion = new Region();

            Assert.AreEqual(0, testRegion.Views.Count());
            Assert.AreEqual(0, testRegion.ActiveViews.Count());

            testRegion.RequestNavigate("MockView1");

            Assert.AreEqual(1, testRegion.Views.Count());
            Assert.AreEqual(1, testRegion.ActiveViews.Count());
        }

        public void ConfigureMockServiceLocator()
        {
            MockServiceLocator serviceLocator = new MockServiceLocator();
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }
    }

    [Export]
    public class MockView1
    {
    }

    [Export("SomeView")]
    public class MockView2
    {
    }

    public class MockServiceLocator : ServiceLocatorImplBase
    {
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            throw new NotImplementedException();
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (serviceType == typeof(IRegionNavigationService))
            {
                MefRegionNavigationContentLoader loader = new MefRegionNavigationContentLoader(this);
                return new MefRegionNavigationService(this, loader, new MefRegionNavigationJournal());
            }

            if (key == "MockView1")
            {
                return new MockView1();
            }

            return null;
        }
    }
}
