using Prism.Common;

namespace Prism.DryIoc.Maui.Tests.Mocks.ViewModels;

internal class ForcedViewModel
{
    public ForcedViewModel(IPageAccessor accessor)
    {
        _accessor = accessor;
    }

    private readonly IPageAccessor _accessor;

    public Page Page => _accessor.Page;
}
