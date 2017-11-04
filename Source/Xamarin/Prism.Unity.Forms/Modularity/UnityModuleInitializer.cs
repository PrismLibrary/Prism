using Unity;
using Prism.Modularity;
using System;

namespace Prism.Unity.Modularity
{
    public class UnityModuleInitializer : IModuleInitializer
    {
        readonly IUnityContainer _container;

        public UnityModuleInitializer(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize(ModuleInfo moduleInfo)
        {
            var module = CreateModule(moduleInfo.ModuleType);
            if (module != null)
                module.Initialize();
        }

        protected virtual IModule CreateModule(Type moduleType)
        {
            return (IModule)_container.Resolve(moduleType);
        }
    }
}
