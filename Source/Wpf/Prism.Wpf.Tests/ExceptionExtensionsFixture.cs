

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Prism.Wpf.Tests
{
    [TestClass]
    public class ExceptionExtensionsFixture
    {
        [TestMethod]
        // Note, this test cannot be run twice in the same test run, because the registeration is static 
        // and we're not supplying an 'Unregister' method
        public void CanRegisterFrameworkExceptionTypes()
        {
            Assert.IsFalse(ExceptionExtensions.IsFrameworkExceptionRegistered(typeof(MockException)));

            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(MockException));

            Assert.IsTrue(ExceptionExtensions.IsFrameworkExceptionRegistered(typeof(MockException)));
        }

        [TestMethod]
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

            Assert.IsNotNull(caughtException);

            Exception exception = caughtException.GetRootException();

            Assert.IsInstanceOfType(exception, typeof(RootException));
        }

        [TestMethod]
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

            Assert.IsNotNull(caughtException);

            Exception exception = caughtException.GetRootException();
            Assert.IsInstanceOfType(exception, typeof(RootException));
        }

        [TestMethod]
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

            Assert.IsNotNull(caughtException);

            Exception exception = caughtException.GetRootException();
            Assert.IsInstanceOfType(exception, typeof(FrameworkException2));
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
