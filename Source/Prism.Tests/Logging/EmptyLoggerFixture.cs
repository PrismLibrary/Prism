// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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