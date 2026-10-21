namespace Playground.Module.ViewModels;

internal class ModuleAboutViewModel : BindableBase
{
    public string Body { get; } =
        "This view, its view models, and dialog registrations are supplied by the Playground.Module IModule. " +
        "The shell and application bootstrap live in the native HelloWorld application; Prism loads this module from the module catalog in App.ConfigureModuleCatalog.";
}
