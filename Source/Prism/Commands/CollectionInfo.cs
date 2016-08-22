using System;
using System.Collections.Specialized;
using System.Reflection;

namespace Prism.Commands
{
    internal class CollectionInfo
    {
        public PropertyInfo Property { get; }
        public INotifyCollectionChanged Collection { get; }

        public CollectionInfo(PropertyInfo property, INotifyCollectionChanged collection)
        {
            this.Property = property;
            this.Collection = collection;
        }
    }
}