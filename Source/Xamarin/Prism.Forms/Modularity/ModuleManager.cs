namespace Prism.Modularity
{
    public class ModuleManager : IModuleManager
    {
        readonly IModuleCatalog _moduleCatalog;
        readonly IModuleInitializer _moduleInitializer;

        /// <summary>
        /// The module catalog.
        /// </summary>
        protected IModuleCatalog ModuleCatalog
        {
            get { return _moduleCatalog; }
        }

        public ModuleManager(IModuleCatalog moduleCatalog, IModuleInitializer moduleInitializer)
        {
            _moduleCatalog = moduleCatalog;
            _moduleInitializer = moduleInitializer;
        }

        void LoadModules()
        {
            foreach (var info in ModuleCatalog.Modules)
            {
                _moduleInitializer.Initialize(info);
            }
        }

        public void Run()
        {
            _moduleCatalog.Initialize();
            LoadModules();
        }
    }
}
