using System;

namespace Prism.Ioc
{
    public interface IContainerProvider
    {
        object Resolve(Type type);

        object Resolve(Type type, string name);
    }
}
