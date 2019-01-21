using System;

namespace Prism.Ioc
{
    public interface IContainerRegistry
    {
        void RegisterInstance(Type type, object instance);

        void RegisterInstance(Type type, object instance, string name);

        void RegisterSingleton(Type from, Type to);

        void RegisterSingleton(Type from, Type to, string name);

        void Register(Type from, Type to);

        void Register(Type from, Type to, string name);

        bool IsRegistered(Type type);

        bool IsRegistered(Type type, string name);
    }
}
