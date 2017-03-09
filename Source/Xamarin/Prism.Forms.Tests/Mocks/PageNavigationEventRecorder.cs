using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Prism.Forms.Tests.Mocks
{
    public class PageNavigationEventRecorder
    {
        private readonly Queue<PageNavigationRecord> _records = new Queue<PageNavigationRecord>();

        public IReadOnlyList<PageNavigationRecord> Records => _records.ToList();

        public bool IsEmpty => _records.Count == 0;

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        public PageNavigationEventRecorder()
        {
        }

        /// <summary>
        /// Recorde navigation event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pageNavigationEvent"></param>
        public void Record(object sender, PageNavigationEvent pageNavigationEvent)
        {
            _records.Enqueue(new PageNavigationRecord(sender, pageNavigationEvent));
        }

        public PageNavigationRecord TakeFirst()
        {
            if(_records.Count == 0) throw new InvalidOperationException("Not exist records.");

            return _records.Dequeue();
        }

        public void Clear()
        {
            _records.Clear();
        }
    }
}
