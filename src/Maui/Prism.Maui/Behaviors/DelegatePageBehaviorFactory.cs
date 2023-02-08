using Prism.Ioc;

namespace Prism.Behaviors;

internal class DelegatePageBehaviorFactory : IPageBehaviorFactory
{
    private readonly Action<Page> _applyBehaviors;

    public DelegatePageBehaviorFactory(Action<Page> applyBehaviors)
    {
        _applyBehaviors = applyBehaviors;
    }

    public void ApplyPageBehaviors(Page page)
    {
        _applyBehaviors(page);
    }
}

internal class DelegateContainerPageBehaviorFactory : IPageBehaviorFactory
{
    private readonly Action<IContainerProvider, Page> _applyBehaviors;
    private readonly IContainerProvider _container;

    public DelegateContainerPageBehaviorFactory(Action<IContainerProvider, Page> applyBehaviors, IContainerProvider container)
    {
        _applyBehaviors = applyBehaviors;
        _container = container;
    }

    public void ApplyPageBehaviors(Page page)
    {
        _applyBehaviors(_container, page);
    }
}