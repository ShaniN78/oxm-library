// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextHelper.cs" company="Mosh Productions.">
//  All rights reserved.
// </copyright>
// <summary>Defines the TextHelper type.</summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace OxmLibrary;

/// <summary>Helper methods for text and static readonly fields used by the code generator.</summary>
public static class TextHelper
{
    /// <summary>Standard namespaces to include in generated code (using directives).</summary>
    public static readonly string[] STANDARDNAMESPACES = { "System", "System.Xml.Linq", "System.Linq", "OxmLibrary", "System.Xml.Serialization" };

    public const string opener = "{\r\n";
    public const string closer = "}\r\n";
    public const string LINEDOWN = "\r\n";
    public const string OXMREGEXVALIDATOR = "OxmRegexValidatorAttribute";
    public const string XMLATTRIBUTENAME = "OxmXmlAttribute";
    public const string OVERRIDEATTRIBUTENAME = "OxmOverrideElementName";

    /// <summary>Factory name used before generating classes.</summary>
    public static string FactoryName { get; set; }

    /// <summary>Error/diagnostic writer for the project.</summary>
    public static TextWriter ConsoleWriter { get; set; }

    /// <summary>Returns a string containing only letter characters from the input.</summary>
    public static string TrimNonLetters(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
        var sb = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            if (char.IsLetter(c))
                sb.Append(c);
        }
        return sb.ToString();
    }

    /// <summary>Returns a string containing only letter and digit characters from the input.</summary>
    public static string TrimNonLettersAndDigits(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
        var sb = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            if (char.IsLetter(c) || char.IsDigit(c))
                sb.Append(c);
        }
        return sb.ToString();
    }

    /// <summary>Returns a string containing only letter characters (LINQ variant).</summary>
    public static string TrimNonLetter2(string input)
    {
        return string.IsNullOrEmpty(input) ? string.Empty : new string(input.Where(char.IsLetter).ToArray());
    }
}
