using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Prism.Forms.Tests.Mocks
{
    public class PageNavigationEventRecoder : IDisposable
    {
        private static ThreadLocal<PageNavigationEventRecoder> Current { get; } = new ThreadLocal<PageNavigationEventRecoder>();

        private readonly Queue<PageNavigationRecord> _records = new Queue<PageNavigationRecord>();

        public IReadOnlyList<PageNavigationRecord> Records => _records.ToList();

        public bool IsEmpty => _records.Count == 0;

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
            _records.Enqueue(new PageNavigationRecord(sender, pageNavigationEvent));
        }

        public PageNavigationRecord TakeFirst()
        {
            if(_records.Count == 0) throw new InvalidOperationException("Not exist records.");

            return _records.Dequeue();
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
