// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Windows;
using Prism.Regions;
using Prism.Regions.Behaviors;

namespace Prism.Wpf.Tests.Mocks
{
    public class MockHostAwareRegionBehavior : IHostAwareRegionBehavior
    {
        public IRegion Region { get; set; }

        public void Attach()
        {
            
        }

        public DependencyObject HostControl { get; set; }
    }
}
