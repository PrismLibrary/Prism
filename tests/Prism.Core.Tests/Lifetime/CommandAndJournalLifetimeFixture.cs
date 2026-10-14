#nullable enable

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Xunit;

namespace Prism.Tests.Lifetime;

public class CommandAndJournalLifetimeFixture
{
    [Fact]
    public void RetainedCompositeReleasesUnregisteredCommandsAndOwnersAcrossChurn()
    {
        var root = new CompositeCommand(true);
        for (var i = 0; i < 32; i++)
        {
            var references = RegisterAndRemove(root);
            CollectionProbe.Collected(references.Item1);
            CollectionProbe.Collected(references.Item2);
            Assert.Empty(root.RegisteredCommands);
        }
        GC.KeepAlive(root);
    }

    [Fact]
    public void UnregisterDetachesCompositeFromRetainedChildPublisher()
    {
        var child = new DelegateCommand(() => { });
        CollectionProbe.Collected(RegisterAndRemoveParent(child));
        child.RaiseCanExecuteChanged();
        child.IsActive = true;
        GC.KeepAlive(child);
    }

    [Fact]
    public async Task CompletedAsyncCommandReleasesOwnerAfterUnregistration()
    {
        var root = new CompositeCommand();
        var reference = await ExecuteAndRemove(root);
        CollectionProbe.Collected(reference);
        GC.KeepAlive(root);
    }

    [Fact]
    public void RetainedJournalOwnsHistoryPayloadUntilClear()
    {
        var journal = new RegionNavigationJournal();
        var references = RecordHistory(journal);
        CollectionProbe.Collect();
        foreach (var reference in references) Assert.True(reference.IsAlive);
        journal.Clear();
        foreach (var reference in references) CollectionProbe.Collected(reference);
        Assert.Null(journal.CurrentEntry);
        Assert.False(journal.CanGoBack);
        Assert.False(journal.CanGoForward);
        GC.KeepAlive(journal);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Tuple<WeakReference, WeakReference> RegisterAndRemove(CompositeCommand root)
    {
        var owner = new CommandOwner();
        var command = new DelegateCommand(owner.Run);
        root.RegisterCommand(command);
        CollectionProbe.Collect();
        root.UnregisterCommand(command);
        root.UnregisterCommand(command);
        return Tuple.Create(new WeakReference(command), new WeakReference(owner));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference RegisterAndRemoveParent(DelegateCommand child)
    {
        var root = new CompositeCommand(true);
        root.RegisterCommand(child);
        root.UnregisterCommand(child);
        return new WeakReference(root);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static async Task<WeakReference> ExecuteAndRemove(CompositeCommand root)
    {
        var owner = new CommandOwner();
        var command = new AsyncDelegateCommand(owner.RunAsync);
        root.RegisterCommand(command);
        await command.Execute();
        Assert.False(command.IsExecuting);
        root.UnregisterCommand(command);
        return new WeakReference(owner);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference[] RecordHistory(RegionNavigationJournal journal)
    {
        var references = new WeakReference[3];
        for (var i = 0; i < references.Length; i++)
        {
            var payload = new object();
            references[i] = new WeakReference(payload);
            var parameters = new NavigationParameters { { "payload", payload } };
            journal.RecordNavigation(new RegionNavigationJournalEntry
            {
                Uri = new Uri("view" + i, UriKind.Relative), Parameters = parameters
            }, true);
        }
        return references;
    }

    [Fact]
    public async Task PendingExecutionOwnsTargetUntilControlledTaskCompletes()
    {
        var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var pair = StartPending(gate.Task);
        CollectionProbe.Collect();
        Assert.True(pair.Item1.IsAlive);
        gate.SetResult(true);
        await pair.Item2;
        CollectionProbe.Collected(pair.Item1);
        GC.KeepAlive(gate);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Tuple<WeakReference, Task> StartPending(Task gate)
    {
        var owner = new PendingOwner(gate);
        var command = new AsyncDelegateCommand(owner.RunAsync);
        return Tuple.Create(new WeakReference(owner), command.Execute());
    }

    private sealed class PendingOwner
    {
        private readonly Task _gate;
        private int _calls;
        public PendingOwner(Task gate) => _gate = gate;
        public async Task RunAsync(CancellationToken token)
        {
            await _gate.ConfigureAwait(false);
            token.ThrowIfCancellationRequested();
            _calls++;
        }
    }

    private sealed class CommandOwner
    {
        private int _calls;
        public void Run() => _calls++;
        public async Task RunAsync(CancellationToken token)
        {
            await Task.Yield();
            token.ThrowIfCancellationRequested();
            _calls++;
        }
    }
}
