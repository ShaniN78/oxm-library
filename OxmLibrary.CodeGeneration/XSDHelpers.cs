using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using ClassList = System.Collections.Generic.Dictionary<string, OxmLibrary.CodeGeneration.ClassDescriptor>;
using System.Text.RegularExpressions;

namespace OxmLibrary.CodeGeneration
{
    public class XSDHelpers
    {
        public static void RecursiveParse(IOxmGenerator generator, XElement XEl, ClassList classes, Dictionary<string, EnumerationDescriptor> Enumerations, int depth, XSchemaCollection schema)
        {
            RecursiveParse(generator, XEl, classes, Enumerations, depth, schema, null);
        }

        internal static void RecursiveParse(IOxmGenerator generator, XElement XEl, ClassList classes, Dictionary<string, EnumerationDescriptor> Enumerations, int depth, XSchemaCollection schema, string forcedName)
        {
            RecursiveParse(generator, XEl, classes, Enumerations, depth, schema, null, new List<string>());
        }

        internal static void RecursiveParse(IOxmGenerator generator, XElement XEl, ClassList classes, Dictionary<string, EnumerationDescriptor> Enumerations, int depth, XSchemaCollection schema, string forcedName, List<string> path)
        {            
            var name = XEl.Attribute("name");
            if (name == null)
                throw new ArgumentException("No element name attribute found.", nameof(XEl));
            if (path.Count(a => a == name.Value) > 2)
                return;
            path.Add(name.Value);
            TextHelper.ConsoleWriter.WriteLine("Parsing {0}", name.Value);

            var referencedToBaseType = XEl.Attribute("type");
            if (referencedToBaseType != null)
            {
                var inheritsFrom = referencedToBaseType.Value.Split(':').Last();
                if (!Regex.IsMatch(referencedToBaseType.Value, "(xs)d?:") && !inheritsFrom.Equals(name.Value, StringComparison.OrdinalIgnoreCase))
                {
                    var theType = schema.Descendant(inheritsFrom, a => a.Name.LocalName != "attribute");                    
                    if (theType != null && theType.Name.LocalName != "simpleType")
                    {
                        RecursiveParse(generator, theType, classes, Enumerations, depth++, schema, null, path);
                        if (classes.ContainsKey(inheritsFrom))
                        {
                            var theClass = classes[inheritsFrom];
                            ClassDescriptor cd = new ClassDescriptor(XEl, theClass.Comments, schema, XEl.Attribute("name").Value, classes, generator);
                            cd.InheritsFrom = theClass.ClassName;
                            if (classes.ContainsKey(cd.ClassName))
                            {
                                var old = classes[cd.ClassName];
                                old.Merge(cd);
                            }
                            else
                                classes.Add(cd.ClassName, cd);
                        }
                    }
                }
            }

            if (XEl.Elements().Any(a => a.Name.LocalName == "complexType") || XEl.Name.LocalName == "complexType")
            {
                var referencedType = XEl.Attribute("type");
                
                var complexType = XEl.Name.LocalName == "complexType" ? XEl : XEl.Elements().FirstOrDefault(a => a.Name.LocalName == "complexType");
                if (referencedType != null && complexType == null)
                    complexType = schema.Descendants().FirstOrDefault(a => a.Name.LocalName == "complexType" && a.Attribute("name") != null && a.Attribute("name").Value == name.Value);
                var comments = ParseAnnotation(XEl);

                if (forcedName != null && forcedName.EndsWith("oxm"))
                {
                    forcedName = forcedName.Substring(0, forcedName.Length - 3);
                }
                ClassDescriptor cd = new ClassDescriptor(complexType, comments, schema, forcedName ?? name.Value, classes, generator);
                cd.Depth = depth;
                if (cd.InheritsFrom != String.Empty && cd.InheritsFrom != cd.ClassName)
                {
                    var Celement = schema.Descendant(cd.InheritsFrom);            
                    if (Celement != null)
                        RecursiveParse(generator, Celement, classes, Enumerations, depth + 1, schema, null, path);
                }
                foreach (var complex in cd.Where(a => a.Complex))
                {
                    if (complex.PType.EndsWith("enu"))
                    {
                        if (!Enumerations.ContainsKey(complex.OverRideName))
                        {
                            var fromName = complex.OverRideName;
                            if (complex.OverRideName.StartsWith(cd.ClassName))
                                fromName = fromName.Substring(cd.ClassName.Length);
                            var Celement = XEl.Descendants().FirstOrDefault(a => a.Attribute("name") != null && a.Attribute("name").Value == fromName);
                            if (Celement == null)
                                Celement = schema.Descendant(fromName);
                            if (Celement != null)
                            {
                                EnumerationDescriptor desc = EnumerationDescriptor.FromXElement(generator, Celement);
                                desc.Name = complex.OverRideName;
                                Enumerations.Add(desc.Name, desc);
                            }
                        }
                    }
                    else 
                    {
                        XElement Celement = null;
                        if (complex.TypeDescriptionInsideElement)
                        {
                            var theName = complex.OverRideName;
                            Celement = XEl.Descendants().FirstOrDefault(a => a.Attribute("name") != null && a.Attribute("name").Value == theName && a.Name.LocalName != "attribute");
                        }
                        if (Celement == null)
                            Celement = schema.Element(complex.OverRideName, b => b.Attribute("type") == null && b.Name.LocalName != "attribute");
                        if (Celement == null)
                            Celement = schema.Descendant(complex.OverRideName, a => a.Name.LocalName != "attribute");
                        if (Celement != null)
                            RecursiveParse(generator, Celement, classes, Enumerations, depth + 1, schema, complex.PType, path);
                    }
                }
                
                if (classes.ContainsKey(cd.ClassName))
                {
                    var old = classes[cd.ClassName];
                    old.Merge(cd);
                }
                else
                    classes.Add(cd.ClassName, cd);
            }
        }

        

        internal static void SequenceParser(ClassDescriptor cd, XElement subElem, XSchemaCollection schema, ClassList classes)
        {
            foreach (var elem in subElem.Elements())
            {
                switch (elem.Name.LocalName)
                {
                    case "sequence":
                    case "choice":
                        SequenceParser(cd, elem, schema, classes);
                        break;
                    case "element":
                        var prop = PropertyDescriptor.ElementParser(cd, elem, schema, classes);
                        if (!cd.Contains(prop.PName))
                            cd.Add(prop);
                        break;
                    case "attribute":
                        prop = PropertyDescriptor.ParseAttribute(elem, schema, cd);
                        if (prop != null)
                            cd.Add(prop);
                        break;
                }
            }
        }
        
        internal static string ParseAnnotation(XElement element)
        {
            var annotate = element.Elements().FirstOrDefault(a => a.Name.LocalName == "annotation");
            if (annotate == null)
                return string.Empty;
            return annotate.Elements().First().Value;
        }
                
        internal static void ParseComplexContent(ClassDescriptor cd, XElement subElem, XSchemaCollection schema, ClassList classes)
        {
            //get first directive
            var directive = subElem.Elements().First();
            //handle only extensions - Hierearchy
            switch (directive.Name.LocalName)
            {
                case "extension":
                    cd.InheritsFrom = directive.Attribute("base").Value;
                    if (cd.InheritsFrom.IndexOf(':') > -1)
                        cd.InheritsFrom = string.Empty;
                    SequenceParser(cd, directive, schema, classes);
                    break;
            }
        }
    }
}
