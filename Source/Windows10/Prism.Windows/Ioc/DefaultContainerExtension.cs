using System;

namespace Prism.Ioc
{
    public sealed class DefaultContainerExtension : IContainerExtension
    {
        public bool SupportsModules => false;

        public void FinalizeExtension()
        {
            // empty
        }

        public void Register(Type from, Type to)
        {
            // empty
        }

        public void Register(Type from, Type to, string name)
        {
            // empty
        }

        public void RegisterInstance(Type type, object instance)
        {
            // empty
        }

        public void RegisterSingleton(Type from, Type to)
        {
            // empty
        }

        public object Resolve(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public object Resolve(Type type, string name)
        {
            return Activator.CreateInstance(type);
        }

        public object ResolveViewModelForView(object view, Type viewModelType)
        {
            return Activator.CreateInstance(viewModelType);
        }
    }
}
