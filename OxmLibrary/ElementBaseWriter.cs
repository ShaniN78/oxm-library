using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Collections;
using System.Reflection;

namespace OxmLibrary
{
    public class ElementBaseWriter
    {
        private static readonly Dictionary<string, Func<object, string>> toStringDelegates = new Dictionary<string,Func<object,string>>();
        private static readonly Dictionary<string, MethodInfo> xmlStringDelegates = new Dictionary<string, MethodInfo>();


        public static Func<object, string> ToStringDelegateFactory(string dataType)
        {
            return toStringDelegates.ContainsKey(dataType) ? toStringDelegates[dataType] : null;
        }

        public static MethodInfo XmlStringDelegateFactory(string dataType)
        {
            return xmlStringDelegates.ContainsKey(dataType) ? xmlStringDelegates[dataType] : null;
        }

        public static void RegisterToStringDelegate(string typeName, Func<object, string> fun, Type typer)
        {
            if (!toStringDelegates.ContainsKey(typeName))
            {
                toStringDelegates.Add(typeName, fun);
                var method = typeof(XmlConvert).GetMethods(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(a => a.Name == "ToString" && a.GetParameters()[0].ParameterType == typer);
                xmlStringDelegates.Add(typeName, method);
            }
        }

        static ElementBaseWriter()
        {            
            RegisterToStringDelegate("TimeSpan", (a) => a.ToString(), typeof(TimeSpan));
            RegisterToStringDelegate("Boolean", (a) => a.ToString(), typeof(bool));
        }
        
        private ElementBaseWriter()
        {
        }

        private ElementBase factoryElement;
        private bool contentsOnly;
        private ElementBaseWriterSettings settings;
        private string prefix;

        private ElementBaseWriter(ElementBase element, bool contentsOnly, string prefix)
        {
            this.factoryElement = element;
            this.contentsOnly = contentsOnly;
            this.prefix = prefix;
        }
        
        /// <summary>
        /// Produce a string from the ElementBase without using The XElement class
        /// should be ElementBase in order for it to work as expected.
        /// </summary>
        /// <param name="element">Element to write</param>
        /// <param name="contentsOnly">Omit opening tag and closing tag of the outerElement</param>
        /// <param name="prefix">Prefix to add to sub elements - can be null</param>
        /// <param name="appendNamespace">Add namespaces to elements</param>
        /// <returns>Serialized xml of the element</returns>
        public static string WriteNoXElement(ElementBase element, bool contentsOnly, string prefix, bool appendNamespace)
        {
            return WriteNoXElement(element,  new ElementBaseWriterSettings(contentsOnly, prefix, appendNamespace));
        }

        /// <summary>
        /// Produce a string from the ElementBase without using The XElement class.
        /// Should be ElementBase in order for it to work as expected.
        /// </summary>
        /// <param name="element">Element to write</param>
        /// <param name="settings">Writer settings (contentsOnly, prefix, appendNamespace, etc.)</param>
        /// <returns>Serialized xml of the element</returns>
        public static string WriteNoXElement(ElementBase element, ElementBaseWriterSettings settings)
        {
            ElementBaseWriter baseWriter = new ElementBaseWriter(element, settings.contentsOnly, settings.prefix);
            baseWriter.settings = settings;
            StringWriter writer = new StringWriter();
            var properties = GetProperties(element, element);
            if (settings.contentsOnly)
                baseWriter.WriteNoXElementContents(writer, element, properties);
            else
                baseWriter.WriteNoXElement(writer, element, settings.appendNamespace);
            writer.Close();
            return writer.GetStringBuilder().ToString();
        }

        public static string Write(ElementBase element, ElementBase factory, ElementBaseWriterSettings settings)
        {
            ElementBaseWriter baseWriter = new ElementBaseWriter(element, false, null);
            baseWriter.settings = settings;
            StringWriter writer = new StringWriter();
            var properties = GetProperties(element, element);
            baseWriter.WriteNoXElement(writer, element, false);
            writer.Close();
            return writer.GetStringBuilder().ToString();            
        }

        private static readonly ElementBaseWriterSettings DefaultSettings = new ElementBaseWriterSettings { appendNamespace = false, contentsOnly = false, prefix = null, DateTimeFormat = null, TreatDefaultValuesAsNull = false, WrapInnerTextInCDATA = true };

        public static string Write(ElementBase element, ElementBase factory)
        {
            var settings = DefaultSettings;
            return Write(element, factory, settings);
        }

        public void WriteNoXElementContents(TextWriter writer, ElementBase element, ElementTypeMatch properties)
        {            
            properties.FindAllWrappers(a => !a.IsAttribute && a.Contents != null).ToList()
                .ForEach(k =>
                {
                    if (k.IsOxmType)
                    {
                        if (k.IsCollection) //collection of oxm(ElementBase) items writing here
                        {
                            foreach (ElementBase item in k.Contents as IEnumerable)
                            {
                                WriteNoXElement(writer, item, false, k.NativeName);
                            }
                        }
                        else
                        {
                            WriteNoXElement(writer, (ElementBase)k.Contents, false, k.NativeName);
                        }
                    }
                    else //Not a oxm(ElementBase) type writing goes here
                    {
                        if (k.NativeName.EndsWith("InnerText"))
                        {
                            if (settings.WrapInnerTextInCDATA)
                            {
                                writer.Write("<![CDATA[{0}]]>", k.Contents.ToString());
                            }
                            else
                                writer.Write(k.Contents.ToString());
                        }
                        else if (k.IsCollection)
                        {
                            foreach (object item in k.Contents as IEnumerable)
                            {
                                writer.Write("<{0}>{1}</{0}>", k.NativeName, (item ?? string.Empty).ToString());
                            }
                        }
                        else if (!settings.TreatDefaultValuesAsNull || !k.ParseType.IsValueType || !k.Contents.Equals(TypeHandling.DefaultValue(k.ParseType)))
                        {
                            var theMethod = XmlStringDelegateFactory(k.ParseType.Name);
                            var value = (settings.DateTimeFormat != null && k.Contents is DateTime) ? ((DateTime)k.Contents).ToString(settings.DateTimeFormat) :
                                theMethod != null ? theMethod.Invoke(null, new object[] { k.Contents}) : k.Contents.ToString();

                            var strongName = Math.Abs(k.NativeName.Length - k.IName.Length) < 3;
                            //XmlConvert.ToString(value);
                            writer.Write("<{2}{0}>{1}</{2}{0}>", strongName ? k.IName : k.NativeName, value, prefix != null ? prefix + ":" : string.Empty);
                        }
                    }

                });
        }        

        /// <summary>
        /// Produce a string from the ElementBase without using The XElement class.
        /// </summary>
        /// <param name="writer">TextWriter to write the output into</param>
        /// <param name="element">Current element</param>
        /// <param name="properties">Element type match / property wrappers for writing</param>
        /// <param name="appendNamespace">Whether to add xmlns attribute</param>
        public void WriteNoXElement(TextWriter writer, ElementBase element, ElementTypeMatch properties, bool appendNamespace)
        {
            WriteNoXElement(writer, element, properties, appendNamespace, element.ElementName);            
        }

        private void WriteNoXElement(TextWriter writer, ElementBase element, ElementTypeMatch properties, bool appendNamespace, string tagName)
        {
            writer.Write("<{0}", tagName);
            properties.FindAllWrappers(a => a.IsAttribute && a.Contents != null).ToList()
                .ForEach(a => writer.Write(" {0}=\"{1}\"", a.NativeName, (a.DataType.IsEnum ? a.EnumValue(a.Contents.ToString()) : a.Contents.ToString())));
            if (appendNamespace)
                writer.Write(" xmlns=\"{0}\"", element.xmlns);

            writer.Write(">");
            WriteNoXElementContents(writer, element, properties);
            writer.Write("</{0}>", tagName);
        }

        public void WriteNoXElement(TextWriter writer, ElementBase element, bool appendNamespace, string tagName)
        {
            var properties = GetProperties(element, element);
            WriteNoXElement(writer, element, properties, appendNamespace, tagName);
        }

        public void WriteNoXElement(TextWriter writer, ElementBase element, bool appendNamespace)
        {
            WriteNoXElement(writer, element, appendNamespace, element.ElementName);
        }

        public void Write(TextWriter writer, ElementBase element)
        {
            XElement root = new XElement(element.ElementName);
            XDocument doc = new XDocument(new XDeclaration("1.0", "utf8", "true"), root);
            recursiveWriter(root, element);
            writer.WriteLine(doc.ToString());
        }
        
        public string Write(ElementBase element)
        {
            return Write(element, element);
        }

        private void recursiveWriter(XElement root, ElementBase element)
        {
            var properties = GetProperties(element, factoryElement);
            properties.OfType<ElementWritingWrapper>().ToList().ForEach(k =>
            {
                if (k.Contents != null)
                {

                    if (k.IsAttribute)
                    {
                        XAttribute attribute = new XAttribute(k.NativeName, k.Contents.ToString());
                        root.Add(attribute);
                    }
                    else
                    {
                        if (k.IsOxmType)
                        {
                            if (k.IsCollection)
                            {
                                foreach (ElementBase item in k.Contents as IEnumerable)
                                {
                                    XElement newelement = new XElement(k.NativeName);
                                    recursiveWriter(newelement, (ElementBase)item);
                                    root.Add(newelement);
                                }
                            }
                            else
                            {
                                XElement newelement = new XElement(k.NativeName);
                                recursiveWriter(newelement, (ElementBase)k.Contents);
                                root.Add(newelement);
                            }
                        }
                        else
                        {
                            if (k.NativeName.EndsWith("InnerText"))
                            {
                                root.Add(k.Contents.ToString());
                            }
                            else if (k.IsCollection)
                            {
                                foreach (object item in k.Contents as IEnumerable)
                                {
                                    XElement newelement = new XElement(k.NativeName);
                                    newelement.Add(item.ToString());
                                    root.Add(newelement);
                                }
                            }
                            else
                            {
                                XElement newelement = new XElement(k.NativeName);
                                var theMethod = ToStringDelegateFactory(k.ParseType.Name);

                                newelement.Add(theMethod != null ? theMethod(k.Contents) : k.Contents.ToString());
                                root.Add(newelement);
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Get Class Properties and create and ElementTypeMatch collection with key/values before serializing
        /// to XML
        /// </summary>
        /// <param name="element"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        private static ElementTypeMatch GetProperties(ElementBase element, ElementBase factory)
        {
            var Parser = ElementsParseCaching.GetParseDictionary(element.GetType(), element.ElementName);
            if (Parser == null)
                Parser = new ElementTypeMatch();
            var props = Parser.Select(a => new ElementWritingWrapper(a, element));
            Parser = new ElementTypeMatch(props);

            return Parser;
            
        }
    }
}
