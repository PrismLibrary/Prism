using Uno.Toolkit;

namespace Prism;

#nullable enable
internal sealed class PrismShellLoadable : ILoadable
{
    private bool _isExecuting = true;
    public bool IsExecuting
    {
        get => _isExecuting;
        set
        {
            _isExecuting = value;
            IsExecutingChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? IsExecutingChanged;
}
