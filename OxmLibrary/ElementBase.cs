﻿namespace OxmLibrary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Base Class for all Classes
    /// </summary>
    [Serializable]
    public abstract class ElementBase
    {
        #region Fields

        /// <summary>
        /// A reference to the Parent element
        /// </summary>
        public ElementBase Parent;
        /// <summary>
        /// Element's originating name space
        /// </summary>
        public string xmlns;

        private static readonly MethodInfo addToArray = typeof(ElementBase).GetMethod("AddToArray", BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic);

        #endregion Fields

        #region Properties

        /// <summary>
        /// Name of the class that used to parse the Element
        /// </summary>
        public abstract string ElementName
        {
            get;
        }

        #endregion Properties

        #region Methods

        public ElementBase MapToPackage(XElement item)
        {
            return MapToPackage(null, item, ElementName);
        }

        public ElementBase MapToPackage(XElement item, string ElementName)
        {
            return MapToPackage(null, item, ElementName);
        }

        public ElementBase MapToPackage(ElementBase parent, XElement item)
        {
            return MapToPackage(parent, item, ElementName);
        }

        /// <summary>
        /// Parse the element from an XML string
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public ElementBase MapToPackage(string reader)
        {
            return MapToPackage(new StringReader(reader));
        }

        /// <summary>
        /// Parse the element from a text reader object
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public ElementBase MapToPackage(TextReader reader)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            var xmlreader = XmlReader.Create(reader, settings);
            if (xmlreader.Name == string.Empty)
            {
                xmlreader.Read();
            }
            if (xmlreader.Name == string.Empty)
                return this;
            return MapToPackage(xmlreader);
        }

        public ElementBase MapToPackage(XmlReader reader)
        {
            return MapToPackage(null, reader, reader.Name);
        }

        public ElementBase MapToPackage(ElementBase parent, XmlReader reader, string IName)
        {
            Parent = parent;
            if (ElementName != IName)
                throw new Exception("Invalid Parse request - element name does not match class");
            var type = this.GetType();


            // Get Parsing dictionary for the Class.
            var Parser = ElementsParseCaching.GetParseDictionary(type, IName);

            if (reader.HasAttributes && Parser.ContainsAttributes)
                getAttributes(reader, Parser);

            if (Parser.ElementsCount == 0)
            {
                TryToSetInnerText(reader.ReadElementContentAsString(), IName);
                return this;
            }
            else
                reader.Read();

            // Stop reading when you hit an EndElement of the same depth or the file has prematurely ended.
            // Making it a little resilient to bad XML files.
            while (reader.NodeType != XmlNodeType.EndElement && !reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    // Search for the corrent element in the dictionary, Elements can't conain - in the name
                    var matchElement = Parser[reader.LocalName.Replace('-', '_')];
                    //If it's an unknown element skip
                    if (matchElement == null)
                    {
                        reader.Skip();
                        continue;
                    }

                    if (matchElement.IsCollection)
                    {
                        if (matchElement.InnerType.IsOxmType)
                        {
                            var elemBase = this.Produce(matchElement.InnerType.CollectionTypeName, null).MapToPackage(this, reader, matchElement.InnerType.CollectionTypeName);
                            if (matchElement.IsGenericCollection)
                            {
                                AddToGenericCollection(matchElement, elemBase);
                            }
                            else
                                AddToNormalArray(matchElement, elemBase);
                        }
                        else
                        {
                            if (matchElement.IsGenericCollection)
                                AddToGenericCollection(matchElement, extractValue(reader, matchElement));
                            else
                                AddToNormalArray(matchElement, extractValue(reader, matchElement));
                        }
                    }
                    else
                    {
                        if (matchElement.IsOxmType && !matchElement.ParseType.IsEnum)
                            matchElement.SetMethod.Invoke(this, new object[] { this.Produce(matchElement.IName, null).MapToPackage(this, reader, matchElement.IName) });
                        else
                            matchElement.SetMethod.Invoke(this, new object[] { extractValue(reader, matchElement) });
                    }
                }
                else
                {
                    reader.Read();
                }
            }
            reader.Read();
            return this;
        }

        public ElementBase MapToPackage(ElementBase parent, XElement item, string IName)
        {
            Parent = parent;
            if (IName != ElementName)
                throw new Exception("Not A Package");

            var type = this.GetType();
            var Parser = ElementsParseCaching.GetParseDictionary(type, IName);

            if (Parser.ContainsAttributes)
                getAttributes(item, Parser);

            foreach (var el in item.Elements())
            {
                // Search for the corrent element in the dictionary, Elements can't conain - in the name
                var matchElement = Parser[el.Name.LocalName.Replace('-', '_')];
                if (matchElement == null)
                    continue;
                if (matchElement.IsCollection)
                {
                    if (matchElement.IsOxmType)
                    {
                        if (matchElement.IsGenericCollection)
                        {
                            AddToGenericCollection(matchElement, this.Produce(matchElement.IName, null).MapToPackage(this, el, matchElement.IName));
                        }
                        else
                            AddToNormalArray(matchElement, this.Produce(matchElement.IName, null).MapToPackage(this, el, matchElement.IName));
                    }
                    else
                    {
                        if (matchElement.IsGenericCollection)
                            AddToGenericCollection(matchElement, extractValue(el, matchElement));
                        else
                            AddToNormalArray(matchElement, extractValue(el, matchElement));
                    }
                }
                else
                {
                    if (matchElement.IsOxmType && !matchElement.ParseType.IsEnum)
                        matchElement.SetMethod.Invoke(this, new object[] { this.Produce(matchElement.IName, null).MapToPackage(this, el, matchElement.IName) });
                    else
                        matchElement.SetMethod.Invoke(this, new object[] { extractValue(el, matchElement) });
                }
            }

            if (Parser.ElementsCount == 0)
                TryToSetInnerText(item, IName);
            return this;
        }

        public abstract ElementBase Produce(string element, string param);

        public override string ToString()
        {
            return ElementBaseWriter.WriteNoXElement(this, false, null, false);
        }

        public string ToString(string prefix, bool AppendNapespace)
        {
            return ElementBaseWriter.WriteNoXElement(this, false, prefix, AppendNapespace);
        }

        public string ToStringContents(string prefix)
        {
            return ElementBaseWriter.WriteNoXElement(this, true, prefix, false);
        }

        internal static T[] AddToArray<T>(T[] elements, T item, ElementTypeMapping matchElement)
        {
            if (elements == null)
            {
                elements = new T[1];
            }
            else
                Array.Resize<T>(ref elements, elements.Length + 1);
            elements[elements.Length - 1] = item;
            return elements;
        }

        protected void getAttributes(XElement item, ElementTypeMatch Parser)
        {
            ElementTypeMapping typ = null;
            var attributes = (from at in item.Attributes()
                              where at.Name.LocalName != "xmlns"
                              let success = Parser.TryGetValue(at.Name.LocalName, out typ)
                              where success
                              select new
                              {
                                  Value = at.Value,
                                  Mapping = typ
                              });
            foreach (var attrib in attributes)
            {
                attrib.Mapping.SetMethod.Invoke(this, new object[] { extractValue(attrib.Value, attrib.Mapping) });
                //assignValue(attrib.Setter, attrib.DataType, attrib.Value);
            }
        }

        private void AddToGenericCollection(ElementTypeMapping matchElement, object item, object collection)
        {
            matchElement.InnerType.AddToCollection(this, item, collection);
        }

        private void AddToGenericCollection(ElementTypeMapping matchElement, object item)
        {
            matchElement.InnerType.AddToCollection(this, item);
        }

        private void AddToNormalArray(ElementTypeMapping matchElement, object elementBase)
        {
            var Method = addToArray.MakeGenericMethod(matchElement.DataType.GetElementType());
            var value = matchElement.Setter.GetValue(this, null);
            matchElement.SetMethod.Invoke(this, new object[] { Method.Invoke(this, new object[] { value, elementBase, matchElement }) });
        }

        private object extractValue(XmlReader reader, ElementTypeMapping matchElement)
        {
            if (matchElement.ParseType.Name == "DateTime")
            {
                var str = reader.ReadElementContentAsString();
                var val = DateTime.Parse(str);
                return val;
            }
            if (matchElement.ParseType.IsEnum)
            {
                var val = reader.ReadElementContentAsString();
                if (Enum.IsDefined(matchElement.ParseType, val))
                    return Enum.Parse(matchElement.ParseType, val);
                else if (matchElement.EnumCount > 0)
                    return Enum.GetValues(matchElement.ParseType).GetValue(0);
                else
                    return null;
            }
            var value = reader.ReadElementContentAs(matchElement.ParseType, null);
            return value;
        }

        private object extractValue(string val, ElementTypeMapping Mapping)
        {
            if (Mapping.IsParseTypeString || val == null)
            {
                return val;
            }
            if (Mapping.ParseType.IsEnum)
            {
                if (Enum.IsDefined(Mapping.ParseType, val))
                    return Enum.Parse(Mapping.ParseType, val);
                else if (Mapping.EnumCount > 0)
                    return Enum.GetValues(Mapping.ParseType).GetValue(0);
                else
                    return null;
            }

            var value = TypeHandling.Parse(Mapping.ParseType, val);
            return value;
        }

        private object extractValue(XElement source, ElementTypeMapping Mapping)
        {
            if (Mapping.IsParseTypeString)
            {
                return source.Value;
            }
            if (Mapping.ParseType.IsEnum)
            {
                if (Enum.IsDefined(Mapping.ParseType, source.Value))
                    return Enum.Parse(Mapping.ParseType, source.Value);
            }

            var value = TypeHandling.Parse(Mapping.ParseType, source.Value);
            return value;
        }

        private void getAttributes(XmlReader reader, ElementTypeMatch Parser)
        {
            Parser.FindAll(a => a.IsAttribute)
                .ToList().ForEach(property =>
                {
                    var value = reader.GetAttribute(property.NativeName);
                    property.SetMethod.Invoke(this, new object[] { extractValue(value, property) });
                });
        }

        private void TryToSetInnerText(string item, string IName)
        {
            PropertyInfo InnerText = this.GetType().GetProperty(IName + "InnerText");
            if (InnerText != null)
            {
                InnerText.SetValue(this, item, null);
            }
        }

        private void TryToSetInnerText(XElement item, string IName)
        {
            PropertyInfo InnerText = this.GetType().GetProperty(IName + "InnerText");
            if (InnerText != null)
            {
                InnerText.SetValue(this, item.Value, null);
            }
        }

        #endregion Methods

        #region Other

        //{
        //    get;
        //    set;
        //}

        #endregion Other
    }
}