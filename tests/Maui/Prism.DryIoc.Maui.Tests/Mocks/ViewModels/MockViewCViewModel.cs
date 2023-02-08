using Prism.Common;

namespace Prism.DryIoc.Maui.Tests.Mocks.ViewModels;

public class MockViewCViewModel : MockViewModelBase
{
    public MockViewCViewModel(IPageAccessor pageAccessor, INavigationService navigationService) : base(pageAccessor, navigationService)
    {
    }
}
