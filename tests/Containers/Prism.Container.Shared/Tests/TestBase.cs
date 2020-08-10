using System;
using Xunit;

namespace Prism.Ioc.Tests
{
    [Collection(nameof(ContainerExtension))]
    public abstract class TestBase : IClassFixture<ContainerSetup>, IDisposable
    {
        private bool disposedValue;

        protected ContainerSetup Setup { get; }

        protected TestBase(ContainerSetup setup)
        {
            Setup = setup;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Setup.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
