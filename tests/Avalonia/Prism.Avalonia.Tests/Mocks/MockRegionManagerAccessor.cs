using System;
using Avalonia;
using Prism.Navigation.Regions;

namespace Prism.Avalonia.Tests.Mocks
{
    internal class MockRegionManagerAccessor : IRegionManagerAccessor
    {
        public Func<AvaloniaObject, string> GetRegionName;
        public Func<AvaloniaObject, IRegionManager> GetRegionManager;

        public event EventHandler UpdatingRegions;

        string IRegionManagerAccessor.GetRegionName(AvaloniaObject element)
        {
            return GetRegionName(element);
        }

        IRegionManager IRegionManagerAccessor.GetRegionManager(AvaloniaObject element)
        {
            if (GetRegionManager != null)
                return GetRegionManager(element);

            return null;
        }

        public void UpdateRegions()
        {
            if (UpdatingRegions != null)
                UpdatingRegions(this, EventArgs.Empty);
        }

        public int GetSubscribersCount()
        {
            return UpdatingRegions != null ? UpdatingRegions.GetInvocationList().Length : 0;
        }
    }
}
