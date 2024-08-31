using System;
using Prism.Events;

namespace Prism.Avalonia.Tests.Mocks
{
    class MockDelegateReference : IDelegateReference
    {
        public Delegate Target { get; set; }

        public MockDelegateReference()
        {
        }

        public MockDelegateReference(Delegate target)
        {
            Target = target;
        }
    }
}
