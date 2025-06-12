using System.Collections;
using System.Collections.Generic;
using Prism.Navigation.Regions;

namespace Prism.Avalonia.Tests.Mocks
{
    internal class MockRegionBehaviorCollection : Dictionary<string, IRegionBehavior>, IRegionBehaviorCollection
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
