

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prism.Regions
{
    /// <summary>
    /// Defines that a view is synchronized with its parent view's Active state.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SyncActiveStateAttribute : Attribute
    {
    }
}
