

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Common;

namespace Prism.Wpf.Tests
{
    [TestClass]
    public class ListDictionaryFixture
    {
        static ListDictionary<string, object> list;

        [TestInitialize]
        public void SetUp()
        {
            list = new ListDictionary<string, object>();
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void AddThrowsIfKeyNull()
        {
            list.Add(null, new object());
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void AddThrowsIfValueNull()
        {
            list.Add("", null);
        }

        [TestMethod]
        public void CanAddValue()
        {
            object value1 = new object();
            object value2 = new object();

            list.Add("foo", value1);
            list.Add("foo", value2);

            Assert.AreEqual(2, list["foo"].Count);
            Assert.AreSame(value1, list["foo"][0]);
            Assert.AreSame(value2, list["foo"][1]);
        }

        [TestMethod]
        public void CanIndexValuesByKey()
        {
            list.Add("foo", new object());
            list.Add("foo", new object());

            Assert.AreEqual(2, list["foo"].Count);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void ThrowsIfRemoveKeyNull()
        {
            list.RemoveValue(null, new object());
        }

        [TestMethod]
        public void CanRemoveValue()
        {
            object value = new object();

            list.Add("foo", value);
            list.RemoveValue("foo", value);

            Assert.AreEqual(0, list["foo"].Count);
        }

        [TestMethod]
        public void CanRemoveValueFromAllLists()
        {
            object value = new object();
            list.Add("foo", value);
            list.Add("bar", value);

            list.RemoveValue(value);

            Assert.AreEqual(0, list.Values.Count);
        }

        [TestMethod]
        public void RemoveNonExistingValueNoOp()
        {
            list.Add("foo", new object());

            list.RemoveValue("foo", new object());
        }

        [TestMethod]
        public void RemoveNonExistingKeyNoOp()
        {
            list.RemoveValue("foo", new object());
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void ThrowsIfRemoveListKeyNull()
        {
            list.Remove(null);
        }

        [TestMethod]
        public void CanRemoveList()
        {
            list.Add("foo", new object());
            list.Add("foo", new object());

            bool removed = list.Remove("foo");

            Assert.IsTrue(removed);
            Assert.AreEqual(0, list.Keys.Count);
        }

        [TestMethod]
        public void CanSetList()
        {
            List<object> values = new List<object>();
            values.Add(new object());
            list.Add("foo", new object());
            list.Add("foo", new object());

            list["foo"] = values;

            Assert.AreEqual(1, list["foo"].Count);
        }

        [TestMethod]
        public void CanEnumerateKeyValueList()
        {
            int count = 0;
            list.Add("foo", new object());
            list.Add("foo", new object());

            foreach (KeyValuePair<string, IList<object>> pair in list)
            {
                foreach (object value in pair.Value)
                {
                    count++;
                }
                Assert.AreEqual("foo", pair.Key);
            }

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void CanGetFlatListOfValues()
        {
            list.Add("foo", new object());
            list.Add("foo", new object());
            list.Add("bar", new object());

            IList<object> values = list.Values;

            Assert.AreEqual(3, values.Count);
        }

        [TestMethod]
        public void IndexerAccessAlwaysSucceeds()
        {
            IList<object> values = list["foo"];

            Assert.IsNotNull(values);
        }


        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void ThrowsIfContainsKeyNull()
        {
            list.ContainsKey(null);
        }

        [TestMethod]
        public void CanAskContainsKey()
        {
            Assert.IsFalse(list.ContainsKey("foo"));
        }

        [TestMethod]
        public void CanAskContainsValueInAnyList()
        {
            object obj = new object();
            list.Add("foo", new object());
            list.Add("bar", new object());
            list.Add("baz", obj);

            bool contains = list.ContainsValue(obj);

            Assert.IsTrue(contains);
        }

        [TestMethod]
        public void CanClearDictionary()
        {
            list.Add("foo", new object());
            list.Add("bar", new object());
            list.Add("baz", new object());

            list.Clear();

            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void CanGetFilteredValuesByKeys()
        {
            list.Add("foo", new object());
            list.Add("bar", new object());
            list.Add("baz", new object());

            IEnumerable<object> filtered = list.FindAllValuesByKey(delegate(string key)
                                                                       {
                                                                           return key.StartsWith("b");
                                                                       });

            int count = 0;
            foreach (object obj in filtered)
            {
                count++;
            }

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void CanGetFilteredValues()
        {
            list.Add("foo", DateTime.Now);
            list.Add("bar", new object());
            list.Add("baz", DateTime.Today);

            IEnumerable<object> filtered = list.FindAllValues(delegate(object value)
                                                                  {
                                                                      return value is DateTime;
                                                                  });
            int count = 0;
            foreach (object obj in filtered)
            {
                count++;
            }

            Assert.AreEqual(2, count);
        }
    }
}