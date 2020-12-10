using Prism.Navigation;

namespace Prism.Forms.Tests.Mocks.ViewModels
{
    public class ContentPageMockViewModel : ViewModelBase, IConfirmNavigation
    {
        public bool OnConfirmNavigationCalled { get; private set; }

        public bool CanNavigate(NavigationParameters parameters)
        {
            OnConfirmNavigationCalled = true;

            object canNavigate;
            if (parameters.TryGetValue("canNavigate", out canNavigate))
                return (bool)canNavigate;

            return true;
        }
    }
}
