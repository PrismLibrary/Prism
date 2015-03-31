// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.IocContainer.Tests.Support.Mocks;
using Prism.Regions;
using Prism.Unity.Tests.Mocks;

namespace Prism.Wpf.Unity.Tests
{
    [TestClass]
    public class UnityRegionNavigationContentLoaderFixture
    {
        [TestMethod]
        public void ShouldFindCandidateViewInRegion()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<object, MockView>("MockView");

            this.ConfigureMockServiceLocator(container);

            // We cannot access the UnityRegionNavigationContentLoader directly so we need to call its
            // GetCandidatesFromRegion method through a navigation request.
            IRegion testRegion = new Region();

            MockView view = new MockView();
            testRegion.Add(view);
            testRegion.Deactivate(view);

            testRegion.RequestNavigate("MockView");

            Assert.IsTrue(testRegion.Views.Contains(view));
            Assert.IsTrue(testRegion.Views.Count() == 1);
            Assert.IsTrue(testRegion.ActiveViews.Count() == 1);
            Assert.IsTrue(testRegion.ActiveViews.Contains(view));
        }

        [TestMethod]
        public void ShouldFindCandidateViewWithFriendlyNameInRegion()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<MockView>("SomeView");

            this.ConfigureMockServiceLocator(container);

            // We cannot access the UnityRegionNavigationContentLoader directly so we need to call its
            // GetCandidatesFromRegion method through a navigation request.
            IRegion testRegion = new Region();

            MockView view = new MockView();
            testRegion.Add(view);
            testRegion.Deactivate(view);

            testRegion.RequestNavigate("SomeView");

            Assert.IsTrue(testRegion.Views.Contains(view));
            Assert.IsTrue(testRegion.ActiveViews.Count() == 1);
            Assert.IsTrue(testRegion.ActiveViews.Contains(view));
        }

        public void ConfigureMockServiceLocator(IUnityContainer container)
        {
            MockServiceLocator serviceLocator = new MockServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }
    }
}
