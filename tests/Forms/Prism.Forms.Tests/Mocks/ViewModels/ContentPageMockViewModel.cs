using System;
using Prism.Navigation;
using System.Threading.Tasks;

namespace Prism.Forms.Tests.Mocks.ViewModels
{
    public class ContentPageMockViewModel : ViewModelBase, IConfirmNavigation
    {
        public bool OnConfirmNavigationCalled { get; private set; } = false;

        public bool CanNavigate(INavigationParameters parameters)
        {
            OnConfirmNavigationCalled = true;

            if (parameters.ContainsKey("canNavigate"))
                return (bool)parameters["canNavigate"];

            return true;
        }
    }

    public class SecondContentPageMockViewModel : ViewModelBase
    {
    }
}
