

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Logging;

namespace Prism.Wpf.Tests.Logging
{
    [TestClass]
    public class TraceLoggerFixture
    {
        TraceListener[] existingListeners;

        [TestInitialize]
        public void RemoveExisitingListeners()
        {
            existingListeners = new TraceListener[Trace.Listeners.Count];
            Trace.Listeners.CopyTo(existingListeners, 0);
            Trace.Listeners.Clear();
        }

        [TestCleanup]
        public void ReAttachExistingListeners()
        {
            Trace.Listeners.AddRange(existingListeners);
        }

        [TestMethod]
        public void ShouldWriteToTraceWriter()
        {
            var listener = new MockTraceListener();
            Trace.Listeners.Add(listener);

            var traceLogger = new TraceLogger();
            traceLogger.Log("Test debug message", Category.Debug, Priority.Low);

            Assert.AreEqual<string>("Test debug message", listener.LogMessage);

            Trace.Listeners.Remove(listener);
        }


        [TestMethod]
        public void ShouldTraceErrorException()
        {
            var listener = new MockTraceListener();
            Trace.Listeners.Add(listener);

            var traceLogger = new TraceLogger();
            traceLogger.Log("Test exception message", Category.Exception, Priority.Low);

            Assert.AreEqual<string>("Test exception message", listener.ErrorMessage);

            Trace.Listeners.Remove(listener);
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