// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextHelper.cs" company="Mosh Productions.">
//  All rights reserved. 
// </copyright>
// <summary>
//   Defines the TextHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxmLibrary
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Helping method for textual purposes and static readonly fields.
    /// </summary>
    public static class TextHelper
    {
        /// <summary>
        /// all the standard namespace to imprt/using
        /// </summary>        
        public static readonly string[] STANDARDNAMESPACES = { "System", "System.Xml.Linq" ,"System.Linq", "OxmLibrary", "System.Xml.Serialization" };
        public const string opener = "{\r\n";
        public const string closer = "}\r\n";
        public const string LINEDOWN = "\r\n";
        public const string OXMREGEXVALIDATOR = "OxmRegexValidatorAttribute";
        public const string XMLATTRIBUTENAME = "OxmXmlAttribute";
        public const string OVERRIDEATTRIBUTENAME = "OxmOverrideElementName";

        /// <summary>
        /// Gets or sets FactoryName before generating classes.
        /// </summary>
        public static string FactoryName { get; set; }

        /// <summary>
        /// Gets or sets errorWriter for the whole project.
        /// </summary>
        public static TextWriter ConsoleWriter { get; set; }

        /// <summary>
        /// Trim all non letters from a string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string TrimNonLetters(string input)
        {
            var sb = new StringBuilder(input.Length);
            foreach (var item in input)
            {
                if (char.IsLetter(item))
                    sb.Append(item);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Trim all non letters from a string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string TrimNonLettersAndDigits(string input)
        {
            var sb = new StringBuilder(input.Length);
            foreach (var item in input)
            {
                if (char.IsLetter(item) || char.IsDigit(item))
                    sb.Append(item);
            }
            return sb.ToString();
        }

        public static string TrimNonLetter2(string input)
        {
            return new string(input.Where(char.IsLetter).ToArray());
        }
    }
}
