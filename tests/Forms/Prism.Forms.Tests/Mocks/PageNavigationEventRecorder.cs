using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Prism.Navigation;

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
        public void Record(object sender, 
            PageNavigationEvent pageNavigationEvent,
            INavigationParameters navigationParameters = null)
        {
            _records.Enqueue(new PageNavigationRecord(sender, pageNavigationEvent, navigationParameters));
        }

        public PageNavigationRecord TakeFirst()
        {
            if (_records.Count == 0) throw new InvalidOperationException("Not exist records.");

            return _records.Dequeue();
        }

        public void Clear()
        {
            _records.Clear();
        }
    }
}
