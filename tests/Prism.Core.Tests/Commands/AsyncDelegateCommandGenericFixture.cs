using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Xunit;

namespace Prism.Tests.Commands;

public class AsyncDelegateCommandGenericFixture
{

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CancellationTokenFactory_ReturnsCommandAndProvidesTokenForEachExecution(bool useAsyncInterface)
    {
        using var first = new CancellationTokenSource();
        using var second = new CancellationTokenSource();
        var tokens = new List<CancellationToken>();
        var factoryCalls = 0;
        var command = new AsyncDelegateCommand<string>((parameter, token) =>
        {
            tokens.Add(token);
            return Task.CompletedTask;
        });

        var configuredCommand = command.CancellationTokenFactory(cancellationTokenFactory: () => ++factoryCalls == 1 ? first.Token : second.Token);

        Assert.Same(command, configuredCommand);
        Assert.Equal(0, factoryCalls);
        IAsyncCommand asyncCommand = command;
        Task Execute() => useAsyncInterface ? asyncCommand.ExecuteAsync("test") : command.Execute("test");

        await Execute();
        await Execute();

        Assert.Equal(2, factoryCalls);
        Assert.Equal(new[] { first.Token, second.Token }, tokens);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ExecuteWithExplicitToken_DoesNotInvokeCancellationTokenFactory(bool useAsyncInterface)
    {
        using var cancellation = new CancellationTokenSource();
        var receivedToken = cancellation.Token;
        var factoryCalls = 0;
        var command = new AsyncDelegateCommand<string>((parameter, token) =>
        {
            receivedToken = token;
            return Task.CompletedTask;
        }).CancellationTokenFactory(() =>
        {
            factoryCalls++;
            return cancellation.Token;
        });

        if (useAsyncInterface)
            await ((IAsyncCommand)command).ExecuteAsync("test", CancellationToken.None);
        else
            await command.Execute("test", CancellationToken.None);

        Assert.Equal(CancellationToken.None, receivedToken);
        Assert.Equal(0, factoryCalls);
    }

    [Fact]
    public void ICommandExecute_UsesDefaultTokenFactory()
    {
        using var cancellation = new CancellationTokenSource();
        var receivedToken = CancellationToken.None;
        var factoryCalls = 0;
        var command = new AsyncDelegateCommand<string>((parameter, token) =>
        {
            receivedToken = token;
            return Task.CompletedTask;
        }).CancellationTokenFactory(() =>
        {
            factoryCalls++;
            return cancellation.Token;
        });

        ((ICommand)command).Execute("test");

        Assert.Equal(cancellation.Token, receivedToken);
        Assert.Equal(1, factoryCalls);
        Assert.False(command.IsExecuting);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task SuppressedExecutionDoesNotAllowAnotherExecution(bool useAsyncInterface, bool useCancellationToken)
    {
        var completion = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        var executions = 0;
        var command = new AsyncDelegateCommand<string>(parameter =>
        {
            executions++;
            return completion.Task;
        });
        IAsyncCommand asyncCommand = command;

        Task Execute() => useAsyncInterface
            ? useCancellationToken ? asyncCommand.ExecuteAsync("test", CancellationToken.None) : asyncCommand.ExecuteAsync("test")
            : useCancellationToken ? command.Execute("test", CancellationToken.None) : command.Execute("test");

        var first = Execute();
        var second = Execute();
        var third = Execute();
        try
        {
            Assert.False(first.IsCompleted);
            Assert.True(second.IsCompletedSuccessfully);
            Assert.True(third.IsCompletedSuccessfully);
            Assert.True(command.IsExecuting);
            Assert.False(command.CanExecute("test"));
            Assert.Equal(1, executions);
        }
        finally
        {
            completion.SetResult(null);
            await Task.WhenAll(first, second, third);
        }

        Assert.False(command.IsExecuting);
        Assert.True(command.CanExecute("test"));
        await Execute();
        Assert.Equal(2, executions);
    }

    [Fact]
    public async Task SuppressedICommandExecutionPreservesExecutingState()
    {
        var completion = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        var executions = 0;
        var command = new AsyncDelegateCommand<string>(parameter =>
        {
            executions++;
            return completion.Task;
        });
        ICommand iCommand = command;
        var first = command.Execute("test");
        try
        {
            iCommand.Execute("test");
            Assert.True(command.IsExecuting);
            Assert.False(iCommand.CanExecute("test"));
            iCommand.Execute("test");
            Assert.Equal(1, executions);
        }
        finally
        {
            completion.SetResult(null);
            await first;
        }
    }

    [Fact]
    public async Task SuppressedExecutionDoesNotRaiseStateChangeNotifications()
    {
        // Construct without a synchronization context so notifications run inline.
        await Task.Run(async () =>
        {
            var completion = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            var command = new AsyncDelegateCommand<string>(parameter => completion.Task);
            var executingStates = new List<bool>();
            var canExecuteStates = new List<bool>();
            command.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(command.IsExecuting))
                    executingStates.Add(command.IsExecuting);
            };
            command.CanExecuteChanged += (_, _) => canExecuteStates.Add(command.CanExecute("test"));

            var first = command.Execute("test");
            var second = command.Execute("test");
            try
            {
                Assert.True(second.IsCompletedSuccessfully);
                Assert.Equal(new[] { true }, executingStates);
                Assert.Equal(new[] { false }, canExecuteStates);
            }
            finally
            {
                completion.SetResult(null);
                await Task.WhenAll(first, second);
            }

            Assert.Equal(new[] { true, false }, executingStates);
            Assert.Equal(new[] { false, true }, canExecuteStates);
        });
    }

    [Fact]
    public async Task ReentrantExecutionFromCanExecuteChangedIsSuppressed()
    {
        var completion = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        var executions = 0;
        var reentered = false;
        Task second = Task.CompletedTask;
        Task third = Task.CompletedTask;
        var command = new AsyncDelegateCommand<string>(parameter =>
        {
            executions++;
            return completion.Task;
        });
        command.CanExecuteChanged += (_, _) =>
        {
            if (!command.IsExecuting || reentered)
                return;

            reentered = true;
            second = command.Execute("test");
            third = command.Execute("test");
        };

        var first = command.Execute("test");
        try
        {
            Assert.True(reentered);
            Assert.True(second.IsCompletedSuccessfully);
            Assert.True(third.IsCompletedSuccessfully);
            Assert.True(command.IsExecuting);
            Assert.Equal(1, executions);
        }
        finally
        {
            completion.SetResult(null);
            await Task.WhenAll(first, second, third);
        }
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task ExecutionFailureClearsStateAndAllowsRetry(bool handled, bool synchronous)
    {
        var completion = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        var exception = new InvalidOperationException("Execution failed");
        Exception caughtException = null;
        var shouldFail = true;
        var handledCount = 0;
        var executions = 0;
        var command = new AsyncDelegateCommand<string>(parameter =>
        {
            executions++;
            if (!shouldFail)
                return Task.CompletedTask;
            if (synchronous)
                throw exception;
            return completion.Task;
        });
        if (handled)
            command.Catch<InvalidOperationException>(ex =>
            {
                caughtException = ex;
                handledCount++;
            });

        var first = command.Execute("test");
        if (!synchronous)
        {
            var suppressed = command.Execute("test");
            completion.SetException(exception);
            await suppressed;
        }

        if (handled)
            await first;
        else
            Assert.Same(exception, await Assert.ThrowsAsync<InvalidOperationException>(() => first));

        Assert.Equal(handled ? 1 : 0, handledCount);
        Assert.False(command.IsExecuting);
        Assert.True(command.CanExecute("test"));
        Assert.Same(handled ? exception : null, caughtException);
        shouldFail = false;
        await command.Execute("test");
        Assert.Equal(2, executions);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CancellationClearsStateAndAllowsRetry(bool handled)
    {
        using var cancellation = new CancellationTokenSource();
        var handledCount = 0;
        var executions = 0;
        var command = new AsyncDelegateCommand<string>((parameter, token) =>
        {
            executions++;
            return token.CanBeCanceled ? Task.Delay(Timeout.Infinite, token) : Task.CompletedTask;
        });
        if (handled)
            command.Catch<TaskCanceledException>(_ => handledCount++);

        var first = command.Execute("test", cancellation.Token);
        await command.Execute("test", CancellationToken.None);
        cancellation.Cancel();
        if (handled)
            await first;
        else
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => first);

        Assert.Equal(handled ? 1 : 0, handledCount);
        Assert.False(command.IsExecuting);
        Assert.True(command.CanExecute("test"));
        await command.Execute("test", CancellationToken.None);
        Assert.Equal(2, executions);
    }

    [Fact]
    public async Task ParallelExecutionStillInvokesEachDelegate()
    {
        var completion = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        var executions = 0;
        var command = new AsyncDelegateCommand<string>(parameter =>
        {
            executions++;
            return completion.Task;
        }).EnableParallelExecution();

        var first = command.Execute("test");
        var second = command.Execute("test");
        try
        {
            Assert.False(first.IsCompleted);
            Assert.False(second.IsCompleted);
            Assert.True(command.CanExecute("test"));
            Assert.Equal(2, executions);
        }
        finally
        {
            completion.SetResult(null);
            await Task.WhenAll(first, second);
        }

        Assert.False(command.IsExecuting);
        Assert.True(command.CanExecute("test"));
    }
}
