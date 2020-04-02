using System.Collections.Generic;
using Xamarin.Forms.Internals;

namespace Prism.DI.Forms.Tests.Mocks.Internals
{
    public class FormsLogObserver : LogListener
    {
        private List<(string category, string message)> _logs { get; }

        public FormsLogObserver()
        {
            _logs = new List<(string category, string message)>();
        }

        public IReadOnlyList<(string category, string message)> Logs => _logs;

        public override void Warning(string category, string message) =>
            _logs.Add((category, message));
    }
}