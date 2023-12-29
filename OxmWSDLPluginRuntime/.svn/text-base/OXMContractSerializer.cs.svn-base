using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Reflection;

namespace OxmLibrary.WSDLRuntime
{
    public class OXMContractSerializer : XmlObjectSerializer
    {
        readonly Type objectType;
        readonly Type wrapperType;
        readonly ElementBase wrapper;
        readonly List<PropertyInfo> props;
        readonly MethodInfo methodInfo;
        readonly ParameterInfo[] parameters;

        public OXMContractSerializer(Type objectType, Type wrapperType, MethodInfo info)
        {
            this.objectType = objectType;

            this.wrapperType = wrapperType;
            if (wrapperType != null)
            {
                this.wrapper = (ElementBase)Activator.CreateInstance(wrapperType);
                props = wrapperType.GetProperties().Where(prop => prop.Name != "ElementName" && prop.Name != "Parent").ToList();
            }
            this.methodInfo = info;
            parameters = info.GetParameters();
        }

        public override bool IsStartObject(System.Xml.XmlDictionaryReader reader)
        {
            return true;
        }

        public override object ReadObject(System.Xml.XmlDictionaryReader reader, bool verifyObjectName)
        {
            //var element = reader.ReadOuterXml();
            //var xel = XElement.Parse(element);

            if (TypeHandling.MyType.IsAssignableFrom(objectType))
            {
                var text = reader.ReadSubtree();
                var xel = XElement.Load(text);
                var instance = (ElementBase)Activator.CreateInstance(objectType);
                return instance.MapToPackage(xel);
            }
            else if (objectType.IsValueType)
                return TypeHandling.Parse(objectType, reader.ReadElementContentAsString());
            else
                return reader.ReadElementContentAsString();
        }

        public override void WriteEndObject(System.Xml.XmlDictionaryWriter writer)
        {
            if (aprop != null) writer.WriteEndElement();
        }

        public override void WriteObjectContent(System.Xml.XmlDictionaryWriter writer, object graph)
        {
            if (graph is ElementBase)
            {
                if (objectType == wrapperType)
                {
                    writer.WriteRaw(((ElementBase)graph).ToString(prefix, true));
                }
                else writer.WriteRaw(((ElementBase)graph).ToStringContents(prefix));
            }
            else
            {
                writer.WriteRaw(graph.ToString());
            }

        }

        ParameterInfo aprop;
        string prefix = null;

        public override void WriteStartObject(System.Xml.XmlDictionaryWriter writer, object graph)
        {
            var backupProp = wrapper != null ? ((ElementBase)wrapper).xmlns : null;
            var xmlnsp = (graph is ElementBase) ? ((ElementBase)graph).xmlns : null;
            aprop = parameters.FirstOrDefault(a => a.ParameterType == graph.GetType());
            
            if (aprop != null)
            {
                var xmlns = xmlnsp ?? backupProp;
                writer.WriteStartElement(aprop.Name, backupProp ?? string.Empty);
                if (xmlns != null)
                {
                    prefix = "a";
                    writer.WriteAttributeString("xmlns", prefix, null, xmlns);// + wrapperType.Namespace);
                }
            }
        }
    }
}
