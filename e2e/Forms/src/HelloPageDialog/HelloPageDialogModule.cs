using System;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Services;

namespace HelloPageDialog
{
    public class HelloPageDialogModule : IModule
    {
        private IPageDialogService _pageDialogService { get; }

        public HelloPageDialogModule(IPageDialogService pageDialogService)
        {
            _pageDialogService = pageDialogService;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _pageDialogService.DisplayAlertAsync("Hello", "You just loaded the PageDialogService Module!", "Ok");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
