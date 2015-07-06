

using System;
using Prism.Regions.Behaviors;

namespace Prism.Regions
{
    /// <summary>
    /// When <see cref="RegionMemberLifetimeAttribute"/> is applied to class provides data
    /// the <see cref="RegionMemberLifetimeBehavior"/> can use to determine if the instance should
    /// be removed when it is deactivated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true )]
    public sealed class RegionMemberLifetimeAttribute : Attribute
    {
        /// <summary>
        /// Instantiates an instance of <see cref="RegionMemberLifetimeAttribute"/>
        /// </summary>
        public RegionMemberLifetimeAttribute()
        {
            KeepAlive = true;
        }

        ///<summary>
        /// Determines if the region member should be kept-alive
        /// when deactivated.
        ///</summary>
        public bool KeepAlive { get; set; }
    }
}
