using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Regions;
using System.Linq;

namespace Prism.Wpf.Tests.Regions
{
    [TestClass]
    public class NavigationParametersFixture
    {
        const string _uri = "?id=3&name=brian";
        const string _uriWithNoQuestionMarkDelimiter = "id=3&name=brian";
        const string _uriWithArray = "color=red&color=white&color=blue";
        const string _uriWithJQueryArray = "color[]=red&color[]=white&color[]=blue";

        [TestMethod]
        public void ParametersParsedFromQuery()
        {
            var parameters = new NavigationParameters(_uri);
            Assert.AreEqual(2, parameters.Count);
            Assert.IsTrue(parameters.ContainsKey("id"));
            Assert.AreEqual("3", parameters["id"]);
            Assert.IsTrue(parameters.ContainsKey("name"));
            Assert.AreEqual("brian", parameters["name"]);
        }

        [TestMethod]
        public void ParametersParsedFromQueryWithNoQuestionMarkDelimiter()
        {
            var parameters = new NavigationParameters(_uriWithNoQuestionMarkDelimiter);
            Assert.AreEqual(2, parameters.Count);
            Assert.IsTrue(parameters.ContainsKey("id"));
            Assert.AreEqual("3", parameters["id"]);
            Assert.IsTrue(parameters.ContainsKey("name"));
            Assert.AreEqual("brian", parameters["name"]);
        }

        [TestMethod]
        public void ParametersParsedFromQueryWithArray()
        {
            var parameters = new NavigationParameters(_uriWithArray);
            Assert.AreEqual(3, parameters.Count);
            CollectionAssert.Contains(parameters.Keys.ToArray(), "color");
        }

        [TestMethod]
        public void ParametersParsedFromQueryWithJQueryArray()
        {
            var parameters = new NavigationParameters(_uriWithJQueryArray);
            Assert.AreEqual(3, parameters.Count);
            CollectionAssert.Contains(parameters.Keys.ToArray(), "color[]");
        }

        [TestMethod]
        public void EmptyNavigationParametersWhenGivenNull()
        {
            var parameters = new NavigationParameters(null);
            Assert.IsNotNull(parameters);
            Assert.AreEqual(0, parameters.Count);
        }

        [TestMethod]
        public void EmptyNavigationParametersWhenGivenSpace()
        {
            var parameters = new NavigationParameters(" ");
            Assert.IsNotNull(parameters);
            Assert.AreEqual(0, parameters.Count);
        }

        [TestMethod]
        public void EmptyNavigationParametersWhenGivenNoValue()
        {
            var parameters = new NavigationParameters("id");
            Assert.IsNotNull(parameters);
            Assert.AreEqual(0, parameters.Count);
        }

        [TestMethod]
        public void ParametersHaveNoKeysWhenEmpty()
        {
            var parameters = new NavigationParameters();
            Assert.AreEqual(0, parameters.Keys.Count());
        }

        [TestMethod]
        public void CountIsZeroWhenParametersAreEmpty()
        {
            var parameters = new NavigationParameters();
            Assert.AreEqual(0, parameters.Count);
        }

        [TestMethod]
        public void CountReturnsNumberOfParameters()
        {
            var parameters = new NavigationParameters($"{_uri}&{_uriWithArray}");
            Assert.AreEqual(5, parameters.Count);
        }

        [TestMethod]
        public void GetValueReturnsDefaultWhenGivenInvalidKey()
        {
            var parameters = new NavigationParameters();
            var result = parameters.GetValue<int>("id");
            Assert.AreEqual(default(int), result);
        }

        [TestMethod]
        public void TryGetValueReturnsDefaultWhenGivenInvalidKey()
        {
            var parameters = new NavigationParameters();
            int value;
            var result = parameters.TryGetValue("id", out value);
            Assert.AreEqual(false, result);
            Assert.AreEqual(default(int), value);
        }

        [TestMethod]
        public void GetValueReturnsDefaultWhenParameterValueIsNull()
        {
            var parameters = new NavigationParameters();
            parameters.Add("value", null);
            var result = parameters.GetValue<int>("value");
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TryGetValueReturnsDefaultWhenParameterValueIsNull()
        {
            var parameters = new NavigationParameters();
            parameters.Add("value", null);
            int value;
            var result = parameters.TryGetValue("value", out value);
            Assert.AreEqual(true, result);
            Assert.AreEqual(0, value);
        }

        [TestMethod]
        public void GetValueReturnsTypedParameterWhenParametersParsedFromQuery()
        {
            var parameters = new NavigationParameters(_uri);
            var result = parameters.GetValue<int>("id");
            Assert.IsInstanceOfType(result, typeof(int));
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TryGetValueReturnsTypedParameterWhenParametersParsedFromQuery()
        {
            var parameters = new NavigationParameters(_uri);
            int value;
            var result = parameters.TryGetValue("id", out value);
            Assert.AreEqual(true, result);
            Assert.AreEqual(3, value);
        }

        [TestMethod]
        public void GetValueReturnsNullWhenParameterValueIsNull()
        {
            var parameters = new NavigationParameters();
            parameters.Add("value", null);
            var result = parameters.GetValue<object>("value");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetValuesReturnsEmptyArrayWhenGivenNoKey()
        {
            var parameters = new NavigationParameters();
            var result = parameters.GetValues<object>(null);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetValuesReturnsEmptyArrayWhenGivenEmptyKey()
        {
            var parameters = new NavigationParameters();
            var result = parameters.GetValues<object>(string.Empty);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetValuesReturnsEmptyArrayWhenParametersParsedFromQueryWithInvalidKey()
        {
            var parameters = new NavigationParameters(_uriWithArray);
            var result = parameters.GetValues<object>("id");
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetValuesReturnsArrayWhenParametersParsedFromQuery()
        {
            var parameters = new NavigationParameters(_uriWithArray);
            var result = parameters.GetValues<object>("color");
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Contains("red"));
            Assert.IsTrue(result.Contains("white"));
            Assert.IsTrue(result.Contains("blue"));
        }

        [TestMethod]
        public void GetValuesReturnsArrayWhenNotUsingQuery()
        {
            var parameters = new NavigationParameters();
            parameters.Add("id", new Person());
            parameters.Add("id", new Person());
            parameters.Add("id", null);
            var result = parameters.GetValues<Person>("id").ToArray();
            Assert.AreEqual(3, result.Count());
            Assert.IsInstanceOfType(result[0], typeof(Person));
            Assert.IsInstanceOfType(result[1], typeof(Person));
            Assert.IsNull(result[2]);
        }

        [TestMethod]
        public void GetValuesConvertsValuesToStringWhenGivenUriQuery()
        {
            var parameters = new NavigationParameters(_uriWithArray);
            var result = parameters.GetValues<string>("color").ToArray();
            Assert.AreEqual(3, result.Count());

            Assert.IsInstanceOfType(result[0], typeof(string));
            Assert.IsInstanceOfType(result[1], typeof(string));
            Assert.IsInstanceOfType(result[2], typeof(string));
        }

        [TestMethod]
        public void GetValuesConvertsValuesToIntWhenGivenUriQuery()
        {
            var parameters = new NavigationParameters("id=1&id=2&id=3");
            var result = parameters.GetValues<int>("id").ToArray();
            Assert.AreEqual(3, result.Count());
            Assert.IsInstanceOfType(result[0], typeof(int));
            Assert.IsInstanceOfType(result[1], typeof(int));
            Assert.IsInstanceOfType(result[2], typeof(int));
        }

        [TestMethod]
        public void GetValueUseParentClassAsTypeParameter()
        {
            var parameters = new NavigationParameters();
            parameters.Add("id", new Child());

            Assert.IsNotNull(parameters.GetValue<Person>("id"));
        }

        [TestMethod]
        public void TryGetValueUseParentClassAsTypeParameter()
        {
            var parameters = new NavigationParameters();
            parameters.Add("id", new Child());

            Person value;
            var result = parameters.TryGetValue<Person>("id", out value);
            Assert.IsTrue(result);
            Assert.IsInstanceOfType(value, typeof(Child));
        }

        [TestMethod]
        public void GetValuesUseParentClassAsTypeParameter()
        {
            var parameters = new NavigationParameters();
            parameters.Add("id", new Child());
            parameters.Add("id", new Child());
            parameters.Add("id", new Person());

            var result = parameters.GetValues<Person>("id").ToArray();

            Assert.AreEqual(3, result.Count());
            Assert.IsNotNull(result[0]);
            Assert.IsNotNull(result[1]);
            Assert.IsNotNull(result[2]);
        }
    }

    public class Person
    { }

    public class Child : Person
    { }
}
