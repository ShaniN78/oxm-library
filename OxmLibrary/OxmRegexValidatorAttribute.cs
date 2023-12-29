using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OxmLibrary
{
    public class OxmRegexValidatorAttribute : Attribute
    {
        public string TheRegex { get; private set; }

        public OxmRegexValidatorAttribute(string expression)
        {
            this.TheRegex = expression;
        }        
    }
}
