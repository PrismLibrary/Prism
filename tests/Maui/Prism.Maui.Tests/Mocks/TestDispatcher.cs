using Microsoft.Maui.Dispatching;

namespace Prism.Maui.Tests.Mocks;

internal class TestDispatcher
{
    public static readonly IDispatcherProvider Provider = new DispatcherProviderMock();

    public static IDispatcher Current => new DispatcherMock();

    private class DispatcherProviderMock : IDispatcherProvider
    {
        public IDispatcher GetForCurrentThread() => new DispatcherMock();
    }

    private class DispatcherMock : IDispatcher
    {
        public bool IsDispatchRequired { get; }

        public IDispatcherTimer CreateTimer() => new DispatchTimerMock();

        public bool Dispatch(Action action) => true;

        public bool DispatchDelayed(TimeSpan delay, Action action)
        {
            return delay > TimeSpan.Zero;
        }
    }

    private class DispatchTimerMock : IDispatcherTimer
    {
        public TimeSpan Interval { get; set; }
        public bool IsRepeating { get; set; }
        public bool IsRunning { get; private set; }

#pragma warning disable CS0067
        public event EventHandler Tick;
#pragma warning restore CS0067

        public void Start()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }
    }
}
