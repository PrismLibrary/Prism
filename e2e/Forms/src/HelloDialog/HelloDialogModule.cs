using System;
using HelloDialog.ViewModels;
using HelloDialog.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace HelloDialog
{
    [ModuleDependency("HelloPageDialogModule")]
    public class HelloDialogModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<DemoDialog, DemoDialogViewModel>();
            containerRegistry.RegisterDialog<UserAlert, UserAlertViewModel>();
            containerRegistry.RegisterForNavigation<DialogDemoPage, DialogDemoPageViewModel>();
        }
    }
}
