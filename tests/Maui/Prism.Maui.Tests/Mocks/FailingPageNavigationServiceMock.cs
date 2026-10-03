using Prism.Events;
using Prism.Maui.Tests.Navigation;
using Prism.Navigation;

namespace Prism.Maui.Tests.Mocks;

internal sealed class FailingPageNavigationServiceMock : PageNavigationService, IPageAware
{
    public FailingPageNavigationServiceMock(PageNavigationContainerMock container, ApplicationMock app)
        : base(container, new TestWindowManager(app.Window), new EventAggregator(), new MutablePageAccessor())
    {
    }

    public bool FailNextPush { get; set; }
    public int FailPopCall { get; set; }
    public bool ThrowAfterPop { get; set; }
    private int _popCalls;

    protected override async Task<Page> DoPop(INavigation navigation, bool useModalNavigation, bool animated)
    {
        var fail = ++_popCalls == FailPopCall;
        if (fail && !ThrowAfterPop)
            throw new InvalidOperationException("Pop failed before changing the native stack.");
        var page = await base.DoPop(navigation, useModalNavigation, animated);
        if (fail)
            throw new InvalidOperationException("Pop failed after changing the native stack.");
        return page;
    }

    Page IPageAware.Page
    {
        get => ((MutablePageAccessor)_pageAccessor).Page;
        set => ((MutablePageAccessor)_pageAccessor).Page = value;
    }

    protected override async Task DoPush(Page currentPage, Page page, bool? useModalNavigation, bool? animated,
        bool insertBeforeLast = false, int navigationOffset = 0)
    {
        await base.DoPush(currentPage, page, useModalNavigation, animated, insertBeforeLast, navigationOffset);
        if (FailNextPush)
        {
            FailNextPush = false;
            throw new InvalidOperationException("Push failed after changing the native stack.");
        }
    }
}
