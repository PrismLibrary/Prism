// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Regions;

namespace Prism.Wpf.Tests.Regions
{
    [TestClass]
    public class AllActiveRegionFixture
    {
        [TestMethod]
        public void AddingViewsToRegionMarksThemAsActive()
        {
            IRegion region = new AllActiveRegion();
            var view = new object();

            region.Add(view);

            Assert.IsTrue(region.ActiveViews.Contains(view));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeactivateThrows()
        {
            IRegion region = new AllActiveRegion();
            var view = new object();
            region.Add(view);

            region.Deactivate(view);
        }


    }
}