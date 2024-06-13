using Avalonia;

namespace Prism.Avalonia.Tests.Mocks
{
    internal class MockRegionManagerAccessor : IRegionManagerAccessor
    {
        public Func<AvaloniaObject, string> GetRegionName;
        public Func<AvaloniaObject, IRegionManager> GetRegionManager;

        public event EventHandler UpdatingRegions;

        string IRegionManagerAccessor.GetRegionName(AvaloniaObject element)
        {
            return this.GetRegionName(element);
        }

        IRegionManager IRegionManagerAccessor.GetRegionManager(AvaloniaObject element)
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
