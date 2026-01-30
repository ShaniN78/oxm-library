using System;
using Xunit;
using OxmLibrary;

namespace OxmLibrary.Tests
{
    public class TypeHandlingTests
    {
        [Theory]
        [InlineData("int", true)]
        [InlineData("string", true)]
        [InlineData("bool", true)]
        [InlineData("DateTime", true)]
        [InlineData("float", true)]
        [InlineData("long", true)]
        [InlineData("INVALID", false)]
        public void IsKnownPrimitiveType_ReturnsExpected(string typeName, bool expected)
        {
            Assert.Equal(expected, TypeHandling.IsKnownPrimitiveType(typeName));
        }

        [Fact]
        public void BasicDataTypes_ContainsExpectedTypes()
        {
            var basic = TypeHandling.BasicDataTypes;
            Assert.Contains("string", basic);
            Assert.Contains("int", basic);
            Assert.Contains("bool", basic);
            Assert.Contains("DateTime", basic);
        }

        [Theory]
        [InlineData("42", typeof(int), 42)]
        [InlineData("-1", typeof(int), -1)]
        [InlineData("true", typeof(bool), true)]
        [InlineData("false", typeof(bool), false)]
        public void Parse_PrimitiveTypes_ReturnsExpected(string value, Type type, object expected)
        {
            var result = TypeHandling.Parse(type, value);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Parse_DateTime_ReturnsParsedValue()
        {
            var result = TypeHandling.Parse(typeof(DateTime), "2025-01-30");
            Assert.IsType<DateTime>(result);
            var dt = (DateTime)result;
            Assert.Equal(2025, dt.Year);
            Assert.Equal(1, dt.Month);
            Assert.Equal(30, dt.Day);
        }

        [Fact]
        public void DefaultValue_ValueType_ReturnsDefault()
        {
            var intDefault = TypeHandling.DefaultValue(typeof(int));
            Assert.Equal(0, intDefault);
        }
    }
}
