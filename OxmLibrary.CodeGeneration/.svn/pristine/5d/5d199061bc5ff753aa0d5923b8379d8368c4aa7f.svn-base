// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassDescriptor.cs" company="Mosh Productions">
//   All rights reserved.
// </copyright>
// <summary>
//   Defines the ClassDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace OxmLibrary.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using ClassList = System.Collections.Generic.Dictionary<string, OxmLibrary.CodeGeneration.ClassDescriptor>;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The class descriptor.
    /// </summary>
    [Serializable]
    public class ClassDescriptor :
        KeyedCollection<string, PropertyDescriptor>, 
        IXmlSerializable, IEquatable<ClassDescriptor>
    {
        #region Fields

        private IOxmGenerator generator;
                
        #endregion Fields

        [XmlIgnore]
        public List<CodeAttributeDeclaration> CustomAttributes { get; private set; }

        private List<CodeConstructor> additionalConstructors;

        #region Constructors
           
        public ClassDescriptor()
        {
            additionalConstructors = new List<CodeConstructor>();
            CustomAttributes = new List<CodeAttributeDeclaration>();
            InheritsFrom = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDescriptor"/> class.
        /// </summary>
        public ClassDescriptor(IOxmGenerator generator) : this()
        {
            Aliases = new List<string>();
            this.generator = generator;
        }

        new IEnumerable<PropertyDescriptor> GetEnumerator()
        {
            var k = base.GetEnumerator();
            while (k.MoveNext())
            {
                var item = k.Current;
                if (!item.DefaultValueOnly)
                    yield return item;
            }
        }

        public List<CodeConstructor> AdditionalConstructors { get { return additionalConstructors; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDescriptor"/> class.
        /// </summary>
        /// <param name="complexType">
        /// The complex type.
        /// </param>
        /// <param name="comments">
        /// The comments.
        /// </param>
        /// <param name="schema">
        /// The schema.
        /// </param>
        /// <param name="className">
        /// The class name.
        /// </param>
        public ClassDescriptor(XContainer complexType, string comments, XSchemaCollection schema, string className, ClassList classes, IOxmGenerator generator)
            : this(generator)
        {
            OxmName = className + "oxm";
            if (generator.TargetNamespaceAware)
            {
                var targetNamespace = (complexType.Document.Elements().FirstOrDefault()).Attribute("targetNamespace");
                if (targetNamespace != null)
                {                    
                    this.Add(PropertyDescriptor.XMLNSAttribute(targetNamespace, generator, this));
                }
            }

            Comments = comments;
            foreach (var subElem in complexType.Elements())
            {
                switch (subElem.Name.LocalName)
                {
                    case "choice":
                        XSDHelpers.SequenceParser(this, subElem, schema, classes);
                        break;
                    case "all":
                    case "sequence":
                        XSDHelpers.SequenceParser(this, subElem, schema, classes);
                        break;
                    case "attribute":
                        var t = PropertyDescriptor.ParseAttribute(subElem, schema, this);
                        if (t != null)
                        {
                            Add(t);
                        }

                        break;
                    case "simpleContent":
                        this.ParseSimpleContent(subElem, schema);
                        break;
                    case "complexContent":
                        XSDHelpers.ParseComplexContent(this, subElem, schema, classes);
                        break;
                }
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Code Generator related to this class
        /// </summary>
        [XmlIgnore]
        public OxmLibrary.CodeGeneration.IOxmGenerator Generator
        {
            get { return generator; }
            set { generator = value; }
        }

        /// <summary>
        /// Gets Aliases.
        /// </summary>
        [XmlElement]
        public List<string> Aliases
        {
            get; private set;
        }

        /// <summary>
        /// Gets ClassName.
        /// </summary>
        [XmlAttribute]
        public string ClassName
        {
            get { return OxmName.Substring(0, OxmName.Length - 3); }
        }

        /// <summary>
        /// Gets or sets Comments.
        /// </summary>
        [XmlElement]
        public string Comments
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets Depth.
        /// </summary>
        [XmlAttribute]
        public int Depth
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether HasInnerTextProperty.
        /// </summary>
        [XmlAttribute]
        public bool HasInnerTextProperty
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets InheritsFrom.
        /// </summary>
        [XmlElement]
        public string InheritsFrom
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets OxmName.
        /// </summary>
        [XmlAttribute]
        public string OxmName
        {
            get; set;
        }

        [XmlIgnore]
        public List<PropertyDescriptor> Properties
        {
            get
            {
                return this.ToList();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// The add property method.
        /// </summary>
        /// <param name="propName">
        /// The property name.
        /// </param>
        /// <param name="propType">
        /// The property type.
        /// </param>
        public PropertyDescriptor AddProperty(string propName, string propType)
        {
            var prop = new PropertyDescriptor(generator, this) { PType = propType, PMaxCount = 1, IsAttribute = false, Comments = string.Empty };
            prop.Complex = prop.PType.EndsWith("oxm");
            prop.PName = prop.Complex ? propType.Substring(0, propType.Length - 3) : propName;
            prop.OverRideName = prop.Complex ? propType.Substring(0, propType.Length - 3) : prop.PName;
            Add(prop);
            return prop;
        }

        /// <summary>
        /// Check equality to another class descriptor.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The equals.
        /// </returns>
        public bool Equals(ClassDescriptor other)
        {
            int match = this.Count(a => other.Any(a.Equals));

            // (from PropertyDescriptor a in Values
            // join PropertyDescriptor b in other.Values
            // on a equals b
            // select a).Count();
            int nonMatching = Math.Abs(Math.Max(Count - match, other.Count - match));
            int complexUnMatching = this.Count(a => a.Complex && !other.Any(a.Equals));
            var partialMatch = match >= Math.Max(1, Count - 3) && Count > 1 && nonMatching < 3 &&
                                complexUnMatching == 0;
            var fullMatch = match == Count && match == other.Count;
            return (partialMatch || fullMatch) && InheritsFrom == other.InheritsFrom;
        }

        /// <summary>
        /// The get schema.
        /// </summary>
        /// <returns>
        /// The xml schema for data set creation.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// The read xml.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public void ReadXml(XmlReader reader)
        {
            var className = reader.GetAttribute("ClassName");
            Depth = Convert.ToInt32(reader.GetAttribute("Depth"));
            HasInnerTextProperty = Convert.ToBoolean(reader.GetAttribute("HasInnerTextProperty"));
            OxmName = reader.GetAttribute("OxmName");

            // Move past container
            reader.Read();
            var serializer = new XmlSerializer(typeof(List<string>));
            Aliases = serializer.Deserialize(reader) as List<string>;
            if (reader.Name == "Comments")
            {
                Comments = reader.ReadElementString("Comments");
            }

            if (reader.Name == "InheritsFrom")
            {
                InheritsFrom = reader.ReadElementString("InheritsFrom");
            }

            // Used while De serialization
            serializer = new XmlSerializer(typeof(PropertyDescriptor));
            reader.Read();

            // Deserialize and add the BizEntitiy objects
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                PropertyDescriptor entity;

                entity = serializer.Deserialize(reader) as PropertyDescriptor;
                entity.SetClassDescriptor(this);
                reader.MoveToContent();
                if (entity != null)
                {
                    Add(entity);
                }
            }

            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Properties")
            {
                reader.Read();
            }

            reader.Read();
        }

        /// <summary>
        /// The to string of the class.
        /// </summary>
        /// <returns>
        /// The string represtation of the class.
        /// </returns>
        public override string ToString()
        {
            return generator.ClassToString(this);            
        }

        /// <summary>
        /// The try to add inner text.
        /// </summary>
        /// <param name="p">
        /// The name of the class.
        /// </param>
        /// <returns>
        /// the property descriptor if any, that was created.
        /// </returns>
        public PropertyDescriptor TryToAddInnerText(int p)
        {
            if (Count == 0 && !HasInnerTextProperty)
            {
                var prop = new PropertyDescriptor(generator, this) { Complex = false, PType = "string", PMaxCount = 1 };
                HasInnerTextProperty = true;
                prop.PName = ClassName + "InnerText";
                Add(prop);
                return prop;
            }

            return null;
        }

        /// <summary>
        /// Serialize the classe descriptor as an XML Element.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("ClassName", ClassName);
            writer.WriteAttributeString("Depth", Depth.ToString());
            writer.WriteAttributeString("HasInnerTextProperty", HasInnerTextProperty.ToString());
            writer.WriteAttributeString("OxmName", OxmName);

            // Used while Serialization
            var serializer = new XmlSerializer(typeof(List<string>));
            serializer.Serialize(writer, Aliases);
            writer.WriteElementString("Comments", Comments);
            writer.WriteElementString("InheritsFrom", InheritsFrom);
            serializer = new XmlSerializer(typeof(PropertyDescriptor));

            // Serialize each BizEntity this collection holds
            writer.WriteStartElement("Properties");
            foreach (var key in this.ToList())
            {
                serializer.Serialize(writer, key);
            }

            writer.WriteEndElement();
        }

        internal int OrderInClass(string propertyName)
        {
            int index = 1;
            foreach (var item in this.GetEnumerator())
            {
                if (item.PName == propertyName)
                    return index;
                index++;
            }
            return 0;
        }

        /// <summary>
        /// The merge - merge two different classes.
        /// </summary>
        /// <param name="check">
        /// The check.
        /// </param>
        public void Merge(ClassDescriptor check)
        {
            string innerTextKey = string.Empty;
            foreach (var k in check.Where(a => !Contains(a.PName)))
            {
                if (k.PName.EndsWith("InnerText"))
                {
                    if (!HasInnerTextProperty)
                    {
                        TryToAddInnerText(0);
                    }
                    innerTextKey = k.PName;
                }
            }
            if (innerTextKey != string.Empty)
            {
                check.Remove(innerTextKey);
            }
            //Merge properties and Preserve ordering. Generate a series of Key/Value pairs with the original ordering for each class, perform a union with both classes
            //and order by the original Key.
            var allKeys = this.Select(a => a.PName).Union(check.Select(a => a.PName)).ToList();
            var original = (from i in allKeys
                            let order = this.OrderInClass(i) + check.OrderInClass(i)
                            orderby order
                            select this.Contains(i) ? this[i] : check[i]).ToList();
            
                //this.Select((a, i) => new KeyValuePair<int, KeyValuePair<string, PropertyDescriptor>>(i, a))
                //               .Union(check.Where(a => !ContainsKey(a.Key)).Select((a, i) => new KeyValuePair<int, KeyValuePair<string, PropertyDescriptor>>(i, a)))
                //               .OrderBy(a => a.Key).Select(a => a.Value).ToList();
            this.Clear();
            original.ForEach(a => this.Add(a));
            
            foreach (var orig in check.Where(a => this.Contains(a.PName)))
            {
                var fromThis = this[orig.PName];
                fromThis.PMaxCount = Math.Max(fromThis.PMaxCount, orig.PMaxCount);
            }

            if (!Aliases.Contains(check.ClassName))
            {
                Aliases.Add(check.ClassName);
            }
        }

        internal void ParseSimpleContent(XElement subElem, XSchemaCollection schema)
        {
            var restriction = subElem.Elements().FirstOrDefault(el => el.Name.LocalName == "restriction");
            var inner = TryToAddInnerText(0);
            if (inner != null)
            {
                if (restriction != null)
                {
                    var pattern = restriction.Elements().Where(a => a.Name.LocalName == "pattern").ToList();
                    if (pattern != null && pattern.Count > 0)
                    {
                        inner.PType += ":" + string.Join("|", pattern.Select(a => a.Attribute("value").Value).ToArray());
                    }
                }
            }
            foreach (var att in subElem.Descendants().Where(a => a.Name.LocalName == "attribute"))
            {
                var attribute = PropertyDescriptor.ParseAttribute(att, schema, this);
                if (attribute != null)
                    this.Add(attribute);
            }
        }

        #endregion Methods

        public void DeleteProperty(string item)
        {
            this.Remove(item);
        }

        public void AddConstructor(CodeConstructor member)
        {
            additionalConstructors.Add(member);
        }

        internal void ChangeKey(PropertyDescriptor item, string newKey)
        {
            if (this.Contains(item))
            {
                base.ChangeItemKey(item, newKey);
            }
        }

        protected override string GetKeyForItem(PropertyDescriptor item)
        {
            return item.PName;
        }
    }
}