using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Logging;

namespace Prism.Tests.Logging
{
    [TestClass]
    public class DebugLoggerFixture
    {
        [TestMethod]
        public void LogShouldNotFail()
        {
            ILoggerFacade logger = new DebugLogger();
            logger.Log(null, Category.Debug, Priority.Medium);
        }
    }
}