

using System;
using Prism.Events;

namespace Prism.Wpf.Tests.Mocks
{
    class MockDelegateReference : IDelegateReference
    {
        public Delegate Target { get; set; }
        public bool IsAlive => true;
        public MockDelegateReference()
        {

        }

        public MockDelegateReference(Delegate target)
        {
            Target = target;
        }
    }
}