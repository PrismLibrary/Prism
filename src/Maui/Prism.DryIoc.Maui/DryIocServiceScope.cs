using DryIoc;

namespace Prism.DryIoc;

internal sealed class DryIocServiceScope : IServiceScope
{
    /// <inheritdoc />
    public IServiceProvider ServiceProvider => _resolverContext;
    private readonly IResolverContext _resolverContext;

    /// <summary>Creating from resolver context</summary>
    public DryIocServiceScope(IResolverContext resolverContext) => _resolverContext = resolverContext;

    /// <summary>Disposes the underlying resolver context</summary>
    public void Dispose() => _resolverContext.Dispose();
}
