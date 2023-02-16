using Prism.Common;

namespace Prism.DryIoc.Maui.Tests.Mocks.ViewModels;

public class MockHomeViewModel : MockViewModelBase
{
    public MockHomeViewModel(IPageAccessor pageAccessor, INavigationService navigationService) : base(pageAccessor, navigationService)
    {
    }
}
