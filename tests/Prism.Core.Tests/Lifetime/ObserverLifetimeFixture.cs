using System;
using System.Runtime.CompilerServices;
using Prism.Commands;
using Prism.Mvvm;
using Xunit;

namespace Prism.Tests.Lifetime;

public class ObserverLifetimeFixture
{
    [Fact]
    public void NotificationCanClearAncestorAndRecreateObservedChain()
    {
        var owner = new Node { Child = new Node { Child = new Node() } };
        var oldChild = owner.Child;
        var command = owner.ObserveDescendant();
        var clearAncestor = true;
        var notifications = 0;
        command.CanExecuteChanged += (_, _) =>
        {
            notifications++;
            if (clearAncestor)
            {
                clearAncestor = false;
                owner.Child = null;
            }
        };

        oldChild.Child = new Node();
        Assert.Null(owner.Child);
        Assert.Equal(2, notifications);
        oldChild.Child.Ready = true;
        Assert.Equal(2, notifications);
        owner.Child = new Node { Child = new Node() };
        owner.Child.Child.Ready = true;
        Assert.Equal(4, notifications);
    }

    [Fact]
    public void ClearingObservedChildShouldReleasePreviousChild()
    {
        var owner = new Node();
        var pair = ObserveChild(owner);
        owner.Child = null;
        CollectionProbe.Collected(pair.Item1);
        GC.KeepAlive(pair.Item2);
        GC.KeepAlive(owner);
    }

    [Fact]
    public void ClearingSelfReferentialChainIgnoresAlreadyCapturedDetachedHandlers()
    {
        var owner = new Node();
        owner.Child = owner;
        var command = owner.ObserveDescendant();
        var notifications = 0;
        command.CanExecuteChanged += (_, _) => notifications++;
        owner.Child = null;
        Assert.Equal(1, notifications);
        owner.Child = new Node { Child = new Node() };
        Assert.Equal(2, notifications);
        owner.Child.Child.Ready = true;
        Assert.Equal(3, notifications);
    }

    [Fact]
    public void ReplacingObservedChildReleasesPreviousChild()
    {
        var owner = new Node();
        var pair = ObserveChild(owner);
        owner.Child = new Node();
        CollectionProbe.Collected(pair.Item1);
        GC.KeepAlive(pair.Item2);
        GC.KeepAlive(owner);
    }

    [Fact]
    public void ClearingNestedOwnerReleasesAllFormerDescendantsAcrossRecreation()
    {
        var owner = new Node();
        var command = owner.ObserveDescendant();
        for (var i = 0; i < 16; i++)
        {
            var references = CreateDescendants(owner);
            owner.Child = null;
            foreach (var reference in references) CollectionProbe.Collected(reference);
        }
        GC.KeepAlive(command);
        GC.KeepAlive(owner);
    }

    [Fact]
    public void RecreatedChainObservesCurrentChildAndIgnoresRemovedChild()
    {
        var owner = new Node { Child = new Node { Child = new Node() } };
        var oldChild = owner.Child;
        var oldLeaf = oldChild.Child;
        var command = owner.ObserveDescendant();
        var notifications = 0;
        command.CanExecuteChanged += (_, _) => notifications++;
        oldLeaf.Ready = true;
        Assert.Equal(1, notifications);
        owner.Child = null;
        Assert.Equal(2, notifications);
        oldLeaf.Ready = false;
        oldChild.Child = new Node();
        Assert.Equal(2, notifications);
        var currentChild = new Node { Child = new Node() };
        owner.Child = currentChild;
        Assert.Equal(3, notifications);
        currentChild.Child.Ready = true;
        Assert.Equal(4, notifications);
        currentChild.Child = null;
        Assert.Equal(5, notifications);
        currentChild.Child = new Node();
        Assert.Equal(6, notifications);
        currentChild.Child.Ready = true;
        Assert.Equal(7, notifications);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Tuple<WeakReference, DelegateCommand> ObserveChild(Node owner)
    {
        var child = new Node();
        owner.Child = child;
        return Tuple.Create(new WeakReference(child), owner.Observe());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[] CreateDescendants(Node owner)
    {
        var child = new Node { Child = new Node() };
        owner.Child = child;
        return new[] { new WeakReference(child), new WeakReference(child.Child) };
    }

    private sealed class Node : BindableBase
    {
        private Node _child;
        private bool _ready;
        public Node Child { get => _child; set => SetProperty(ref _child, value); }
        public bool Ready { get => _ready; set => SetProperty(ref _ready, value); }
        public DelegateCommand Observe() => new DelegateCommand(() => { }).ObservesProperty(() => Child.Ready);
        public DelegateCommand ObserveDescendant() => new DelegateCommand(() => { }).ObservesProperty(() => Child.Child.Ready);
    }
}
