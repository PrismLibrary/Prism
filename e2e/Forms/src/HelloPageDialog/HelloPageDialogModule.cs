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

        public async void OnInitialized(IContainerProvider containerProvider)
        {
            var name = await _pageDialogService.DisplayPromptAsync("Hello", "What is your name?", placeholder: "Your name");
            _pageDialogService.DisplayAlertAsync("Hello", $"{(string.IsNullOrWhiteSpace(name) ? "You" : name)} just loaded the PageDialogService Module!", "Ok");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
