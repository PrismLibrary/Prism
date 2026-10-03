using Prism.Common;
using Prism.Events;

namespace Prism.DryIoc.Maui.Tests.Mocks.Navigation;

internal sealed class TestPageNavigationService : PageNavigationService
{
    /// <summary>
    /// When true, the next <see cref="DoPop"/> returns null without calling the base implementation (exercises go-back failure paths).
    /// </summary>
    internal static bool ForceNextDoPopToReturnNull;
    internal static bool ThrowAfterNextPop;
    internal static bool ThrowAfterNextPush;

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
        if (ForceNextDoPopToReturnNull)
        {
            ForceNextDoPopToReturnNull = false;
            Recorder.Pop(new NavigationPop(null, useModalNavigation, animated));
            return null;
        }

        var page = await base.DoPop(navigation, useModalNavigation, animated);
        Recorder.Pop(new NavigationPop(page, useModalNavigation, animated));
        if (ThrowAfterNextPop)
        {
            ThrowAfterNextPop = false;
            throw new InvalidOperationException("Native pop failed after mutating the stack.");
        }
        return page;
    }

    protected override async Task DoPush(Page currentPage, Page page, bool? useModalNavigation, bool? animated, bool insertBeforeLast = false, int navigationOffset = 0)
    {
        Recorder.Push(new NavigationPush(currentPage, page, useModalNavigation, animated, insertBeforeLast, navigationOffset));
        await base.DoPush(currentPage, page, useModalNavigation, animated, insertBeforeLast, navigationOffset);
        if (ThrowAfterNextPush)
        {
            ThrowAfterNextPush = false;
            throw new InvalidOperationException("Native push failed after mutating the stack.");
        }
    }
}
