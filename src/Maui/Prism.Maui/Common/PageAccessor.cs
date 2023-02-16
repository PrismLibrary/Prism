namespace Prism.Common;

internal class PageAccessor : IPageAccessor
{
    private WeakReference<Page> _weakPage;
    public Page Page
    {
        get => _weakPage?.TryGetTarget(out var target) ?? false ? target : null;
        set => _weakPage = value is null ? null : new(value);
    }
}
