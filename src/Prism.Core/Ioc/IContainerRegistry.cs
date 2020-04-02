using System;

namespace Prism.Ioc
{
    public interface IContainerRegistry
    {
        IContainerRegistry RegisterInstance(Type type, object instance);

        IContainerRegistry RegisterInstance(Type type, object instance, string name);

        IContainerRegistry RegisterSingleton(Type from, Type to);

        IContainerRegistry RegisterSingleton(Type from, Type to, string name);

        IContainerRegistry Register(Type from, Type to);

        IContainerRegistry Register(Type from, Type to, string name);

        bool IsRegistered(Type type);

        bool IsRegistered(Type type, string name);
    }
}
