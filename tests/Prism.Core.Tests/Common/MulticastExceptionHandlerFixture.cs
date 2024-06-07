using Prism.Common;
using Xunit;

namespace Prism.Tests.Common;

#nullable enable
public class MulticastExceptionHandlerFixture
{
    [Fact]
    public void CanHandleGenericException()
    {
        var handler = new MulticastExceptionHandler();
        void Callback(Exception exception) { }
        handler.Register<Exception>(Callback);
        Assert.True(handler.CanHandle(new Exception()));
    }

    [Fact]
    public void DidHandleGenericException()
    {
        var handler = new MulticastExceptionHandler();
        handler.Register<Exception>(Callback);
        bool handled = false;
        void Callback(Exception exception)
        {
            handled = true;
        }
        handler.Handle(new Exception());

        Assert.True(handled);
    }

    [Fact]
    public void CanHandleGenericExceptionAsync()
    {
        var handler = new MulticastExceptionHandler();
        handler.Register<Exception>(Callback);
        Task Callback(Exception exception)
        {
            return Task.CompletedTask;
        }
        Assert.True(handler.CanHandle(new Exception()));
    }

    [Fact]
    public void CanHandleUsingBaseExceptionType()
    {
        var handler = new MulticastExceptionHandler();
        handler.Register<IOException>(Callback);
        Task Callback(IOException exception)
        {
            return Task.CompletedTask;
        }
        Assert.True(handler.CanHandle(new FileNotFoundException()));
    }

    [Fact]
    public void DidHandleGenericExceptionAsync()
    {
        var handler = new MulticastExceptionHandler();
        handler.Register<Exception>(Callback);
        bool handled = false;
        Task Callback(Exception exception)
        {
            handled = true;
            return Task.CompletedTask;
        }
        handler.Handle(new Exception());

        Assert.True(handled);
    }
    [Fact]
    public void CanHandleSpecificException()
    {
        var handler = new MulticastExceptionHandler();
        handler.Register<FileNotFoundException>(Callback);
        void Callback(FileNotFoundException exception) { }
        Assert.True(handler.CanHandle(new FileNotFoundException()));
    }

    [Fact]
    public async Task DidHandleSpecificException()
    {
        var handler = new MulticastExceptionHandler();
        handler.Register<FileNotFoundException>(Callback);
        bool handled = false;
        void Callback(FileNotFoundException exception)
        {
            handled = true;
        }
        await handler.HandleAsync(new FileNotFoundException());

        Assert.True(handled);
    }

    [Fact]
    public async Task DidHandleSpecificExceptionWithParameter()
    {
        var handler = new MulticastExceptionHandler();
        var expected = Guid.NewGuid();
        bool handled = false;
        handler.Register<FileNotFoundException>(Callback);

        void Callback(FileNotFoundException fnfe, object? parameter)
        {
            Assert.Equal(expected, parameter);
            handled = true;
        }
        await handler.HandleAsync(new FileNotFoundException(), expected);

        Assert.True(handled);
    }

    [Fact]
    public void CanHandleSpecificExceptionAsync()
    {
        var handler = new MulticastExceptionHandler();
        handler.Register<FileNotFoundException>(Callback);
        Task Callback(FileNotFoundException exception)
        {
            return Task.CompletedTask;
        }
        Assert.True(handler.CanHandle(new FileNotFoundException()));
    }

    [Fact]
    public async Task DidHandleSpecificExceptionAsync()
    {
        var handler = new MulticastExceptionHandler();
        handler.Register<FileNotFoundException>(Callback);
        bool handled = false;
        Task Callback(FileNotFoundException exception)
        {
            handled = true;
            return Task.CompletedTask;
        }
        await handler.HandleAsync(new FileNotFoundException());

        Assert.True(handled);
    }

    [Fact]
    public async Task DidHandleSpecificExceptionAsyncWithParameter()
    {
        var handler = new MulticastExceptionHandler();
        bool handled = false;
        var expected = Guid.NewGuid();
        handler.Register<FileNotFoundException>(Callback);

        Task Callback(FileNotFoundException fnfe, object?  parameter)
        {
            Assert.Equal(expected, parameter);
            handled = true;
            return Task.CompletedTask;
        }
        await handler.HandleAsync(new FileNotFoundException(), expected);

        Assert.True(handled);
    }

    [Fact]
    public async Task DidHandleSpecificExceptionAsyncWithParameterFirst()
    {
        var handler = new MulticastExceptionHandler();
        bool handled = false;
        var expected = Guid.NewGuid();
        handler.Register<FileNotFoundException>(Callback);

        Task Callback(object? parameter, FileNotFoundException fnfe)
        {
            Assert.Equal(expected, parameter);
            handled = true;
            return Task.CompletedTask;
        }
        await handler.HandleAsync(new FileNotFoundException(), expected);

        Assert.True(handled);
    }
}
