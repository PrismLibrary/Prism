using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Maui.Tests.Mocks.Ioc;

internal class TestContainer : IContainerExtension
{
    private List<object> _instances = new ();
    private List<KeyValuePair<Type, Func<IContainerProvider, object>>> _factories = new();

    public IScopedProvider CurrentScope { get; }

    public IScopedProvider CreateScope()
    {
        throw new NotImplementedException();
    }

    public void FinalizeExtension()
    {
    }

    public bool IsRegistered(Type type)
    {
        throw new NotImplementedException();
    }

    public bool IsRegistered(Type type, string name)
    {
        throw new NotImplementedException();
    }

    public IContainerRegistry Register(Type from, Type to)
    {
        return this;
    }

    public IContainerRegistry Register(Type from, Type to, string name)
    {
        return this;
    }

    public IContainerRegistry Register(Type type, Func<object> factoryMethod)
    {
        return this;
    }

    public IContainerRegistry Register(Type type, Func<IContainerProvider, object> factoryMethod)
    {
        return this;
    }

    public IContainerRegistry RegisterInstance(Type type, object instance)
    {
        _instances.Add(instance);
        return this;
    }

    public IContainerRegistry RegisterInstance(Type type, object instance, string name)
    {
        throw new NotImplementedException();
    }

    public IContainerRegistry RegisterMany(Type type, params Type[] serviceTypes)
    {
        throw new NotImplementedException();
    }

    public IContainerRegistry RegisterManySingleton(Type type, params Type[] serviceTypes)
    {
        throw new NotImplementedException();
    }

    public IContainerRegistry RegisterScoped(Type from, Type to)
    {
        throw new NotImplementedException();
    }

    public IContainerRegistry RegisterScoped(Type type, Func<object> factoryMethod)
    {
        throw new NotImplementedException();
    }

    public IContainerRegistry RegisterScoped(Type type, Func<IContainerProvider, object> factoryMethod)
    {
        _factories.Add(new(type, factoryMethod));
        return this;
    }

    public IContainerRegistry RegisterSingleton(Type from, Type to)
    {
        throw new NotImplementedException();
    }

    public IContainerRegistry RegisterSingleton(Type from, Type to, string name)
    {
        throw new NotImplementedException();
    }

    public IContainerRegistry RegisterSingleton(Type type, Func<object> factoryMethod)
    {
        throw new NotImplementedException();
    }

    public IContainerRegistry RegisterSingleton(Type type, Func<IContainerProvider, object> factoryMethod)
    {
        _factories.Add(new(type, factoryMethod));
        return this;
    }

    public object Resolve(Type type)
    {
        return Resolve(type, Array.Empty<(Type, object)>());
    }

    public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
    {
        if(type.IsGenericType && typeof(IEnumerable<>).MakeGenericType(type.GenericTypeArguments) == type)
        {
            var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(type.GenericTypeArguments));
            foreach(var item in _instances.Where(x => x.GetType().IsAssignableTo(type.GenericTypeArguments.First())))
            {
                var addMethod = list.GetType().GetMethod("Add");
                addMethod.Invoke(list, new[] { item });
            }

            foreach(var item in _factories.Where(x => x.Key == type.GenericTypeArguments.First()))
            {
                var addMethod = list.GetType().GetMethod("Add");
                addMethod.Invoke(list, new[] { item.Value(this) });
            }

            return list;
        }
        else if (type == typeof(IEnumerable<ViewRegistration>))
            return _instances.OfType<ViewRegistration>();
        else if (_instances.Any(x => x.GetType() == type || x.GetType().IsAssignableTo(type)))
            return _instances.Last(x => x.GetType() == type || x.GetType().IsAssignableTo(type));

        var constructor = type.GetConstructors()
            .OrderByDescending(x => x.GetParameters().Length)
            .FirstOrDefault();

        if (constructor is null || !constructor.GetParameters().Any())
        {
            var aInstance = Activator.CreateInstance(type);
            _instances.Add(aInstance);
            return aInstance;
        }

        var args = new List<object>();
        foreach(var parameter in constructor.GetParameters())
        {
            if (parameter.ParameterType == typeof(IEnumerable<ViewRegistration>))
                args.Add(_instances.OfType<ViewRegistration>());
            else if (parameter.ParameterType == typeof(IContainerExtension) || parameter.ParameterType == typeof(IContainerProvider))
                args.Add(this);
            else if (_instances.Any(x => x.GetType().IsAssignableTo(parameter.ParameterType)))
                args.Add(_instances.Last(x => x.GetType().IsAssignableTo(parameter.ParameterType)));
            else if (!type.IsAbstract && !type.IsInterface)
                args.Add(Resolve(parameter.ParameterType, parameters));
            else
                throw new NotSupportedException();
        }

        var instance = constructor.Invoke(args.ToArray());
        _instances.Add(instance);
        return instance;
    }

    public object Resolve(Type type, string name)
    {
        throw new NotImplementedException();
    }

    public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
    {
        throw new NotImplementedException();
    }
}
