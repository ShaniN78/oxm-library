using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OxmLibrary
{
    public class OxmOverrideElementNameAttribute : Attribute
    {
        public string ElementName { get; private set; }

        public OxmOverrideElementNameAttribute(string elementName)
        {
            ElementName = elementName;
        }
    }

    public class OxmXmlAttributeAttribute : Attribute
    {
        public string MapTo {get; private set; }

        public OxmXmlAttributeAttribute(string mapTo)
        {
            MapTo = mapTo;
        }

        public OxmXmlAttributeAttribute()
        {
            MapTo = "";
        }
    }
}
