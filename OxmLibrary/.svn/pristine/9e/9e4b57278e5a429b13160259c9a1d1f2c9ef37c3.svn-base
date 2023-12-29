using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OxmLibrary
{
    public class ElementWritingWrapper : ElementTypeMapping
    {
        public object Contents { get; set; }
        
        public ElementWritingWrapper(ElementTypeMapping mapping) : base(mapping)
        {

        }

        public ElementWritingWrapper(ElementTypeMapping mapping, ElementBase extractValue)
            : base(mapping)
        {
            Contents = mapping.GetValue(extractValue);
        }
    }
}
