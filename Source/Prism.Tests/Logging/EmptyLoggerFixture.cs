using Xunit;
using Prism.Logging;

namespace Prism.Tests.Logging
{
    public class EmptyLoggerFixture
    {
        [Fact]
        public void LogShouldNotFail()
        {
            ILoggerFacade logger = new EmptyLogger();

            logger.Log(null, Category.Debug, Priority.Medium);
        }
    }
}