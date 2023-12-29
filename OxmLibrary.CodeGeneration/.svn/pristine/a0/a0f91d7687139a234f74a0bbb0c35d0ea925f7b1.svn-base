using System.IO;
namespace OxmLibrary.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class CsharpCodeTemplates : CodeTemplateBase
    {
        #region Constructors

        public CsharpCodeTemplates()
        {
            typeConversion = LoadConversionsFromText(global::OxmLibrary.CodeGeneration.Properties.Resources.CSharpTypeConversion);
        }

        #endregion Constructors

        #region Properties

        public override string SerializableAttribute
        {
            get
            {
                return "[Serializable()]\r\n";
            }
        }

        public override string DisplayName
        {
            get { return "CSharp"; }
        }

        public override string Language
        {
            get { return "CS"; }
        }
        
        #endregion Properties

        #region Methods

        public override void WriteAssignFieldWithValue(TextWriter sb, PropertyDescriptor pd)
        {
            var value = pd.DefaultValue;
            if (pd.PType.StartsWith("string"))
                value = '"' + value + '"';
            sb.Write("{0} = {1};\r\n", pd.PName, value);
        }

        public override void WriteClassTemplateFooter(TextWriter sb, ClassDescriptor cd)
        {
            sb.WriteLine("}");
        }

        public override string GetAttribute(string attributeName, params string[] parameters)
        {
            var parameter = string.Join(",", parameters);
            return string.Format("[{0}({1})]\r\n", attributeName, parameter);
        }

        public override void InnerWriteClassTemplateHeader(TextWriter sb, ClassDescriptor cd, string InheritsFrom, bool enableDataBinding)
        {
            sb.Write("public partial class {0} {2} {1}", cd.OxmName, InheritsFrom, InheritsFrom != string.Empty ? ":" : string.Empty);
            if (enableDataBinding)
            {
                sb.Write(", INotifyPropertyChanged");
            }
            sb.WriteLine();
            sb.WriteLine(TextHelper.opener);
            if (enableDataBinding)
            {
                sb.WriteLine("public event PropertyChangedEventHandler PropertyChanged;\r\n");
                sb.WriteLine("public void OnPropertyChanged(string propertyname)");
                sb.WriteLine("{");
                sb.WriteLine("if (PropertyChanged != null)");
                sb.WriteLine("{");
                sb.WriteLine("PropertyChanged(this, new PropertyChangedEventArgs(propertyname));");
                sb.WriteLine("}");
                sb.WriteLine("}\r\n");
            }
            if (cd.InheritsFrom != "Nothing")
            {
                sb.Write("public override string ElementName" + TextHelper.LINEDOWN);
                sb.Write("{{\r\nget {{ return \"{0}\";  }} \r\n}}\r\n", cd.ClassName);
            }
        }

        public override void InternalWriteDefaultValueConstructor(TextWriter sb, ClassDescriptor cd)
        {
            sb.Write("public {0}()\r\n", cd.OxmName);
            sb.WriteLine("{");
            foreach (var property in cd.Where(a => !string.IsNullOrEmpty(a.DefaultValue)))
            {
                WriteAssignFieldWithValue(sb, property);
            }
            sb.WriteLine("}");
        }

        public override void WriteDepthZeroConstructor(TextWriter sb, ClassDescriptor cd)
        {
            sb.Write("public {0}(string xml)\r\n", cd.OxmName);
            sb.WriteLine("{");
            sb.Write("XElement _xml = XElement.Parse(xml);");
            sb.WriteLine();            
            sb.Write("this.MapToPackage(_xml, ElementName);");
            sb.WriteLine();
            sb.WriteLine("}");
        }

        public override void WriteEmptyConstructor(TextWriter sb, ClassDescriptor cd)
        {
            sb.Write("public {0}()\r\n", cd.OxmName);
            sb.WriteLine("{");
            sb.WriteLine("}");
        }

        public override void WriteEnumeration(TextWriter sb, EnumerationDescriptor ed)
        {
            sb.WriteLine("[Serializable]");
            sb.Write("public enum {0}enu {{{1}", ed.Name, TextHelper.LINEDOWN);
            int i = 1;
            foreach (var item in ed.ValuesAndComments())
            {

                string value = provider.IsValidIdentifier(item.Key) ? item.Key : ("item" + TextHelper.TrimNonLettersAndDigits(item.Key));
                if (value != item.Key && value == "item") value += "Empty" + (i++).ToString();
                string attribute = value != item.Key
                                       ?
                                           string.Format("[XmlEnumAttribute(\"{0}\")]", item.Key)
                                       : string.Empty;
                sb.Write("   ");
                if (item.Key != ed.Values[0])
                    sb.Write(",");
                if (!string.IsNullOrEmpty(item.Value))
                    sb.Write("\r\n///<summary>{0}</summary>\r\n", item.Value.Trim());
                sb.Write("{0}{1}", attribute, value);
            }

            sb.WriteLine(" }");
        }

        public override void WriteFactoryLayer(TextWriter sb, bool AddSerializeableAttribute, string localName, List<ClassDescriptor> ClassDetails)
        {
            if (AddSerializeableAttribute)
                sb.Write("[Serializable()]" + TextHelper.LINEDOWN);
            sb.Write("public class {0}Factory : ElementBase", localName);
            TextHelper.FactoryName = localName + "Factory";
            sb.WriteLine();
            sb.Write(TextHelper.opener);
            sb.Write("public override string ElementName" + TextHelper.LINEDOWN);
            sb.Write(TextHelper.opener);
            sb.Write("get { return \"Factory Layer\"; }" + TextHelper.LINEDOWN);
            sb.Write(TextHelper.closer);

            sb.Write(
                string.Format("public override ElementBase Produce(string element, string param)"));
            sb.WriteLine();
            sb.Write(TextHelper.opener);
            sb.Write("switch (element)");
            sb.WriteLine();
            sb.Write(TextHelper.opener);
            foreach (ClassDescriptor cls in ClassDetails.Where(a => a.InheritsFrom != "Nothing"))
            {
                sb.Write("case \"{0}\":", cls.ClassName);
                sb.WriteLine();

                sb.Write("   return new {0}({1});", cls.OxmName, cls.Depth == 1 ? "param" : string.Empty);
                sb.WriteLine();
            }

            sb.Write(TextHelper.closer);
            sb.Write("return null;");
            sb.WriteLine();
            sb.Write(TextHelper.closer);
            sb.Write(TextHelper.closer);
        }

        public override void WriteFileTemplateFooter(TextWriter sb)
        {
            sb.WriteLine("}");
        }

        public override void WriteFileTemplateHeader(TextWriter sb, string CurrentNamespace)
        {
            sb.WriteLine("\r\n");
            sb.Write("namespace {0}\r\n", CurrentNamespace);
            sb.WriteLine("{");
        }

        public override void InternalWriteFullProperty(TextWriter sb, PropertyDescriptor pd, List<string> Attributes, string propertyType, string validationRegex
            , GenerationConfiguration config)
        {
            var ptype = propertyType;
            if (pd.PMaxCount > 1)
                ptype = (config.UseCustomCollections ? config.CollectionTypeForGeneration + "<" : string.Empty)
                        + ptype +
                        (config.UseCustomCollections ? ">" : "[]");

            sb.Write("private {0} _{1};\r\n", ptype, pd.PName);
            sb.WriteLine();
            WriteAttributes(sb, pd, validationRegex);
            sb.Write("public {0} {1}\r\n", ptype, pd.PName);
            sb.WriteLine("{");
            sb.Write("get\r\n{");
            sb.WriteLine();
            if (config.UseCustomCollections && pd.PMaxCount > 1)
            {
                sb.Write("if (_{0} == null)\r\n", pd.PName);
                sb.Write("  _{0} = new {1}();\r\n", pd.PName, ptype);
            }
            sb.Write("return _{0};\r\n}}\r\n", pd.PName);
            sb.WriteLine("set\r\n");
            sb.WriteLine("{");
            sb.Write("_{0} = value;\r\n", pd.PName);
            if (config.EnableDataBinding)
            {
                sb.Write("OnPropertyChanged(\"{0}\");\r\n", pd.PName);
            }

            sb.WriteLine("}");
            sb.WriteLine();
            sb.WriteLine("}");
        }

        public override void WriteIndexer(TextWriter sb, string PropertyName, string ClassName)
        {
            sb.Write("public {0}oxm this[int indexer]", ClassName);
            sb.WriteLine();
            sb.WriteLine("{");
            sb.Write("get {{ return {0}[indexer]; }}", PropertyName);
            sb.WriteLine();
            sb.Write("set {{ {0}[indexer] = value; }}", PropertyName);
            sb.WriteLine();
            sb.WriteLine("}");
        }

        public override void WriteInnerTextProperty(TextWriter sb, string ClassName)
        {
            sb.Write("public string {0}InnerText {{ get; set; }}", ClassName);
            sb.WriteLine();
        }

        public override void WriteNameSpaceDecleration(TextWriter sb, string TextWriter)
        {
            sb.Write("using {0};\r\n", TextWriter);
        }

        public override void WriteXmlComment(TextWriter sb, string Comment)
        {
            sb.WriteLine("/// <summary>");
            foreach (var item in Comment.Split(";\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                sb.Write("/// {0}\r\n", item.Replace("\r\n", "\r\n"));
            }
            sb.WriteLine("/// </summary>");
        }

        private void WriteAttributes(TextWriter sb, PropertyDescriptor pd, string validationRegex)
        {
            if (pd.IsAttribute)
            {
                sb.Write("[{0}(\"{1}\")]\r\n", TextHelper.XMLATTRIBUTENAME, pd.PName);
            }

            if (pd.OverRideName != pd.PName && !string.IsNullOrEmpty(pd.OverRideName))
            {
                sb.Write("[{0}(\"{1}\")]\r\n", TextHelper.OVERRIDEATTRIBUTENAME, pd.OverRideName);
            }

            if (!string.IsNullOrEmpty(validationRegex))
            {
                sb.Write("[{2}(@\"{0}\")]{1}", validationRegex, TextHelper.LINEDOWN, TextHelper.OXMREGEXVALIDATOR);
            }
        }

        #endregion Methods

        public override bool DecreaseIndentBeforeWriting(string currentLine)
        {
            return (currentLine.IndexOf('}') != -1 && currentLine.IndexOf('{') == -1);
        }

        public override bool DecreaseIndentAfterWriting(string currentLine)
        {
            return (currentLine.IndexOf('}') != -1 && currentLine.IndexOf('{') != -1);
        }

        public override bool IncreaseIndentAfterWriting(string currentLine)
        {
            return (currentLine.IndexOf('{') != -1);
        }

        public override bool CheckForEmptyLine(string currentLine, string nextLine)
        {
            return (currentLine.IndexOf('}') != -1 && nextLine.Trim() != "}");
        }

        public override void InternalWriteAutomaticProperty(TextWriter sb, PropertyDescriptor pd, List<string> Attributes, string propertyType, string validationRegex, GenerationConfiguration config)
        {
            WriteAttributes(sb, pd, validationRegex);

            var ptype = propertyType;
            if (pd.PMaxCount > 1)
                ptype = (config.UseCustomCollections ? config.CollectionTypeForGeneration + "<" : string.Empty)
                        + propertyType +
                        (config.UseCustomCollections ? ">" : "[]");

            sb.Write("public {0}", ptype);
            sb.Write(" {0} {{ get; set; }}", pd.PName);
            sb.WriteLine();
        }

        public override string TypeOf(string TypeName)
        {
            return string.Format("typeof({0})", TypeName);
        }

        public override string LanguageNameForCodeDOM
        {
            get { return "C#"; }
        }

        public override void OutPropertyTypeName(TextWriter writer, PropertyDescriptor prop)
        {
            writer.Write(this.NormalizeDataType(prop.PType));
            writer.Write(" {0}", prop.PName);
        }
    }
}