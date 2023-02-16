using DryIoc;

namespace Prism.DryIoc;

internal sealed class DryIocServiceScopeFactory : IServiceScopeFactory
{
    private readonly IResolverContext _scopedResolver;

    /// <summary>Stores passed scoped container to open nested scope.</summary>
    /// <param name="scopedResolver">Scoped container to be used to create nested scope.</param>
    public DryIocServiceScopeFactory(IResolverContext scopedResolver) => _scopedResolver = scopedResolver;

    /// <summary>Opens scope and wraps it into DI <see cref="IServiceScope"/> interface.</summary>
    /// <returns>DI wrapper of opened scope.</returns>
    public IServiceScope CreateScope()
    {
        var r = _scopedResolver;
        var scope = r.ScopeContext == null
            ? Scope.Of(r.OwnCurrentScope)
            : r.ScopeContext.SetCurrent(p => Scope.Of(p));
        return new DryIocServiceScope(r.WithCurrentScope(scope));
    }
}
