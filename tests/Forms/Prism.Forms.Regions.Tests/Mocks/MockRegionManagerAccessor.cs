using System;
using Prism.Regions;
using Xamarin.Forms;

namespace Prism.Forms.Regions.Mocks
{
    internal class MockRegionManagerAccessor : IRegionManagerAccessor
    {
        public Func<VisualElement, string> GetRegionName;
        public Func<VisualElement, IRegionManager> GetRegionManager;

        public event EventHandler UpdatingRegions;

        string IRegionManagerAccessor.GetRegionName(VisualElement element)
        {
            return GetRegionName(element);
        }

        IRegionManager IRegionManagerAccessor.GetRegionManager(VisualElement element)
        {
            if (GetRegionManager != null)
            {
                return GetRegionManager(element);
            }

            return null;
        }

        public void UpdateRegions()
        {
            UpdatingRegions?.Invoke(this, EventArgs.Empty);
        }

        public int GetSubscribersCount()
        {
            return UpdatingRegions != null ? UpdatingRegions.GetInvocationList().Length : 0;
        }
    }
}
