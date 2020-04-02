using Prism.Navigation;

namespace Prism.DI.Forms.Tests.Mocks.ViewModels
{
    public class CustomNamedNavServiceViewModel
    {
        public INavigationService NavigationService { get; }

        public CustomNamedNavServiceViewModel(INavigationService meaninglessName) => 
            NavigationService = meaninglessName;
    }
}