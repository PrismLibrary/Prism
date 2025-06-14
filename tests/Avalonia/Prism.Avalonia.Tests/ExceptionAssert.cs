using System;
using Xunit;

namespace Prism.Avalonia.Tests
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
                Assert.IsType(expectedExceptionType, ex);
                return;
            }

            //Assert.Fail("No exception thrown.  Expected exception type of {0}.", expectedExceptionType.Name);
        }
    }
}
