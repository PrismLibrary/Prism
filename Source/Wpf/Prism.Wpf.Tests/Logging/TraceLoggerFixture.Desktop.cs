

using System.Diagnostics;
using Xunit;
using Prism.Logging;
using System;

namespace Prism.Wpf.Tests.Logging
{
    
    public class TraceLoggerFixture: IDisposable
    {
        TraceListener[] existingListeners;

        public TraceLoggerFixture()
        {
            existingListeners = new TraceListener[Trace.Listeners.Count];
            Trace.Listeners.CopyTo(existingListeners, 0);
            Trace.Listeners.Clear();
        }

        [Fact]
        public void ShouldWriteToTraceWriter()
        {
            var listener = new MockTraceListener();
            Trace.Listeners.Add(listener);

            var traceLogger = new TraceLogger();
            traceLogger.Log("Test debug message", Category.Debug, Priority.Low);

            Assert.Equal("Test debug message", listener.LogMessage);

            Trace.Listeners.Remove(listener);
        }


        [Fact]
        public void ShouldTraceErrorException()
        {
            var listener = new MockTraceListener();
            Trace.Listeners.Add(listener);

            var traceLogger = new TraceLogger();
            traceLogger.Log("Test exception message", Category.Exception, Priority.Low);

            Assert.Equal("Test exception message", listener.ErrorMessage);

            Trace.Listeners.Remove(listener);
        }

        public void Dispose()
        {
            Trace.Listeners.AddRange(existingListeners);
        }
    }

    class MockTraceListener : TraceListener
    {
        public string LogMessage { get; set; }
        public string ErrorMessage { get; set; }

        public override void Write(string message)
        {
            LogMessage = message;
        }

        public override void WriteLine(string message)
        {
            LogMessage = message;
        }

        public override void WriteLine(string message, string category)
        {
            LogMessage = message;
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            if (eventType == TraceEventType.Error)
            {
                ErrorMessage = message;
            }
            else
            {
                LogMessage = message;
            }
        }
    }
}