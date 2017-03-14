using System;
using Prism.Mvvm;
using Xunit;

namespace Prism.Tests.Mvvm
{
    public class PropertySupportFixture
    {
        [Fact]
        public virtual void WhenExtractingNameFromAValidPropertyExpression_ThenPropertyNameReturned()
        {
            var propertyName = PropertySupport.ExtractPropertyName(() => this.InstanceProperty);
            Assert.Equal("InstanceProperty", propertyName);
        }

        [Fact]
        public void WhenExpressionRepresentsAStaticProperty_ThenExceptionThrown()
        {
            Assert.Throws<ArgumentException>(() => PropertySupport.ExtractPropertyName(() => StaticProperty));
        }

        [Fact]
        public void WhenExpressionIsNull_ThenAnExceptionIsThrown()
        {
            Assert.Throws<ArgumentNullException>(() => PropertySupport.ExtractPropertyName<int>(null));
        }

        [Fact]
        public void WhenExpressionRepresentsANonMemberAccessExpression_ThenAnExceptionIsThrown()
        {
            Assert.Throws<ArgumentException>(() => PropertySupport.ExtractPropertyName(() => GetHashCode()));
        }

        [Fact]
        public void WhenExpressionRepresentsANonPropertyMemberAccessExpression_ThenAnExceptionIsThrown()
        {
            Assert.Throws<ArgumentException>(() => PropertySupport.ExtractPropertyName(() => InstanceField));
        }

        public static int StaticProperty { get; set; }
        public int InstanceProperty { get; set; }
        public int InstanceField;
        public static int SetOnlyStaticProperty { set { } }
    }
}
