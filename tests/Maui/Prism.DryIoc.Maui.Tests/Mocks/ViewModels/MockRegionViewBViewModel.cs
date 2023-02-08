using Prism.Common;

namespace Prism.DryIoc.Maui.Tests.Mocks.ViewModels;

public class MockRegionViewBViewModel
{
    private IPageAccessor _accessor;

    public MockRegionViewBViewModel(IPageAccessor accessor)
    {
        _accessor = accessor;
    }

    public Page Page => _accessor.Page;
}
