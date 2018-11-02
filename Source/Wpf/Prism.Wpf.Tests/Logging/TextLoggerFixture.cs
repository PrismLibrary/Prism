

using System;
using System.IO;
using System.Text;
using Xunit;
using Prism.Logging;

namespace Prism.Wpf.Tests.Logging
{
    
    public class TextLoggerFixture
    {
        [Fact]
        public void ShouldWriteToTextWriter()
        {
            TextWriter writer = new StringWriter();
            ILoggerFacade logger = new TextLogger() { Writer = writer };

            logger.Log("Test", Category.Debug, Priority.Low);
            Assert.Contains(writer.ToString(), "Test");
            Assert.Contains(writer.ToString(), "DEBUG");
            Assert.Contains(writer.ToString(), "Low");
        }

        [Fact]
        public void ShouldDisposeWriterOnDispose()
        {
            MockWriter writer = new MockWriter();
            ILoggerFacade logger = new TextLogger() { Writer = writer };

            Assert.False(writer.DisposeCalled);
            writer.Dispose();
            Assert.True(writer.DisposeCalled);
        }
    }

    internal class MockWriter : TextWriter
    {
        public bool DisposeCalled;
        public override Encoding Encoding
        {
            get { throw new NotImplementedException(); }
        }

        protected override void Dispose(bool disposing)
        {
            DisposeCalled = true;
            base.Dispose(disposing);
        }
    }
}