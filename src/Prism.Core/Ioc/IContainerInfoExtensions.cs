using System;
using System.ComponentModel;

namespace Prism.Ioc
{
#pragma warning disable CS1591 // Hidden API
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class IContainerInfoExtensions
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Type GetRegistrationType(this IContainerExtension container, string key)
        {
            if (container is IContainerInfo ci)
                return ci.GetRegistrationType(key);

            return null;
        }
    }
#pragma warning restore CS1591 // Hidden API
}
