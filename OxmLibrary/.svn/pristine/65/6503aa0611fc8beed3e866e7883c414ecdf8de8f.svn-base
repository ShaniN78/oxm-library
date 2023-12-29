using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OxmLibrary
{
    public class ElementBaseWriterSettings
    {
        /// <summary>
        /// Just write the contents of the element without the opening and closing tag.
        /// </summary>
        public bool contentsOnly { get; set; }

        public string prefix { get; set; }

        public bool appendNamespace { get; set; }

        /// <summary>
        /// Formatting of the date time, leave null if you want it to use the current culture information.
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// Treat values types default as null, and don't type the tag at all.
        /// </summary>
        public bool TreatDefaultValuesAsNull { get; set; }

        /// <summary>
        /// Gets or sets a value that determines wheter the output should contain CDATA around the innertext
        /// </summary>
        public bool WrapInnerTextInCDATA { get; set; }

        public ElementBaseWriterSettings()
        {
            WrapInnerTextInCDATA = true;
        }

        public ElementBaseWriterSettings(bool contentsOnly, string prefix, bool appendNamespace) : base()
        {
            this.contentsOnly = contentsOnly;
            this.prefix = prefix;
            this.appendNamespace = appendNamespace;
        }

    }
}
