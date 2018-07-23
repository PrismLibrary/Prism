using Prism.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Prism.Modularity
{
    /// <summary>
    /// Represents a group of <see cref="IModuleInfo"/> instances that are usually deployed together. <see cref="ModuleInfoGroup"/>s
    /// are also used by the <see cref="IModuleCatalog"/> to prevent common deployment problems such as having a module that's required
    /// at startup that depends on modules that will only be downloaded on demand.
    ///
    /// The group also forwards <see cref="Ref"/> and <see cref="InitializationMode"/> values to the <see cref="IModuleInfo"/>s that it
    /// contains.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class ModuleInfoGroup : IModuleInfoGroup
    {
        private readonly Collection<IModuleInfo> modules = new Collection<IModuleInfo>();

        /// <summary>
        /// Gets or sets the <see cref="IModuleInfo.InitializationMode"/> for the whole group. Any <see cref="IModuleInfo"/> classes that are
        /// added after setting this value will also get this <see cref="InitializationMode"/>.
        /// </summary>
        /// <see cref="IModuleInfo.InitializationMode"/>
        /// <value>The initialization mode.</value>
        public InitializationMode InitializationMode { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IModuleInfo.Ref"/> value for the whole group. Any <see cref="IModuleInfo"/> classes that are
        /// added after setting this value will also get this <see cref="Ref"/>.
        ///
        /// The ref value will also be used by the <see cref="IModuleManager"/> to determine which  <see cref="IModuleTypeLoader"/> to use.
        /// For example, using an "file://" prefix with a valid URL will cause the FileModuleTypeLoader to be used
        /// (Only available in the desktop version of CAL).
        /// </summary>
        /// <see cref="ModuleInfo.Ref"/>
        /// <value>The ref value that will be used.</value>
        public string Ref { get; set; }

        /// <summary>
        /// Adds an <see cref="IModuleInfo"/> moduleInfo to the <see cref="ModuleInfoGroup"/>.
        /// </summary>
        /// <param name="item">The <see cref="IModuleInfo"/> to the <see cref="ModuleInfoGroup"/>.</param>
        public void Add(IModuleInfo item)
        {
            ForwardValues(item);
            modules.Add(item);
        }


        /// <summary>
        /// Forwards <see cref="InitializationMode"/> and <see cref="Ref"/> properties from this <see cref="ModuleInfoGroup"/>
        /// to <paramref name="moduleInfo"/>.
        /// </summary>
        /// <param name="moduleInfo">The module info to forward values to.</param>
        /// <exception cref="ArgumentNullException">An <see cref="ArgumentNullException"/> is thrown if <paramref name="moduleInfo"/> is <see langword="null"/>.</exception>
        protected void ForwardValues(IModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException(nameof(moduleInfo));

            if (moduleInfo.Ref == null)
            {
                moduleInfo.Ref = Ref;
            }

            if (moduleInfo.InitializationMode == InitializationMode.WhenAvailable && InitializationMode != InitializationMode.WhenAvailable)
            {
                moduleInfo.InitializationMode = InitializationMode;
            }
        }

        /// <summary>
        /// Removes all <see cref="IModuleInfo"/>s from the <see cref="ModuleInfoGroup"/>.
        /// </summary>
        public void Clear() => modules.Clear();

        /// <summary>
        /// Determines whether the <see cref="ModuleInfoGroup"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ModuleInfoGroup"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="ModuleInfoGroup"/>; otherwise, false.
        /// </returns>
        public bool Contains(IModuleInfo item) => modules.Contains(item);

        /// <summary>
        /// Copies the elements of the <see cref="ModuleInfoGroup"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="ModuleInfoGroup"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.
        /// -or-
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="ModuleInfoGroup"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(IModuleInfo[] array, int arrayIndex)
        {
            modules.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ModuleInfoGroup"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="ModuleInfoGroup"/>.
        /// </returns>
        public int Count => modules.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="ModuleInfoGroup"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>false, because the <see cref="ModuleInfoGroup"/> is not Read-Only.
        /// </returns>
        public bool IsReadOnly => false;

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ModuleInfoGroup"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ModuleInfoGroup"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="ModuleInfoGroup"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="ModuleInfoGroup"/>.
        /// </returns>
        public bool Remove(IModuleInfo item) => modules.Remove(item);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IModuleInfo> GetEnumerator() => modules.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Adds an item to the <see cref="ModuleInfoGroup"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="T:System.Object"/> to add to the <see cref="ModuleInfoGroup"/>.
        /// Must be of type <see cref="ModuleInfo"/>
        /// </param>
        /// <returns>
        /// The position into which the new element was inserted.
        /// </returns>
        int IList.Add(object value)
        {
            this.Add((IModuleInfo)value);
            return 1;
        }

        /// <summary>
        /// Determines whether the <see cref="ModuleInfoGroup"/> contains a specific value.
        /// </summary>
        /// <param name="value">
        /// The <see cref="T:System.Object"/> to locate in the <see cref="ModuleInfoGroup"/>.
        /// Must be of type <see cref="IModuleInfo"/>
        /// </param>
        /// <returns>
        /// true if the <see cref="T:System.Object"/> is found in the <see cref="ModuleInfoGroup"/>; otherwise, false.
        /// </returns>
        bool IList.Contains(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));


            if (!(value is IModuleInfo moduleInfo))
                throw new ArgumentException(Resources.ValueMustBeOfTypeModuleInfo, nameof(value));

            return Contains(moduleInfo);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="ModuleInfoGroup"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="T:System.Object"/> to locate in the <see cref="ModuleInfoGroup"/>.
        /// Must be of type <see cref="ModuleInfo"/>
        /// </param>
        /// <returns>
        /// The index of <paramref name="value"/> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(object value) => modules.IndexOf((IModuleInfo)value);

        /// <summary>
        /// Inserts an item to the <see cref="ModuleInfoGroup"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted.</param>
        /// <param name="value">
        /// The <see cref="T:System.Object"/> to insert into the <see cref="ModuleInfoGroup"/>.
        /// Must be of type <see cref="IModuleInfo"/>
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="ModuleInfoGroup"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is not of type <see cref="IModuleInfo"/>
        /// </exception>
        public void Insert(int index, object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));


            if (!(value is IModuleInfo moduleInfo))
                throw new ArgumentException(Resources.ValueMustBeOfTypeModuleInfo, nameof(value));

            modules.Insert(index, moduleInfo);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ModuleInfoGroup"/> has a fixed size.
        /// </summary>
        /// <returns>false, because the <see cref="ModuleInfoGroup"/> does not have a fixed length.
        /// </returns>
        public bool IsFixedSize => false;

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ModuleInfoGroup"/>.
        /// </summary>
        /// <param name="value">
        /// The <see cref="T:System.Object"/> to remove from the <see cref="ModuleInfoGroup"/>.
        /// Must be of type <see cref="ModuleInfo"/>
        /// </param>
        void IList.Remove(object value)
        {
            Remove((IModuleInfo)value);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.
        /// </exception>
        public void RemoveAt(int index) => modules.RemoveAt(index);

        /// <summary>
        /// Gets or sets the <see cref="object"/> at the specified index.
        /// </summary>
        /// <value></value>
        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (ModuleInfo)value;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is less than zero.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.
        /// -or-
        /// <paramref name="index"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        void ICollection.CopyTo(Array array, int index) => 
            ((ICollection)modules).CopyTo(array, index);

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
        /// </summary>
        /// <value></value>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.
        /// </returns>
        public bool IsSynchronized => ((ICollection)modules).IsSynchronized;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        public object SyncRoot => ((ICollection)modules).SyncRoot;

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(IModuleInfo item) => modules.IndexOf(item);

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        public void Insert(int index, IModuleInfo item) => modules.Insert(index, item);

        /// <summary>
        /// Gets or sets the <see cref="IModuleInfo"/> at the specified index.
        /// </summary>
        /// <value>The <see cref="IModuleInfo"/> at the specified index </value>
        public IModuleInfo this[int index]
        {
            get => modules[index];
            set => modules[index] = value;
        }
    }
}
