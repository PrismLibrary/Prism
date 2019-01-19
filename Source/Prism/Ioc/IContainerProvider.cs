using System;
using System.Collections.Generic;

namespace Prism.Ioc
{
    public interface IContainerProvider
    {
        object Resolve(Type type);

        object Resolve(Type type, IDictionary<Type, object> parameters);

        object Resolve(Type type, string name);
    }
}
