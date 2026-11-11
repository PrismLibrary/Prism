using System.Collections;
using System.Collections.Generic;
using Prism.Navigation.Regions;

namespace Prism.Avalonia.Tests.Mocks
{
    internal class MockRegionBehaviorCollection : Dictionary<string, IRegionBehavior>, IRegionBehaviorCollection
    {
        IEnumerator<KeyValuePair<string, IRegionBehavior>> IEnumerable<KeyValuePair<string, IRegionBehavior>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
