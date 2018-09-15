using Prism.Navigation;
using System.Linq;
using Xunit;

namespace Prism.Forms.Tests.Navigation
{
    public class NavigationParametersFixture
    {
        const string _uri = "?id=3&name=brian";
        const string _uriWithNoQuestionMarkDelimiter = "id=3&name=brian";
        const string _uriWithArray = "color=red&color=white&color=blue";
        const string _uriWithJQueryArray = "color[]=red&color[]=white&color[]=blue";

        [Fact]
        public void ParametersParsedFromQuery()
        {
            var parameters = new NavigationParameters(_uri);
            Assert.Equal(2, parameters.Count);
            Assert.True(parameters.ContainsKey("id"));
            Assert.Equal("3", parameters["id"]);
            Assert.True(parameters.ContainsKey("name"));
            Assert.Equal("brian", parameters["name"]);
        }

        [Fact]
        public void ParametersParsedFromQueryWithNoQuestionMarkDelimiter()
        {
            var parameters = new NavigationParameters(_uriWithNoQuestionMarkDelimiter);
            Assert.Equal(2, parameters.Count);
            Assert.True(parameters.ContainsKey("id"));
            Assert.Equal("3", parameters["id"]);
            Assert.True(parameters.ContainsKey("name"));
            Assert.Equal("brian", parameters["name"]);
        }

        [Fact]
        public void ParametersParsedFromQueryWithArray()
        {
            var parameters = new NavigationParameters(_uriWithArray);
            Assert.Equal(3, parameters.Count);
            Assert.Contains("color", parameters.Keys);
        }

        [Fact]
        public void ParametersParsedFromQueryWithJQueryArray()
        {
            var parameters = new NavigationParameters(_uriWithJQueryArray);
            Assert.Equal(3, parameters.Count);
            Assert.Contains("color[]", parameters.Keys);
        }

        [Fact]
        public void EmptyNavigationParametersWhenGivenNull()
        {
            var parameters = new NavigationParameters(null);
            Assert.NotNull(parameters);
            Assert.Equal(0, parameters.Count);
        }

        [Fact]
        public void EmptyNavigationParametersWhenGivenSpace()
        {
            var parameters = new NavigationParameters(" ");
            Assert.NotNull(parameters);
            Assert.Equal(0, parameters.Count);
        }

        [Fact]
        public void EmptyNavigationParametersWhenGivenNoValue()
        {
            var parameters = new NavigationParameters("id");
            Assert.NotNull(parameters);
            Assert.Equal(0, parameters.Count);
        }

        [Fact]
        public void ParametersHaveNoKeysWhenEmpty()
        {
            var parameters = new NavigationParameters();
            Assert.Empty(parameters.Keys);
        }

        [Fact]
        public void CountIsZeroWhenParametersAreEmpty()
        {
            var parameters = new NavigationParameters();
            Assert.Equal(0, parameters.Count);
        }

        [Fact]
        public void CountReturnsNumberOfParameters()
        {
            var parameters = new NavigationParameters($"{_uri}&{_uriWithArray}");
            Assert.Equal(5, parameters.Count);
        }

        [Fact]
        public void GetValueReturnsDefaultWhenGivenInvalidKey()
        {
            var parameters = new NavigationParameters();
            var result = parameters.GetValue<int>("id");
            Assert.Equal(default(int), result);
        }

        [Fact]
        public void TryGetValueReturnsDefaultWhenGivenInvalidKey()
        {
            var parameters = new NavigationParameters();
            var result = parameters.TryGetValue("id", out int value);
            Assert.False(result);
            Assert.Equal(default(int), value);
        }

        [Fact]
        public void GetValueReturnsDefaultWhenParameterValueIsNull()
        {
            var parameters = new NavigationParameters();
            parameters.Add("value", null);
            var result = parameters.GetValue<int>("value");
            Assert.Equal(0, result);
        }

        [Fact]
        public void TryGetValueReturnsDefaultWhenParameterValueIsNull()
        {
            var parameters = new NavigationParameters();
            parameters.Add("value", null);
            var result = parameters.TryGetValue("value", out int value);
            Assert.True(result);
            Assert.Equal(0, value);
        }

        [Fact]
        public void GetValueReturnsTypedParameterWhenParametersParsedFromQuery()
        {
            var parameters = new NavigationParameters(_uri);
            var result = parameters.GetValue<int>("id");
            Assert.IsType<int>(result);
            Assert.Equal(3, result);
        }

        [Fact]
        public void TryGetValueReturnsTypedParameterWhenParametersParsedFromQuery()
        {
            var parameters = new NavigationParameters(_uri);
            int value;
            var result = parameters.TryGetValue("id", out value);
            Assert.True(result);
            Assert.Equal(3, value);
        }

        [Fact]
        public void GetValueReturnsNullWhenParameterValueIsNull()
        {
            var parameters = new NavigationParameters();
            parameters.Add("value", null);
            var result = parameters.GetValue<object>("value");
            Assert.Null(result);
        }

        [Fact]
        public void GetValuesReturnsEmptyArrayWhenGivenNoKey()
        {
            var parameters = new NavigationParameters();
            var result = parameters.GetValues<object>(null);
            Assert.Empty(result);
        }

        [Fact]
        public void GetValuesReturnsEmptyArrayWhenGivenEmptyKey()
        {
            var parameters = new NavigationParameters();
            var result = parameters.GetValues<object>(string.Empty);
            Assert.Empty(result);
        }

        [Fact]
        public void GetValuesReturnsEmptyArrayWhenParametersParsedFromQueryWithInvalidKey()
        {
            var parameters = new NavigationParameters(_uriWithArray);
            var result = parameters.GetValues<object>("id");
            Assert.Empty(result);
        }

        [Fact]
        public void GetValuesReturnsArrayWhenParametersParsedFromQuery()
        {
            var parameters = new NavigationParameters(_uriWithArray);
            var result = parameters.GetValues<object>("color");
            Assert.Equal(3, result.Count());
            Assert.Contains("red", result);
            Assert.Contains("white", result);
            Assert.Contains("blue", result);
        }

        [Fact]
        public void GetValuesReturnsArrayWhenNotUsingQuery()
        {
            var parameters = new NavigationParameters();
            parameters.Add("id", new Person());
            parameters.Add("id", new Person());
            parameters.Add("id", null);
            var result = parameters.GetValues<Person>("id").ToArray();
            Assert.Equal(3, result.Count());
            Assert.IsType<Person>(result[0]);
            Assert.IsType<Person>(result[1]);
            Assert.Null(result[2]);
        }

        [Fact]
        public void GetValuesConvertsValuesToStringWhenGivenUriQuery()
        {
            var parameters = new NavigationParameters(_uriWithArray);
            var result = parameters.GetValues<string>("color").ToArray();
            Assert.Equal(3, result.Count());
            Assert.IsType<string>(result[0]);
            Assert.IsType<string>(result[1]);
            Assert.IsType<string>(result[2]);
        }

        [Fact]
        public void GetValuesConvertsValuesToIntWhenGivenUriQuery()
        {
            var parameters = new NavigationParameters("id=1&id=2&id=3");
            var result = parameters.GetValues<int>("id").ToArray();
            Assert.Equal(3, result.Count());
            Assert.IsType<int>(result[0]);
            Assert.IsType<int>(result[1]);
            Assert.IsType<int>(result[2]);
        }

        [Fact]
        public void GetValueUseParentClassAsTypeParameter()
        {
            var parameters = new NavigationParameters();
            parameters.Add("id", new Child());

            Assert.NotNull(parameters.GetValue<Person>("id"));
        }

        [Fact]
        public void TryGetValueUseParentClassAsTypeParameter()
        {
            var parameters = new NavigationParameters();
            parameters.Add("id", new Child());

            Person value;
            var result = parameters.TryGetValue<Person>("id", out value);
            Assert.True(result);
            Assert.IsType<Child>(value);
        }

        [Fact]
        public void GetValuesUseParentClassAsTypeParameter()
        {
            var parameters = new NavigationParameters();
            parameters.Add("id", new Child());
            parameters.Add("id", new Child());
            parameters.Add("id", new Person());

            var result = parameters.GetValues<Person>("id").ToArray();

            Assert.Equal(3, result.Count());
            Assert.NotNull(result[0]);
            Assert.NotNull(result[1]);
            Assert.NotNull(result[2]);
        }

        [Fact]
        public void ToStringWorksWithNullParameterValues()
        {
            var parameters = new NavigationParameters();
            parameters.Add("id1", 1);
            parameters.Add("id2", null);
            parameters.Add("id3", 3);

            var result = parameters.ToString();

            Assert.Equal("?id1=1&id2=&id3=3", result);
        }
    }

    public class Person
    { }

    public class Child : Person
    { }
}
