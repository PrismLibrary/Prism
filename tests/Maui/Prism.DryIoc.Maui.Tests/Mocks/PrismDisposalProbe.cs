namespace Prism.DryIoc.Maui.Tests.Mocks;

public sealed class PrismDisposalProbe : IDisposable
{
    public int DisposeCount { get; private set; }

    public void Dispose()
    {
        DisposeCount++;
    }
}
