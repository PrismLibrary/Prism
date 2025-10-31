using System;
using Prism.Properties;

namespace Prism.Navigation.Regions
{
    /// <summary>
    /// Provides a base class for region's behaviors.
    /// </summary>
    public abstract class RegionBehavior : IRegionBehavior, IDisposable
    {
        private IRegion region;
        private bool _disposed;

        /// <summary>
        /// Behavior's attached region.
        /// </summary>
        public IRegion Region
        {
            get
            {
                return region;
            }
            set
            {
                if (this.IsAttached)
                {
                    throw new InvalidOperationException(Resources.RegionBehaviorRegionCannotBeSetAfterAttach);
                }

                this.region = value;
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if the behavior is attached to a region, <see langword="false"/> otherwise.
        /// </summary>
        public bool IsAttached { get; private set; }

        /// <summary>
        /// Attaches the behavior to the region.
        /// </summary>
        public void Attach()
        {
            if (this.region == null)
            {
                throw new InvalidOperationException(Resources.RegionBehaviorAttachCannotBeCallWithNullRegion);
            }

            IsAttached = true;
            OnAttach();
        }

        /// <summary>
        /// Override this method to perform the logic after the behavior has been attached.
        /// </summary>
        protected abstract void OnAttach();

        /// <summary>
        /// Override this method to perform cleanup when the behavior is being disposed.
        /// </summary>
        protected virtual void OnDetach()
        {
        }

        /// <summary>
        /// Disposes the behavior and detaches it from the region.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the behavior.
        /// </summary>
        /// <param name="disposing">True if disposing, false if finalizing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                OnDetach();
                IsAttached = false;
            }

            _disposed = true;
        }
    }
}
