using Xunit;
using Prism.Logging;

namespace Prism.Tests.Logging
{
    public class DebugLoggerFixture
    {
        [Fact]
        public void LogShouldNotFail()
        {
            ILoggerFacade logger = new DebugLogger();
            logger.Log(null, Category.Debug, Priority.Medium);
        }
    }
}