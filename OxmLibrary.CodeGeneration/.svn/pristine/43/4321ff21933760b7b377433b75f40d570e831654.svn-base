// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyDescriptor.cs" company="Mosh Productions.">
//   All rights reserved.
// </copyright>
// <summary>
//   The property descriptor for the oxm generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxmLibrary.CodeGeneration
{
    using System;
    using System.Text;
    using System.Xml.Serialization;
    using System.Xml.Linq;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ClassList = System.Collections.Generic.Dictionary<string, OxmLibrary.CodeGeneration.ClassDescriptor>;
    using System.CodeDom;
    using System.Collections.Generic;

    /// <summary>
    /// The property descriptor.
    /// </summary>
    [Serializable]
    public class PropertyDescriptor : IEquatable<PropertyDescriptor>
    {
        private IOxmGenerator generator;

        #region Constructors

        public PropertyDescriptor()
        {
            Comments = string.Empty;
            this.DefaultValueOnly = false;
            IsAttribute = false;
            this.CustomAttributes = new List<CodeAttributeDeclaration>();
            
        }

        public PropertyDescriptor(ClassDescriptor classDesc) : this()
        {
            this.classDesc = classDesc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDescriptor"/> class.
        /// </summary>
        public PropertyDescriptor(IOxmGenerator generator, ClassDescriptor classDesc)
            : this(classDesc)
        {
            this.generator = generator;
        }

        #endregion Constructors

        #region Properties

        private ClassDescriptor classDesc;

        public void SetClassDescriptor(ClassDescriptor cd)
        {
            this.classDesc = cd;
        }

        [XmlIgnore]
        public List<CodeAttributeDeclaration> CustomAttributes { get; private set; }

        [XmlAttribute]
        public string DefaultValue { get; set; }

        [XmlIgnore]
        public bool TypeDescriptionInsideElement { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Complex.
        /// </summary>
        [XmlAttribute]
        public bool Complex { get; set; }

        /// <summary>
        /// Gets or sets Comments.
        /// </summary>
        public string Comments { get; set; }

        [XmlAttribute]
        public bool DefaultValueOnly { get; set; }

        /// <summary>
        /// Gets or sets Contents.
        /// </summary>
        [XmlIgnore]
        public object Contents { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsAttribute.
        /// </summary>
        public bool IsAttribute { get; set; }

        /// <summary>
        /// Gets or sets PMaxCount.
        /// </summary>
        public int PMaxCount { get; set; }

        private string _Pname;

        /// <summary>
        /// Gets or sets PName.
        /// </summary>
        public string PName
        {
            get
            {
                return _Pname;
            }
            set
            {
                if (this.classDesc != null)
                    this.classDesc.ChangeKey(this, value);
                _Pname = value;
            }
        }

        /// <summary>
        /// Gets or sets PType.
        /// </summary>
        public string PType { get; set; }

        /// <summary>
        /// Gets or sets PropType.
        /// </summary>
        [XmlIgnore]
        public Type PropType { get; set; }

        /// <summary>
        /// Gets or sets OverRideName.
        /// </summary>
        public string OverRideName { get; set; }

        #endregion Properties

        #region IEquatable<PropertyDescriptor> Members

        /// <summary>
        /// Check equality to another property descriptor.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// True or false
        /// </returns>
        public bool Equals(PropertyDescriptor other)
        {
            return PType == other.PType && PName == other.PName && IsAttribute == other.IsAttribute;
        }

        #endregion

        public void SetGenerator(IOxmGenerator generator)
        {
            this.generator = generator;
        }

        internal static PropertyDescriptor ElementParser(ClassDescriptor cd, XElement elem, XSchemaCollection schema, ClassList Classes)
        {
            return ElementParser(cd, elem, schema, Classes, string.Empty);
        }

        internal static PropertyDescriptor ElementParser(ClassDescriptor cd, XElement elem, XSchemaCollection schema, ClassList Classes, string keyAdded)
        {
            var type = elem.Attribute("type");
            string key = string.Empty;
            if (elem.Attribute("name") != null)
            {
                key = elem.Name.NamespaceName + elem.Attribute("name").Value + (type != null ? type.Value : string.Empty) + keyAdded;
                if (schema.PropertiesParsedCache.ContainsKey(key))
                    return schema.PropertiesParsedCache[key];
            }

            var prop = internalElementParser(cd, elem, schema, Classes);
            if (prop != null)
            {
                prop.PName = cd.Generator.Provider.CreateValidIdentifier(prop.PName);
            }
            if (key != string.Empty && !schema.PropertiesParsedCache.ContainsKey(key))
                schema.PropertiesParsedCache.Add(key, prop);
            return prop;
        }

        private static PropertyDescriptor internalElementParser(ClassDescriptor cd, XElement elem, XSchemaCollection schema, ClassList Classes)
        {
            var refAtt = elem.Attribute("ref");
            if (refAtt == null)
            {
                refAtt = elem.Attribute("type");
                if (refAtt != null && Regex.IsMatch(refAtt.Value, "(xs)d?:"))
                    refAtt = null;
            }
            var annotation = XSDHelpers.ParseAnnotation(elem);
            PropertyDescriptor prop = new PropertyDescriptor(cd.Generator, cd);
            prop.PMaxCount = 1;
            var maxOccurs = elem.Attribute("maxOccurs");
            if (maxOccurs != null)
            {
                if (maxOccurs.Value.Equals("unbounded", StringComparison.OrdinalIgnoreCase))
                    prop.PMaxCount = 10;
                else if (maxOccurs.Value != null)
                {
                    prop.PMaxCount = int.Parse(maxOccurs.Value);
                }
            }

            if (refAtt != null)
            {
                var referenceValue = refAtt.Value.Split(':').Last();
                XElement xel = schema.Element(a => a.Attribute("name") != null && a != elem && a.Attribute("name").Value == referenceValue);
                if (xel == null)
                    xel = schema.Descendants().Where(a => a.Attribute("name") != null).FirstOrDefault(a => a.Attribute("name").Value == refAtt.Value);
                if (xel != null && xel != elem)
                {
                    var name = elem.Attribute("name");
                    var proper = ElementParser(cd, xel, schema, Classes, name != null ? name.Value : string.Empty);

                    if (prop.DefaultValue != null)
                        proper.DefaultValue = prop.DefaultValue;
                    if (name != null)
                        proper.PName = name.Value;
                    if (maxOccurs != null && maxOccurs.Value.Equals("unbounded", StringComparison.OrdinalIgnoreCase))
                        proper.PMaxCount = 10;
                    return proper;
                }
            }

            prop.PName = elem.Attribute("name").Value;

            var internalComplexType = (elem.Descendants().FirstOrDefault(a => a.Name.LocalName == "complexType"));
            var isComplex = internalComplexType != null || elem.Name.LocalName == "complexType";
            var typeAttribute = elem.Attribute("type");
            if (isComplex && typeAttribute == null)
            {
                typeAttribute = new XAttribute("type", prop.PName);
            }
            var ptype = typeAttribute ?? new XAttribute("type", "xsd:string");
            XElement simpleType = schema.AcquireSimpleType(ptype.Value, elem);

            var hasReference = !Regex.IsMatch(ptype.Value, "(xs)?:");//   
            var oldHasReference = !ptype.Value.StartsWith("xsd");
            prop.Complex = (isComplex || hasReference) && simpleType == null;
            prop.IsAttribute = false;
            var ptypeSplitted = ptype.Value.Split(':');
            prop.PType = isComplex ? prop.PName : (ptypeSplitted.Last());
            prop.OverRideName = hasReference ? prop.PType : prop.PName;
            if (isComplex)
            {
                prop.PType += "oxm";
            }

            prop.TypeDescriptionInsideElement = internalComplexType != null;
            if (simpleType != null)
            {
                var restriction = simpleType.Elements().FirstOrDefault(a => a.Name.LocalName == "restriction");
                if (restriction != null)
                    prop.PType = (restriction.Attribute("base") ?? new XAttribute("base", "xsd:string")).Value.Split(':').Last();
                if (restriction != null)
                {
                    var pattern = restriction.Elements().Where(a => a.Name.LocalName == "pattern").ToList();
                    if (pattern != null && pattern.Count > 0)
                    {
                        prop.PType += ":" + string.Join("|", pattern.Select(a => a.Attribute("value").Value).ToArray());
                    }
                }
            }

            if (prop.PType.EndsWith("enu"))
            {
                prop.PType = cd.ClassName + prop.PType;
                prop.OverRideName = cd.ClassName + prop.OverRideName;
            }

            var defaultAttribute = elem.Attribute("default");
            if (defaultAttribute != null)
            {
                prop.DefaultValue = (prop.PType.EndsWith("enu") ? prop.PType + "." : "") + defaultAttribute.Value;
            }

            prop.Comments = annotation ?? string.Empty;
            return prop;
        }


        internal static PropertyDescriptor ParseAttribute(XElement subElem, XSchemaCollection schema, ClassDescriptor cd)
        {
            var prop = internalParseAttribute(subElem, schema, cd);
            if (prop != null)
            {
                prop.PName = prop.PName.Replace('-', '_');
            }
            return prop;
        }

        private static PropertyDescriptor internalParseAttribute(XElement subElem, XSchemaCollection schema, ClassDescriptor cd)
        {
            var annotation = subElem.Elements().FirstOrDefault(a => a.Name.LocalName == "annotation");

            PropertyDescriptor prop = new PropertyDescriptor(cd.Generator, cd);
            var defaultAttribute = subElem.Attribute("default");
            prop.PMaxCount = 1;
            prop.IsAttribute = true;
            prop.Complex = false;
            prop.Comments = annotation != null ? annotation.Elements().FirstOrDefault().Value : string.Empty;
            var name = subElem.Attribute("name");
            XElement simpleType = null;
            bool hasEnum;
            var referenceAttribute = subElem.Attribute("ref") ?? subElem.Attribute("type");
            if (referenceAttribute != null && !Regex.IsMatch(referenceAttribute.Value, "(xs)d?:"))
            {
                var theAttribute = schema.Element(referenceAttribute.Value);
                if (theAttribute == null)
                    return null;
                simpleType = theAttribute.Elements().FirstOrDefault(a => a.Name.LocalName == "simpleType");
                if (simpleType == null && theAttribute.Name.LocalName == "simpleType")
                {
                    simpleType = theAttribute;
                }
                annotation = theAttribute.Elements().FirstOrDefault(a => a.Name.LocalName == "annotation");
                prop.Comments = annotation != null ? annotation.Elements().FirstOrDefault().Value : string.Empty;
                prop.PName = referenceAttribute.Value;
                hasEnum = simpleType != null ? simpleType.Descendants().Any(a => a.Name.LocalName == "enumeration") : false;
                name = theAttribute.Attribute("name");
                prop.PType = hasEnum ? name.Value + "enu" : "string";
            }
            else
            {
                prop.PName = subElem.Attribute("name").Value;
                simpleType = subElem.Elements().FirstOrDefault(a => a.Name.LocalName == "simpleType");
                hasEnum = simpleType != null ? simpleType.Descendants().Any(a => a.Name.LocalName == "enumeration") : false;
                var theType = subElem.Attribute("type") ?? new XAttribute("type", hasEnum ? prop.PName + "enu" : "string");
                prop.PType = theType.Value.Split(':').Last();
            }

            prop.Complex = hasEnum;
            if (simpleType != null && prop.PType == "string")
            {
                var restriction = simpleType.Elements().FirstOrDefault(a => a.Name.LocalName == "restriction");
                if (restriction != null)
                {
                    var pattern = restriction.Elements().Where(a => a.Name.LocalName == "pattern").ToList();
                    if (pattern != null && pattern.Count > 0)
                    {
                        prop.PType += ":" + string.Join("|", pattern.Select(a => a.Attribute("value").Value).ToArray());
                    }
                }
            }

            prop.OverRideName = prop.PName;

            if (prop.PType.EndsWith("enu"))
            {
                prop.PType = cd.ClassName + prop.PType;
                prop.OverRideName = cd.ClassName + prop.OverRideName;
            }

            if (defaultAttribute != null)
            {
                prop.DefaultValue = (prop.PType.EndsWith("enu") ? prop.PType + "." : "") + defaultAttribute.Value;
            }

            return prop;
        }

        /// <summary>
        /// The add full property.
        /// </summary>
        /// <returns>
        /// The string representation of a full property
        /// </returns>
        public string ToFullProperty(string propertyType, string validationRegex)
        {
            return generator.PropertyToString(this, true, propertyType, validationRegex);
        }

        /// <summary>
        /// The add automatic property.
        /// </summary>
        /// <returns>
        /// The string representation of an automatic property
        /// </returns>
        public string ToAutomaticProperty(string propertyType, string validationRegex)
        {
            return generator.PropertyToString(this, false, propertyType, validationRegex);
        }

        public static PropertyDescriptor FromCodeMemberProperty(CodeMemberProperty property, IOxmGenerator generator, ClassDescriptor cd)
        {
            PropertyDescriptor descriptor = new PropertyDescriptor(generator, cd);
            descriptor.PName = property.Name;
            descriptor.PType = property.Type.ArrayElementType != null ? property.Type.ArrayElementType.BaseType : property.Type.BaseType;
            descriptor.PMaxCount = property.Type.ArrayElementType != null ? 10 : 1;
            descriptor.IsAttribute = false;
            descriptor.Complex = false;
            return descriptor;
        }

        public static PropertyDescriptor FromCodeFieldProperty(CodeMemberField property, IOxmGenerator generator, ClassDescriptor cd)
        {
            PropertyDescriptor descriptor = new PropertyDescriptor(generator, cd);
            descriptor.PName = property.Name;
            descriptor.PType = property.Type.ArrayElementType != null ? property.Type.ArrayElementType.BaseType : property.Type.BaseType;
            descriptor.PMaxCount = property.Type.ArrayElementType != null ? 10 : 1;
            descriptor.IsAttribute = false;
            descriptor.Complex = false;
            return descriptor;
        }

        public CodeParameterDeclarationExpression AsCodeParameter(string CollectionName, FieldDirection direction, Func<string, string> NormalizeTypes)
        {
            CodeParameterDeclarationExpression param = new CodeParameterDeclarationExpression();
            param.Name = PName;
            param.Direction = direction;
            if (PMaxCount < 2)
            {

                param.Type = new CodeTypeReference { ArrayRank = 0, ArrayElementType = null, BaseType = NormalizeTypes(PType), Options = CodeTypeReferenceOptions.GenericTypeParameter };
            }
            else
            {
                param.Type = new CodeTypeReference { ArrayRank = 1, ArrayElementType = new CodeTypeReference { BaseType = NormalizeTypes(PType) }, BaseType = CollectionName, Options = CodeTypeReferenceOptions.GenericTypeParameter };
            }
            return param;
        }

        internal static PropertyDescriptor XMLNSAttribute(XAttribute targetNamespace, IOxmGenerator generator, ClassDescriptor cd)
        {
            PropertyDescriptor descriptor = new PropertyDescriptor(generator, cd);
            descriptor.IsAttribute = true;
            descriptor.DefaultValueOnly = true;
            descriptor.PropType = typeof(string);
            descriptor.PType = "string";
            descriptor.Complex = false;
            descriptor.DefaultValue = targetNamespace.Value;
            descriptor.OverRideName = "xmlns";
            descriptor.PName = "xmlns";
            descriptor.PMaxCount = 1;
            return descriptor;
        }
    }
}