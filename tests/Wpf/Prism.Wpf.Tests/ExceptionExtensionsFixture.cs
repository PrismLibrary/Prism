

using System;
using Xunit;

namespace Prism.Wpf.Tests
{
    
    public class ExceptionExtensionsFixture
    {
        [Fact]
        // Note, this test cannot be run twice in the same test run, because the registeration is static 
        // and we're not supplying an 'Unregister' method
        public void CanRegisterFrameworkExceptionTypes()
        {
            Assert.False(ExceptionExtensions.IsFrameworkExceptionRegistered(typeof(MockException)));

            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(MockException));

            Assert.True(ExceptionExtensions.IsFrameworkExceptionRegistered(typeof(MockException)));
        }

        [Fact]
        public void CanGetRootException()
        {
            Exception caughtException = null;
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(FrameworkException1));
            try
            {
                try
                {
                    throw new RootException();
                }
                catch (Exception ex)
                {
                    
                    throw new FrameworkException1(ex);
                }
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            Assert.NotNull(caughtException);

            Exception exception = caughtException.GetRootException();

            Assert.IsType<RootException>(exception);
        }

        [Fact]
        public void CanCompensateForInnerFrameworkExceptionType()
        {
            Exception caughtException = null;
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(FrameworkException2));
            try
            {
                try
                {
                    try
                    {
                        throw new RootException();
                    }
                    catch (Exception ex)
                    {
                        
                        throw new FrameworkException2(ex);
                    }
                }
                catch (Exception ex)
                {
                    
                    throw new NonFrameworkException(ex);
                }
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            Assert.NotNull(caughtException);

            Exception exception = caughtException.GetRootException();
            Assert.IsType<RootException>(exception);
        }

        [Fact]
        public void GetRootExceptionReturnsTopExceptionWhenNoUserExceptionFound()
        {
            Exception caughtException = null;
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(FrameworkException1));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(FrameworkException2));
            try
            {
                try
                {
                    throw new FrameworkException1(null);
                }
                catch (Exception ex)
                {

                    throw new FrameworkException2(ex);
                }
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            Assert.NotNull(caughtException);

            Exception exception = caughtException.GetRootException();
            Assert.IsType<FrameworkException2>(exception);
        }
            
        private class MockException : Exception
        {

        }



        private class FrameworkException2 : Exception
        {
            public FrameworkException2(Exception innerException)
                : base("", innerException)
            {

            }
        }

        private class FrameworkException1:Exception
        {
            public FrameworkException1(Exception innerException) : base("", innerException)
            {
                
            }
        }

        private class RootException:Exception
        {}

        private class NonFrameworkException : Exception
        {
            public NonFrameworkException(Exception innerException)
                : base("", innerException)
            {
                
            }
        }

    }

    

}
