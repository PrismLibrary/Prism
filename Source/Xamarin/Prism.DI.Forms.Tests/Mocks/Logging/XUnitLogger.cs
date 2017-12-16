using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace Prism.Forms.Tests.Mocks.Logging
{
    public class XunitLogger : ILoggerFacade
    {
        private ITestOutputHelper _output { get; }

        public XunitLogger(ITestOutputHelper output)
        {
            _output = output;
        }

        public void Log(string message, Category category, Priority priority)
        {
            _output.WriteLine($"{category} - {priority}: {message}");
        }
    }
}
