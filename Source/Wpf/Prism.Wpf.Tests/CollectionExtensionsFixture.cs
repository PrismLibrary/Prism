

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Prism.Wpf.Tests
{
    [TestClass]
    public class CollectionExtensionsFixture
    {
        [TestMethod]
        public void CanAddRangeToCollection()
        {
            Collection<object> col = new Collection<object>();
            List<object> itemsToAdd = new List<object>{"1", "2"};

            col.AddRange(itemsToAdd);

            Assert.AreEqual(2, col.Count);
            Assert.AreEqual("1", col[0]);
            Assert.AreEqual("2", col[1]);
        }
    }
}
