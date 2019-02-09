using System;
using System.Collections.Generic;

namespace Prism.Ioc
{
    public interface IContainerProvider
    {
        object Resolve(Type type);

        object Resolve(Type type, params (Type Type, object Instance)[] parameters);

        object Resolve(Type type, string name);

        object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters);
    }
}
