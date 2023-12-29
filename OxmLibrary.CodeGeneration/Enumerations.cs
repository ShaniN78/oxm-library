using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace OxmLibrary.CodeGeneration
{
    public enum PropertiesMode { 
        /// <summary>
        /// All Properties will be full properties
        /// </summary>
        Full, 
        /// <summary>
        /// Only value types (structs) will be full properties
        /// </summary>\
        [Description("Value Types Only")]
        [Browsable(false)]
        ValueTypesOnly,
        /// <summary>
        /// Only Collection (Enumerables) Properties are Full.
        /// </summary>
        [Description("All Auto But collections")]
        AllAutoExceptCollections, 
        /// <summary>
        /// All But oxm classes will be full properties.
        /// </summary>
        [Description("All Full but OXM")]
        AllFullButOXM, 
        /// <summary>
        /// All properties will be automatic properties.
        /// </summary>
        Automatic }

    public enum PropertiesNaming { 
        /// <summary>
        /// Property name will be the bare element name.
        /// </summary>
        ElementName, 
        /// <summary>
        /// When nested inside another type will be prefixed according to the first level element
        /// </summary>
        NestedFirstLevel, 
        /// <summary>
        /// Property will be prefixed with the full path name.
        /// </summary>
        NestedFullPath }

    public enum PluginFileFormatSupport
    {
        /// <summary>
        /// read file from local file system.
        /// </summary>
        FromLocalFile,
        FromUrl,
        FromLocanAndUrl,
        NoFileRequired
    }
}
