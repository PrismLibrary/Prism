using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Prism.Wpf.Tests
{
    public static class ExceptionAssert
    {
        public static void Throws<TException>(Action action)
            where TException : Exception
        {
            Throws(typeof(TException), action);
        }

        public static void Throws(Type expectedExceptionType, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, expectedExceptionType);
                return;
            }

            Assert.Fail("No exception thrown.  Expected exception type of {0}.", expectedExceptionType.Name);
        }
    }
}
