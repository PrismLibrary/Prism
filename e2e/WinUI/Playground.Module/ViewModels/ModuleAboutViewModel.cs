namespace Playground.Module.ViewModels;

internal class ModuleAboutViewModel : BindableBase
{
    public string Body { get; } =
        "PlaygroundModule registers the views, view models, and dialogs. " +
        "App.ConfigureModuleCatalog adds the module to the catalog.";
}
