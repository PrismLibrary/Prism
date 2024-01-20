using Prism.Common;
using Prism.Events;

namespace Prism.DryIoc.Maui.Tests.Mocks.Navigation;

internal sealed class TestPageNavigationService : PageNavigationService
{
    public TestPageNavigationService(
        IContainerProvider container,
        IWindowManager windowManager,
        IEventAggregator eventAggregator,
        IPageAccessor pageAccessor,
        NavigationTestRecorder recorder)
        : base(container, windowManager, eventAggregator, pageAccessor)
    {
        Recorder = recorder;
    }

    public NavigationTestRecorder Recorder { get; }

    protected override async Task<Page> DoPop(INavigation navigation, bool useModalNavigation, bool animated)
    {
        var page = await base.DoPop(navigation, useModalNavigation, animated);
        Recorder.Pop(new NavigationPop(page, useModalNavigation, animated));
        return page;
    }

    protected override Task DoPush(Page currentPage, Page page, bool? useModalNavigation, bool? animated, bool insertBeforeLast = false, int navigationOffset = 0)
    {
        Recorder.Push(new NavigationPush(currentPage, page, useModalNavigation, animated, insertBeforeLast, navigationOffset));
        return base.DoPush(currentPage, page, useModalNavigation, animated, insertBeforeLast, navigationOffset);
    }
}
