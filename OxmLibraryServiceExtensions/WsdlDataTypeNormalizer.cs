using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxmLibrary.CodeGeneration;
using System.CodeDom;
using OxmLibrary;
using System.IO;

namespace OxmLibrary.WSDLServiceExtensions
{
    public class WsdlDataTypeNormalizer
    {
        private GenerationConfiguration options;
        private OxmGenerator3 generator;

        private CodeNamespaceCollection namespaces;
        public List<CodeTypeDeclaration> TypesToGenerate { get; private set; }
        private TextWriter currentOutput;

        public WsdlDataTypeNormalizer(GenerationConfiguration config, OxmGenerator3 generator, CodeNamespaceCollection namespaces)
        {
            this.options = config;
            this.generator = generator;
            this.namespaces = namespaces;
            TypesToGenerate = new List<CodeTypeDeclaration>();
        }

        public void AssignWriter(TextWriter writer)
        {
            this.currentOutput = writer;
        }

        public string NormalizeDataTypes(CodeTypeReference typeReference, bool AsMethodSignature)
        {
            return NormalizeDataTypes(typeReference.BaseType, AsMethodSignature);
        }

        public string NormalizeDataTypes(string typeName, bool AsMethodSignature)
        {
            string result;
            if (typeName.EndsWith("oxm"))
                return typeName;
            if (TryToGetFromGenerator(typeName, AsMethodSignature, out result))
                return generator.NormalizeDataTypeToSystem(result);
            if (TryToGetFromKnownTypes(typeName, AsMethodSignature, out result))
                return result.EndsWith("oxm") ? result : result + "oxm";
            return generator.NormalizeDataType(typeName);
        }

        public List<CodeParameterDeclarationExpression> GetMethodSignatureParameters(CodeParameterDeclarationExpression previous)
        {
            List<CodeParameterDeclarationExpression> result = new List<CodeParameterDeclarationExpression>();
            var typeName = previous.Type.BaseType;
            if (TryToGenerateFromGenerator(previous, result))
                return result;
            if (TryToGenerateFromKnownTypes(previous, result))
                return result;
            result.Add(previous);
            return result;
        }

        private bool TryToGenerateFromKnownTypes(CodeParameterDeclarationExpression typeName, List<CodeParameterDeclarationExpression> result)
        {
            var type = SearchTypes(typeName.Type.BaseType, namespaces);
            if (type != null)
            {
                AddToGeneratedTypes(type);
                result.Add(typeName);
                return true;
            }
            return false;
        }

        private void AddToGeneratedTypes(CodeTypeDeclaration type)
        {
            if (!TypesToGenerate.Contains(type))
            {
                TypesToGenerate.Add(type);
                FixType(type, namespaces);
            }
        }

        private void FixType(CodeTypeDeclaration generatedTypes, CodeNamespaceCollection collection)
        {
            var TypesToRemove = new List<CodeMemberField>();
            var properties = generatedTypes.Members.OfType<CodeMemberProperty>().ToList();
            ClassDescriptor cd;

            foreach (var field in generatedTypes.Members.OfType<CodeMemberField>())
            {
                if (properties.Any(a => field.Name == a.Name + "Field"))
                {
                    TypesToRemove.Add(field);
                    continue;
                }

                var type = field.Type.BaseType;
                if (generator.TryGetClass(field.Name, out cd))
                {
                    field.Type.BaseType = cd.ClassName;
                    continue;
                }

                var theType = SearchTypes(type, collection);
                if (theType != null)
                {
                    AddToGeneratedTypes(theType);
                    if (!field.Type.BaseType.EndsWith("oxm"))
                        field.Type.BaseType += "oxm";
                }
            }

            foreach (var field in generatedTypes.Members.OfType<CodeMemberProperty>())
            {
                var type = field.Type.BaseType;
                if (generator.TryGetClass(field.Name, out cd))
                {
                    field.Type.BaseType = cd.ClassName;
                    continue;
                }

                var theType = SearchTypes(type, collection);
                if (theType != null)
                {
                    AddToGeneratedTypes(theType);
                    if (!field.Type.BaseType.EndsWith("oxm"))
                        field.Type.BaseType += "oxm";
                }
            }

            TypesToRemove.ForEach(ttr => generatedTypes.Members.Remove(ttr));

            foreach (var constructor in generatedTypes.Members.OfType<CodeConstructor>())
            {
                if (constructor.Parameters.Count > 0)
                {
                    var parametersToRemove = new List<CodeParameterDeclarationExpression>();
                    foreach (var parameter in constructor.Parameters.OfType<CodeParameterDeclarationExpression>())
                    {
                        var type = parameter.Type.BaseType;
                        var theType = SearchTypes(type, collection);
                        if (theType != null)
                        {
                            var typeName = NormalizeDataTypes(theType.Name, false);
                            if (!parameter.Type.BaseType.EndsWith("oxm"))
                                parameter.Type.BaseType += "oxm";
                        }
                    }
                    parametersToRemove.ForEach(pm => constructor.Parameters.Remove(pm));
                }
            }
        }

        private bool TryToGenerateFromGenerator(CodeParameterDeclarationExpression typeName, List<CodeParameterDeclarationExpression> result)
        {
            ClassDescriptor descriptor;
            var probableClassName = typeName.Type.BaseType.Length > 7 ? typeName.Type.BaseType.Substring(0, typeName.Type.BaseType.Length - 7) : typeName.Type.BaseType;
            if (!generator.TryGetClass(typeName.Type.BaseType, out descriptor) && !generator.TryGetClass(probableClassName, out descriptor))
            {
                result = null;
                return false;
            }
            if (options.GetAdditionalParameter<bool>("MessageContract"))
            {
                result.Add(typeName);
                typeName.Type.BaseType = descriptor.OxmName;
                return true;
            }
            else if (descriptor.Count > 0)
            {
                foreach (var item in descriptor.Where(a => !a.Value.DefaultValueOnly))
                {
                    result.Add(item.Value.AsCodeParameter(options.CustomCollectionType, FieldDirection.In, generator.NormalizeDataTypeToSystem));
                }
                return true;
            }
            return true;
        }


        private bool TryToGetFromKnownTypes(string typeName, bool AsMethodSignature, out string result)
        {
            var type = SearchTypes(typeName, namespaces);
            if (type != null)
            {
                AddToGeneratedTypes(type);
                result = type.Name;
                return true;
            }
            result = null;
            return false;
        }

        private CodeTypeDeclaration SearchTypes(string type, CodeNamespaceCollection collection)
        {
            foreach (CodeNamespace codeNamespace in collection)
            {
                foreach (CodeTypeDeclaration typeDeclartion in codeNamespace.Types)
                {
                    if (typeDeclartion.Name == type)
                    {
                        return typeDeclartion;
                    }
                }
            }
            return null;
        }

        private bool TryToGetFromGenerator(string typeName, bool AsMethodSignature, out string result)
        {
            ClassDescriptor descriptor;
            if (typeName.EndsWith("oxm")) typeName = typeName.Substring(0, typeName.Length - 3);
            if (!generator.TryGetClass(typeName, out descriptor))
            {
                result = null;
                return false;
            }
            if (options.GetAdditionalParameter<bool>("MessageContract"))
            {
                result = descriptor.OxmName;
                return true;
            }
            else if (descriptor.Count > 1)
            {
                if (AsMethodSignature)
                {
                    StringWriter writer = new StringWriter();
                    generator.CodeGenerator.GetClassMethodSignature(writer, descriptor);
                    result = writer.GetStringBuilder().ToString();
                    return true;
                }
                else
                {
                    result = generator.NormalizeDataTypeToSystem(descriptor.First(a => !a.Value.DefaultValueOnly).Value.PType);
                    return true;
                }
            }
            result = string.Empty;
            return true;
        }
    }
}
