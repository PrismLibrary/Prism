

using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Mef.Modularity;
using Prism.Modularity;

namespace Prism.Mef.Wpf.Tests
{
    [TestClass]
    public class DownloadedPartCatalogCollectionFixture
    {
        [TestMethod]
        public void WhenItemsAdded_GetReturnsItems()
        {
            // Prepare
            ModuleInfo moduleInfo1 = new ModuleInfo();
            ModuleInfo moduleInfo2 = new ModuleInfo();
            ModuleInfo moduleInfo3 = new ModuleInfo();

            ComposablePartCatalog catalog1 = new TypeCatalog();
            ComposablePartCatalog catalog2 = new TypeCatalog();
            ComposablePartCatalog catalog3 = new TypeCatalog();

            DownloadedPartCatalogCollection target = new DownloadedPartCatalogCollection();

            // Act
            target.Add(moduleInfo1, catalog1);
            target.Add(moduleInfo2, catalog2);
            target.Add(moduleInfo3, catalog3);

            // Verify
            Assert.AreSame(catalog1, target.Get(moduleInfo1));
            Assert.AreSame(catalog2, target.Get(moduleInfo2));
            Assert.AreSame(catalog3, target.Get(moduleInfo3));
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void WhenItemsNotInCollection_GetThrows()
        {
            // Prepare
            ModuleInfo moduleInfo1 = new ModuleInfo();
            ModuleInfo moduleInfo2 = new ModuleInfo();
            ModuleInfo moduleInfo3 = new ModuleInfo();

            ComposablePartCatalog catalog1 = new TypeCatalog();
            ComposablePartCatalog catalog2 = new TypeCatalog();

            DownloadedPartCatalogCollection target = new DownloadedPartCatalogCollection();

            target.Add(moduleInfo1, catalog1);
            target.Add(moduleInfo2, catalog2);

            // Act
            target.Get(moduleInfo3);    
        
            // Verify
            
        }

        [TestMethod]
        public void WhenItemsInCollection_TryGetReturnsTrueAndCatalog()
        {
            // Prepare
            ModuleInfo moduleInfo1 = new ModuleInfo();
            ModuleInfo moduleInfo2 = new ModuleInfo();
            ModuleInfo moduleInfo3 = new ModuleInfo();

            ComposablePartCatalog catalog1 = new TypeCatalog();
            ComposablePartCatalog catalog2 = new TypeCatalog();
            ComposablePartCatalog catalog3 = new TypeCatalog();

            DownloadedPartCatalogCollection target = new DownloadedPartCatalogCollection();

            target.Add(moduleInfo1, catalog1);
            target.Add(moduleInfo2, catalog2);
            target.Add(moduleInfo3, catalog3);

            // Act
            bool actual = target.TryGet(moduleInfo3, out catalog3);    
        
            // Verify
            Assert.IsTrue(actual);
            Assert.AreSame(catalog3, target.Get(moduleInfo3));
            
        }

        [TestMethod]
        public void WhenItemsNotInCollection_TryGetReturnsFalse()
        {
            // Prepare
            ModuleInfo moduleInfo1 = new ModuleInfo();
            ModuleInfo moduleInfo2 = new ModuleInfo();
            ModuleInfo moduleInfo3 = new ModuleInfo();

            ComposablePartCatalog catalog1 = new TypeCatalog();
            ComposablePartCatalog catalog2 = new TypeCatalog();

            DownloadedPartCatalogCollection target = new DownloadedPartCatalogCollection();

            target.Add(moduleInfo1, catalog1);
            target.Add(moduleInfo2, catalog2);

            ComposablePartCatalog catalog3;

            // Act
            bool actual = target.TryGet(moduleInfo3, out catalog3);    
        
            // Verify
            Assert.IsFalse(actual);
            
        }

        [TestMethod]
        public void WhenItemsRemvoed_TryGetReturnsFalse()
        {
            // Prepare
            ModuleInfo moduleInfo1 = new ModuleInfo();
            ModuleInfo moduleInfo2 = new ModuleInfo();
            ModuleInfo moduleInfo3 = new ModuleInfo();

            ComposablePartCatalog catalog1 = new TypeCatalog();
            ComposablePartCatalog catalog2 = new TypeCatalog();
            ComposablePartCatalog catalog3 = new TypeCatalog();

            DownloadedPartCatalogCollection target = new DownloadedPartCatalogCollection();

            target.Add(moduleInfo1, catalog1);
            target.Add(moduleInfo2, catalog2);
            target.Add(moduleInfo3, catalog3);

            // Act
            target.Remove(moduleInfo1);

            ComposablePartCatalog catalog1b;
            bool actual = target.TryGet(moduleInfo1, out catalog1b);

            // Verify
            Assert.IsFalse(actual);
           
        }

        [TestMethod]
        public void WhenItemsCleared_TryGetReturnsFalse()
        {
            // Prepare
            ModuleInfo moduleInfo1 = new ModuleInfo();
            ModuleInfo moduleInfo2 = new ModuleInfo();
            ModuleInfo moduleInfo3 = new ModuleInfo();

            ComposablePartCatalog catalog1 = new TypeCatalog();
            ComposablePartCatalog catalog2 = new TypeCatalog();
            ComposablePartCatalog catalog3 = new TypeCatalog();

            DownloadedPartCatalogCollection target = new DownloadedPartCatalogCollection();

            target.Add(moduleInfo1, catalog1);
            target.Add(moduleInfo2, catalog2);
            target.Add(moduleInfo3, catalog3);

            // Act
            target.Clear();

            ComposablePartCatalog catalog1b;
            ComposablePartCatalog catalog2b;
            ComposablePartCatalog catalog3b;

            bool actual1 = target.TryGet(moduleInfo1, out catalog1b);
            bool actual2 = target.TryGet(moduleInfo2, out catalog2b);
            bool actual3 = target.TryGet(moduleInfo3, out catalog3b);
            

            // Verify
            Assert.IsFalse(actual1);
            Assert.IsFalse(actual2);
            Assert.IsFalse(actual3);

        }
    }
}
