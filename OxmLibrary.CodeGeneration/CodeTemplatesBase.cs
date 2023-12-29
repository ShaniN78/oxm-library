using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Reflection;

namespace OxmLibrary.CodeGeneration
{
    public abstract class CodeTemplateBase
    {
        private object generator;
        internal CodeDomProvider provider;
        private Type generatorType;
        private BindingFlags generatorBindingFlags = BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance;
        private CodeGeneratorOptions options;

        public CodeDomProvider Provider
        {
            get { return provider; }
        }

        public Dictionary<string, string> LoadConversionsFromText(string fileName)
        {
            return LoadConversionsFromText(File.Open(fileName, FileMode.Open, FileAccess.Read));
        }

        public Dictionary<string, string> LoadConversionsFromText(byte[] byteArray)
        {
            return LoadConversionsFromText(new MemoryStream(byteArray));
        }

        public Dictionary<string, string> LoadConversionsFromText(Stream anyStream)
        {
            var text = new StreamReader(anyStream).ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var result = (from i in text
                          let sp = i.Split(' ')
                          where sp.Length == 2
                          select sp).ToDictionary(key => key[0], val => val[1]);
            return result;
        }

        public string TypeName
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        internal Dictionary<string, string> typeConversion;

        public Dictionary<string, string> TypeConversion
        {
            get
            {
                return typeConversion;
            }
        }

        public abstract string DisplayName { get; }

        public abstract string Language { get; }

        public abstract string SerializableAttribute { get; }

        public string NormalizeDataType(string xmlDataType)
        {
            var result = xmlDataType.ToLower();
            result = TypeConversion.ContainsKey(result) ? TypeConversion[result] : xmlDataType;
            if (result.StartsWith("System"))
            {
                result = provider.GetTypeOutput(new CodeTypeReference(result, CodeTypeReferenceOptions.GenericTypeParameter));
            }
            return result;
        }

        public abstract bool DecreaseIndentBeforeWriting(string currentLine);

        public abstract bool DecreaseIndentAfterWriting(string currentLine);

        public abstract bool IncreaseIndentAfterWriting(string currentLine);

        public abstract void WriteFileTemplateHeader(TextWriter sb, string CurrentNameSpace);

        public abstract void WriteNameSpaceDecleration(TextWriter sb, string NameSpace);

        public abstract void WriteFileTemplateFooter(TextWriter sb);

        public abstract void InnerWriteClassTemplateHeader(TextWriter sb, ClassDescriptor cd, string InheritsFrom, bool enableDataBinding);

        public void WriteClassTemplateHeader(TextWriter sb, ClassDescriptor cd, string InheritsFrom, bool enableDataBinding)
        {
            if (cd.CustomAttributes.Count > 0)
            {
                var swriter = new System.CodeDom.Compiler.IndentedTextWriter(sb);
                generatorType.GetField("output", generatorBindingFlags).SetValue(generator, swriter);
                CodeAttributeDeclarationCollection collection = new CodeAttributeDeclarationCollection(cd.CustomAttributes.ToArray());
                MethodInfo Generateconstructor = generatorType.GetMethods(generatorBindingFlags).FirstOrDefault(a => a.Name == "GenerateAttributes" && a.GetParameters().Length == 1);
                Generateconstructor.Invoke(generator, new object[] { collection });
            }
            InnerWriteClassTemplateHeader(sb, cd, InheritsFrom, enableDataBinding);
        }

        public abstract void WriteClassTemplateFooter(TextWriter sb, ClassDescriptor cd);

        public abstract string GetAttribute(string attributeName, params string[] parameters);

        public abstract void OutPropertyTypeName(TextWriter writer, PropertyDescriptor prop);

        public void GetClassMethodSignature(TextWriter writer, ClassDescriptor descriptor)
        {
            var firstFlag = true;
            foreach (var param in descriptor.Where(a => !a.DefaultValueOnly))
            {
                if (firstFlag)
                    firstFlag = false;
                else
                    writer.Write(", ");
                this.OutPropertyTypeName(writer, param);
            }
        }

        /// <summary>
        /// Add innerText property to a class descriptor. 
        /// TODO: re factor into class descriptor.
        /// </summary>
        /// <param name="sb">
        /// The class file string builder
        /// </param>
        /// <param name="ClassName">
        /// The name of the class.
        /// </param>        
        public abstract void WriteInnerTextProperty(TextWriter sb, string ClassName);

        public abstract void WriteIndexer(TextWriter sb, string PropertyName, string ClassName);

        public void WriteProperty(TextWriter sb, PropertyDescriptor pd, List<string> Attributes, string propertyType, string validationRegex,
            GenerationConfiguration config)
        {
            if (pd.CustomAttributes.Count > 0)
            {
                var swriter = new System.CodeDom.Compiler.IndentedTextWriter(sb);
                generatorType.GetField("output", generatorBindingFlags).SetValue(generator, swriter);
                CodeAttributeDeclarationCollection collection = new CodeAttributeDeclarationCollection(pd.CustomAttributes.ToArray());
                MethodInfo Generateconstructor = generatorType.GetMethods(generatorBindingFlags).FirstOrDefault(a => a.Name == "GenerateAttributes" && a.GetParameters().Length == 1);
                Generateconstructor.Invoke(generator, new object[] { collection });
            }

            switch (config.AutomaticProperties)
            {
                case PropertiesMode.Automatic:
                    InternalWriteAutomaticProperty(sb, pd, Attributes, propertyType, validationRegex, config);
                    break;
                case PropertiesMode.AllFullButOXM:
                    if (pd.Complex && pd.PType.EndsWith("oxm"))
                        InternalWriteAutomaticProperty(sb, pd, Attributes, propertyType, validationRegex, config);
                    else
                        InternalWriteFullProperty(sb, pd, Attributes, propertyType, validationRegex, config);
                    break;
                case PropertiesMode.AllAutoExceptCollections:
                    if (pd.PMaxCount > 1)
                        InternalWriteFullProperty(sb, pd, Attributes, propertyType, validationRegex, config);
                    else
                        InternalWriteAutomaticProperty(sb, pd, Attributes, propertyType, validationRegex, config);
                    break;
                case PropertiesMode.ValueTypesOnly:
                    throw new NotImplementedException("Mode not implemented yet....");
                case PropertiesMode.Full:
                    InternalWriteFullProperty(sb, pd, Attributes, propertyType, validationRegex, config);
                    break;
            }
        }

        public abstract void InternalWriteFullProperty(TextWriter sb, PropertyDescriptor pd, List<string> Attributes, string propertyType, string validationRegex,
            GenerationConfiguration config);

        public abstract void InternalWriteAutomaticProperty(TextWriter sb, PropertyDescriptor pd, List<string> Attributes, string propertyType, string validationRegex,
            GenerationConfiguration config);

        public abstract void WriteDepthZeroConstructor(TextWriter sb, ClassDescriptor cd);

        public abstract void WriteEmptyConstructor(TextWriter sb, ClassDescriptor cd);

        public abstract void InternalWriteDefaultValueConstructor(TextWriter sb, ClassDescriptor cd);

        internal void GenerateProvider()
        {
            provider = CodeDomProvider.CreateProvider(LanguageNameForCodeDOM);
            var typer = provider.GetType();
            generator = typer.GetField("generator", BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance).GetValue(provider);
            generatorType = generator.GetType();

            options = new CodeGeneratorOptions { BracingStyle = "C", BlankLinesBetweenMembers = true };
            generatorType.GetField("options", generatorBindingFlags).SetValue(generator, options);
        }

        public void WriteDefaultValueConstructor(TextWriter sb, ClassDescriptor cd)
        {
            InternalWriteDefaultValueConstructor(sb, cd);
            CodeTypeDeclaration declaration = new CodeTypeDeclaration(cd.OxmName);

            var swriter = new System.CodeDom.Compiler.IndentedTextWriter(sb);

            generatorType.GetField("output", generatorBindingFlags).SetValue(generator, swriter);
            generatorType.GetField("currentClass", generatorBindingFlags).SetValue(generator, declaration);

            MethodInfo Generateconstructor = generatorType.GetMethod("GenerateConstructor", generatorBindingFlags);

            foreach (var member in cd.AdditionalConstructors)
            {
                Generateconstructor.Invoke(generator, new object[] { member, declaration });
            }
        }

        public abstract void WriteAssignFieldWithValue(TextWriter sb, PropertyDescriptor pd);

        public abstract void WriteFactoryLayer(TextWriter sb, bool AddSerializeableAttribute, string localName, List<ClassDescriptor> ClassDetails);

        public abstract void WriteEnumeration(TextWriter sb, EnumerationDescriptor ed);

        public abstract bool CheckForEmptyLine(string currentLine, string nextLine);

        public abstract string TypeOf(string TypeName);

        public abstract string LanguageNameForCodeDOM { get; }

        /// <summary>
        /// Generate xml document just like this one, just with summary
        /// </summary>
        /// <param name="sb">TextWriter object</param>
        /// <param name="Comment">Single string comment, can be seperated by ; \r \n</param>
        public abstract void WriteXmlComment(TextWriter sb, string Comment);
    }
}
