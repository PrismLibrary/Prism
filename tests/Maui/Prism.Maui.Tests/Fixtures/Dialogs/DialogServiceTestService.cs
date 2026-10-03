using Prism.Dialogs;

#nullable enable
namespace Prism.Maui.Tests.Fixtures.Dialogs;

internal sealed class DialogServiceTestService(Page page) : DialogServiceBase
{
    protected override Page GetCurrentPage() => page;
}
