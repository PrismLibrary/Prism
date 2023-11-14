using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Xunit;

namespace Prism.Tests.Commands;

public class AsyncDelegateCommandFixture
{
    [Fact]
    public void WhenConstructedWithDelegate_InitializesValues()
    {
        var actual = new AsyncDelegateCommand(() => default);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task CannotExecuteWhileExecuting()
    {
        var tcs = new TaskCompletionSource<object>();
        var command = new AsyncDelegateCommand(async () => await tcs.Task);

        Assert.True(command.CanExecute());
        var task = command.Execute();
        Assert.False(command.CanExecute());
        tcs.SetResult("complete");
        await task;
        Assert.True(command.CanExecute());
    }

    [Fact]
    public async Task CanExecuteParallelTaskWhenEnabled()
    {
        var tcs = new TaskCompletionSource<object>();
        var command = new AsyncDelegateCommand(async () => await tcs.Task)
            .EnableParallelExecution();

        Assert.True(command.CanExecute());
        var task = command.Execute();
        Assert.True(command.CanExecute());
        tcs.SetResult("complete");
        await task;
        Assert.True(command.CanExecute());
    }

    [Fact]
    public async Task CanExecuteChangedFiresWhenExecuting()
    {
        var tcs = new TaskCompletionSource<object> ();
        var command = new AsyncDelegateCommand(async () => await tcs.Task);
        bool canExecuteChanged = false;

        command.CanExecuteChanged += Command_CanExecuteChanged;

        void Command_CanExecuteChanged(object sender, System.EventArgs e)
        {
            canExecuteChanged = true;
        }

        var task = command.Execute();
        command.CanExecuteChanged -= Command_CanExecuteChanged;

        Assert.True(command.IsExecuting);
        Assert.True(canExecuteChanged);
        tcs.SetResult(null);
        await task;
        Assert.False(command.IsExecuting);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldExecuteCommandAsynchronously()
    {
        // Arrange
        bool executed = false;
        var tcs = new TaskCompletionSource<object>();
        var command = new AsyncDelegateCommand(async (_) =>
        {
            await tcs.Task;
            executed = true;
        });

        // Act
        var task = command.Execute();
        Assert.False(executed);
        tcs.SetResult("complete");
        await task;

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public async Task ExecuteAsync_WithCancellationToken_ShouldExecuteCommandAsynchronously()
    {
        // Arrange
        bool executionStarted = false;
        bool executed = false;
        bool taskCancelled = false;
        var command = new AsyncDelegateCommand(Execute)
            .Catch<TaskCanceledException>(ex =>
            {
                taskCancelled = true;
            });

        async Task Execute(CancellationToken token)
        {
            executionStarted = true;
            await Task.Delay(1000, token);
            executed = true;
        }

        // Act
        using (var cancellationTokenSource = new CancellationTokenSource())
        {
            cancellationTokenSource.CancelAfter(50); // Cancel after 50 milliseconds
            await command.Execute(cancellationTokenSource.Token);
        }

        // Assert
        Assert.True(executionStarted);
        Assert.False(executed);
        Assert.True(taskCancelled);
    }

    [Fact]
    public async Task ICommandExecute_UsesDefaultTokenSourceFactory()
    {
        var cts = new CancellationTokenSource();
        var command = new AsyncDelegateCommand((token) => Task.Delay(1000, token))
            .CancellationTokenSourceFactory(() => cts.Token);
        ICommand iCommand = command;
        iCommand.Execute(null);

        Assert.True(command.IsExecuting);
        cts.Cancel();
        await Task.Delay(10);

        Assert.False(command.IsExecuting);
    }

    [Fact]
    public void ICommandExecute_HandlesErrorOnce()
    {
        var handled = 0;
        ICommand command = new AsyncDelegateCommand<string>(str => throw new System.Exception("Test"))
            .Catch(ex => handled++);
        command.Execute(string.Empty);
        Assert.Equal(1, handled);
    }
}
