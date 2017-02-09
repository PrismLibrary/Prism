using System;
using System.Collections.Generic;
using System.Threading;

namespace Prism.Forms.Tests.Mocks
{
    public class PageNavigationEventRecoder : IDisposable
    {
        private static object Monitored { get; } = new object();
        /// <summary>
        /// History currently in Recording.
        /// </summary>
        public static PageNavigationEventRecoder Current { get; private set; }

        private readonly List<PageNavigationRecord> _records = new List<PageNavigationRecord>();

        public IReadOnlyList<PageNavigationRecord> Records => _records;
        /// <summary>
        /// Initialize Instance.
        /// </summary>
        private PageNavigationEventRecoder()
        {
        }

        /// <summary>
        /// Recorde navigation event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pageNavigationEvent"></param>
        public static void Record(object sender, PageNavigationEvent pageNavigationEvent)
        {
            Current?.RecordInner(sender, pageNavigationEvent);
        }

        /// <summary>
        /// Recorde navigation event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pageNavigationEvent"></param>
        private void RecordInner(object sender, PageNavigationEvent pageNavigationEvent)
        {
            _records.Add(new PageNavigationRecord(sender, pageNavigationEvent));
        }

        /// <summary>
        /// Start Recording of Event.
        /// </summary>
        /// <returns></returns>
        public static PageNavigationEventRecoder BeginRecord()
        {
            // History is virtually Singleton.
            // But xUnit works with multithreading.
            // For this reason, it should not be able to execute multithreaded.
            Monitor.Enter(Monitored);
            try
            {
                while (Current != null)
                {
                    Monitor.Wait(Monitored);
                }
                Current = new PageNavigationEventRecoder();
                return Current;
            }
            finally
            {
                Monitor.Exit(Monitored);
            }
        }

        public void Dispose()
        {
            Monitor.Enter(Monitored);
            try
            {
                Current = null;
                Monitor.PulseAll(Monitored);
            }
            finally
            {
                Monitor.Exit(Monitored);
            }
        }
    }
}
