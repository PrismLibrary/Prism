

using System.Windows;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Behaviors;

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
