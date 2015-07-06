

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Logging;

namespace Prism.Tests.Logging
{
    [TestClass]
    public class EmptyLoggerFixture
    {
        [TestMethod]
        public void LogShouldNotFail()
        {
            ILoggerFacade logger = new EmptyLogger();

            logger.Log(null, Category.Debug, Priority.Medium);
        }
    }
}