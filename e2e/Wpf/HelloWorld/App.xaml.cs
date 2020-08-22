using HelloWorld.Views;
using Prism.Ioc;
using System.Windows;
using Prism.Modularity;
using HelloWorld.Modules.ModuleA;

namespace HelloWorld
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSharedSamples();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleAModule>();
        }

        //to test various module catalogs:
        //1. Comment out ConfigureModuleCatalog 
        //2. Remove the project reference to the module project 
        //3. Make sure the Modules directory exists in the output path when debugging
        //4. Uncomment CreateModuleCatalog to test other modules catalogs
        //5. Uncomment post build task in ModuleA.csproj
        //protected override IModuleCatalog CreateModuleCatalog()
        //{
        //    return new ConfigurationModuleCatalog();

        //    return new DirectoryModuleCatalog() { ModulePath = "Modules" };

        //    return new XamlModuleCatalog("ModuleCatalog.xaml");
        //}
    }
}
