using System.Xml.Linq;
using Xunit;
using OxmLibrary;

namespace OxmLibrary.Tests
{
    /// <summary>
    /// Minimal concrete ElementBase for testing MapToPackage and ToString round-trip.
    /// ElementBase looks for a property named {ElementName}InnerText for simple content.
    /// </summary>
    public sealed class TestElement : ElementBase
    {
        public string TestElementInnerText { get; set; }
        public override string ElementName => "TestElement";

        public override ElementBase Produce(string element, string param)
        {
            if (element == "TestElement")
                return new TestElement();
            return null;
        }
    }

    public class ElementBaseTests
    {
        [Fact]
        public void MapToPackage_FromXElement_WithSimpleContent_PopulatesInnerText()
        {
            var el = XElement.Parse("<TestElement>hello</TestElement>");
            var instance = new TestElement();
            instance.MapToPackage(el);
            Assert.Equal("hello", instance.TestElementInnerText);
        }

        [Fact]
        public void MapToPackage_FromXmlString_WithSimpleContent_PopulatesInnerText()
        {
            var xml = "<TestElement>world</TestElement>";
            var instance = new TestElement();
            instance.MapToPackage(xml);
            Assert.Equal("world", instance.TestElementInnerText);
        }

        [Fact]
        public void ToString_RoundTrip_MatchesInput()
        {
            var xml = "<TestElement>roundtrip</TestElement>";
            var instance = new TestElement();
            instance.MapToPackage(xml);
            var written = instance.ToString();
            Assert.Contains("roundtrip", written);
            Assert.Contains("TestElement", written);
        }
    }
}
