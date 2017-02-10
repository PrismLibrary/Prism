using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Prism.Forms.Tests.Mocks
{
    public class PageNavigationEventRecoder : IDisposable
    {
        private static ThreadLocal<PageNavigationEventRecoder> Current { get; } = new ThreadLocal<PageNavigationEventRecoder>();

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
            Current.Value?.RecordInner(sender, pageNavigationEvent);
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
            Current.Value = new PageNavigationEventRecoder();
            return Current.Value;
        }

        public void Dispose()
        {
            Current.Value = null;
        }
    }
}
