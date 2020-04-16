using System;
using System.ComponentModel;

namespace Prism.Ioc
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IContainerInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetRegistrationType(string key);
    }
}
