using Prism.Regions.Behaviors;
using System.Collections.Generic;
using System.Collections;

namespace Prism.Forms.Regions.Mocks
{
    internal class MockRegionBehaviorCollection : Dictionary<string, IRegionBehavior>, IRegionBehaviorCollection
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
