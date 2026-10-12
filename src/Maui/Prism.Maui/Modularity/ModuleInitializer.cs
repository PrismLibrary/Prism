using Prism.Ioc;

namespace Prism.Modularity;

/// <summary>Resolves modules from the container, registers their types, and invokes their initialization callbacks.</summary>
public class ModuleInitializer : IModuleInitializer
{
    readonly IContainerExtension _container;

    /// <summary>Initializes a module initializer using the application's container.</summary>
    /// <param name="container">The container used to resolve modules and register their services.</param>
    public ModuleInitializer(IContainerExtension container)
    {
        _container = container;
    }

    /// <summary>Creates the specified module and calls its registration and initialization callbacks in that order.</summary>
    /// <param name="moduleInfo">The module metadata containing the type name to resolve.</param>
    public void Initialize(IModuleInfo moduleInfo)
    {
        var module = CreateModule(Type.GetType(moduleInfo.ModuleType, true));
        if (module != null)
        {
            module.RegisterTypes(_container);
            module.OnInitialized(_container);
        }
    }

    /// <summary>Resolves a module instance from the application's container.</summary>
    /// <param name="moduleType">The type implementing <see cref="IModule"/> to resolve.</param>
    /// <returns>The resolved module.</returns>
    protected virtual IModule CreateModule(Type moduleType)
    {
        return (IModule)_container.Resolve(moduleType);
    }
}
