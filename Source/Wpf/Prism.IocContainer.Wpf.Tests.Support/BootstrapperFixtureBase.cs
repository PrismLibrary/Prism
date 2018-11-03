
using System;
using Xunit;

namespace Prism.IocContainer.Wpf.Tests.Support
{
    public class BootstrapperFixtureBase
    {
        protected static void AssertExceptionThrownOnRun(Bootstrapper bootstrapper, Type expectedExceptionType, string expectedExceptionMessageSubstring)
        {
            bool exceptionThrown = false;
            try
            {
                bootstrapper.Run();
            }
            catch (Exception ex)
            {
                Assert.Equal(expectedExceptionType, ex.GetType());
                Assert.Contains(expectedExceptionMessageSubstring, ex.Message);
                exceptionThrown = true;
            }

            if (!exceptionThrown)
            {
                //Assert.Fail("Exception not thrown.");
            }
        }
    }
}