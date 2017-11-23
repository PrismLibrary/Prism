using System;

namespace Prism.Ioc
{
    public interface IContainer
    {
        object Instance { get; }

        object Resolve(Type type);

        object Resolve(Type type, string name);

        T Resolve<T>();

        T Resolve<T>(string name);

        void RegisterInstance<TInterface>(TInterface instance);

        void RegisterSingleton<T>(); //maybe

        void RegisterSingleton<TFrom, TTo>() where TTo : TFrom;

        void RegisterType(Type from, Type to);

        void RegisterType(Type from, Type to, string name);

        void RegisterType<T>(); //maybe

        void RegisterType<T>(string name); //maybe

        void RegisterType<TFrom, TTo>() where TTo : TFrom;

        void RegisterType<TFrom, TTo>(string name) where TTo : TFrom;
    }
}
