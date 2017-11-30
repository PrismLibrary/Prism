using System;

namespace Prism.Ioc
{
    public interface IContainerRegistry
    {
        void RegisterInstance(Type type, object instance);

        void RegisterSingleton(Type from, Type to);

        void Register(Type from, Type to);

        void Register(Type from, Type to, string name);
    }
}
