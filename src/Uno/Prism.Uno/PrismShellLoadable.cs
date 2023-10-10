namespace Prism;

#nullable enable
internal sealed class PrismShellLoadable : Uno.ILoadable, Uno.Toolkit.ILoadable
{
    public bool IsExecuting { get; private set; } = true;

    public bool IsLoaded { get; private set; }

    public event EventHandler? IsExecutingChanged;
    public event EventHandler<EventArgs>? Loaded;

    public void FinishLoading()
    {
        IsExecuting = false;
        IsLoaded = true;
        IsExecutingChanged?.Invoke(this, EventArgs.Empty);
        Loaded?.Invoke(this, EventArgs.Empty);
    }
}
