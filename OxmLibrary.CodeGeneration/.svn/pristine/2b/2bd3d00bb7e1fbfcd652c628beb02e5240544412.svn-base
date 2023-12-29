using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OxmLibrary.CodeGeneration
{
    [Serializable]
    public class EnumerationDescriptor
    {
        private IOxmGenerator generator;

        internal EnumerationDescriptor()
        {

        }

        internal EnumerationDescriptor(IOxmGenerator generator)
        {
            this.generator = generator;
        }

        private EnumerationDescriptor(string name, IOxmGenerator generator) : this(generator)
        {
            Name = name;
        }

        [XmlAttribute]
        public string Name { get; set; }

        public string Comments { get; set; }

        public List<string> Values { get; set; }
        public List<string> ValuesComments { get; set; }

        public static EnumerationDescriptor FromXElement(IOxmGenerator generator, XElement xelement)
        {
            if (xelement != null)
            {
                var name = xelement.Attribute("name").Value;
                var annotation = XSDHelpers.ParseAnnotation(xelement);
                var desc = new EnumerationDescriptor(name, generator);
                desc.Comments = annotation;
                var enums = xelement.Descendants().Where(a => a.Name.LocalName == "enumeration");
                desc.Values = (from el in enums
                               select el.Attribute("value").Value).ToList();
                desc.ValuesComments = (from el in enums
                                       let k = XSDHelpers.ParseAnnotation(el)
                                       select k).ToList();
                return desc;
            }
            return null;
        }

        public IEnumerable<KeyValuePair<string, string>> ValuesAndComments()
        {
            for (int i = 0; i < Values.Count; i++)
            {
                yield return new KeyValuePair<string, string>(Values[i], ValuesComments[i]);
            }
        }              
    }
}