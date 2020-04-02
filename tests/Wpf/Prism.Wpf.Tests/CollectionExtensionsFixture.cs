

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace Prism.Wpf.Tests
{
    
    public class CollectionExtensionsFixture
    {
        [Fact]
        public void CanAddRangeToCollection()
        {
            Collection<object> col = new Collection<object>();
            List<object> itemsToAdd = new List<object>{"1", "2"};

            col.AddRange(itemsToAdd);

            Assert.Equal(2, col.Count);
            Assert.Equal("1", col[0]);
            Assert.Equal("2", col[1]);
        }
    }
}
