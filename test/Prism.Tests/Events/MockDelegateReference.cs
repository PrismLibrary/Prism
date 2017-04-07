using System;
using Prism.Events;

namespace Prism.Tests.Events
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