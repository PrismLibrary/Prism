

using System;
using System.Windows;
using Prism.Regions;

namespace Prism.Wpf.Tests.Mocks
{
    internal class MockRegionManagerAccessor : IRegionManagerAccessor
    {
        public Func<DependencyObject, string> GetRegionName;
        public Func<DependencyObject, IRegionManager> GetRegionManager;

        public event EventHandler UpdatingRegions;

        string IRegionManagerAccessor.GetRegionName(DependencyObject element)
        {
            return this.GetRegionName(element);
        }

        IRegionManager IRegionManagerAccessor.GetRegionManager(DependencyObject element)
        {
            if (this.GetRegionManager != null)
            {
                return this.GetRegionManager(element);
            }

            return null;
        }

        public void UpdateRegions()
        {
            if (this.UpdatingRegions != null)
            {
                this.UpdatingRegions(this, EventArgs.Empty);
            }
        }

        public int GetSubscribersCount()
        {
            return this.UpdatingRegions != null ? this.UpdatingRegions.GetInvocationList().Length : 0;
        }
    }
}