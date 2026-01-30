namespace OxmLibrary;

/// <summary>Settings for writing ElementBase instances to XML.</summary>
public class ElementBaseWriterSettings
{
    /// <summary>Write only the contents of the element without the opening and closing tag.</summary>
    public bool contentsOnly { get; set; }

    public string prefix { get; set; }

    public bool appendNamespace { get; set; }

    /// <summary>Format string for DateTime values; null uses current culture.</summary>
    public string DateTimeFormat { get; set; }

    /// <summary>Treat value-type defaults as null and omit the tag.</summary>
    public bool TreatDefaultValuesAsNull { get; set; }

    /// <summary>Whether to wrap inner text in CDATA.</summary>
    public bool WrapInnerTextInCDATA { get; set; }

    public ElementBaseWriterSettings()
    {
        WrapInnerTextInCDATA = true;
    }

    public ElementBaseWriterSettings(bool contentsOnly, string prefix, bool appendNamespace)
    {
        WrapInnerTextInCDATA = true;
        this.contentsOnly = contentsOnly;
        this.prefix = prefix;
        this.appendNamespace = appendNamespace;
    }
}
