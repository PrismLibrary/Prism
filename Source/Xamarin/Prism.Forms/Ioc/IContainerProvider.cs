using System;

namespace Prism.Ioc
{
    public interface IContainerProvider<TContainer> : IContainerProvider
    {
        TContainer Instance { get; }
    }

    public interface IContainerProvider
    {
        object Resolve(Type type);

        object Resolve(Type type, string name);

        T Resolve<T>();

        T Resolve<T>(string name);
    }
}
