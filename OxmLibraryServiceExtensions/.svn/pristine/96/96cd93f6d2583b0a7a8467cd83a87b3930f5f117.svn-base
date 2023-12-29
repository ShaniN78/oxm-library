using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel.Description;

using System.CodeDom.Compiler;
using System.CodeDom;
using OxmLibrary;
using System.ServiceModel;
using OxmLibrary.CodeGeneration;

namespace OxmLibrary.WSDLServiceExtensions
{
    public class WsdlCodeWriting
    {
        List<ContractDescription> contracts;
        List<CodeTypeDeclaration> TypesToGenerate;
        private List<CodeCompileUnit> CompileUnits;
        private OxmGenerator3 oxmGenerator;
        private CodeDomProvider provider;
        private WsdlDataTypeNormalizer currentNormalizer;
        private WsdlImporting importer;
        private bool CreateMessageContract;

        public WsdlCodeWriting(WsdlImporting importer, IEnumerable<ContractDescription> contracts, OxmGenerator3 oxmGenerator)
        {
            this.contracts = contracts.ToList();
            CompileUnits = new List<CodeCompileUnit>();
            this.oxmGenerator = oxmGenerator;
            provider = CodeDomProvider.CreateProvider(oxmGenerator.CodeGenerator.LanguageNameForCodeDOM);
            this.importer = importer;
        }

        public CodeAttributeDeclaration CodeAttribute(string wrapperName, CodeTemplateBase CodeGenerator)
        {
            CodeAttributeDeclaration declare = new CodeAttributeDeclaration("OxmLibrary.OXMContractSerializer"
                , new CodeAttributeArgument(new CodeSnippetExpression(CodeGenerator.TypeOf(wrapperName)))
                );
            return declare;
        }

        public string PreProcess(OxmGenerator3 oxmGenerator, ServiceContractGenerator generator)
        {
            if (contracts.Count > 0)
            {
                var generate = generator.GenerateServiceContractType(contracts[0]);
                var targetUnit = generator.TargetCompileUnit;
                var nonNullNamespace = targetUnit.Namespaces.OfType<CodeNamespace>().FirstOrDefault(a => !string.IsNullOrEmpty(a.Name));
                if (nonNullNamespace != null)
                    return nonNullNamespace.Name;
            }
            return string.Empty;
        }

        public List<CodeTypeDeclaration> PrepareTypes(OxmGenerator3 oxmGenerator, ServiceContractGenerator generator)
        {
            CreateMessageContract = oxmGenerator.Config.AdditionalParameters.Get<bool>("MessageContract");
            TypesToGenerate = new List<CodeTypeDeclaration>();

            List<string> GeneratedContracts = new List<string>();

            foreach (var contract in contracts)
            {
                CodeCompileUnit unit = new CodeCompileUnit();
                var typer = generator.GenerateServiceContractType(contract);
                var codeUnit = generator.TargetCompileUnit;
                var MyNamespace = codeUnit.Namespaces.OfType<CodeNamespace>().FirstOrDefault(a => a.Name == contract.Namespace);
                if (MyNamespace == null)
                    MyNamespace = codeUnit.Namespaces[0];
                if (!unit.Namespaces.Contains(MyNamespace))
                    unit.Namespaces.Add(MyNamespace);

                currentNormalizer = new WsdlDataTypeNormalizer(oxmGenerator.Config, oxmGenerator, codeUnit.Namespaces);

                var myInterface = MyNamespace.Types.OfType<CodeTypeDeclaration>().FirstOrDefault(a => a.IsInterface && a.Name == contract.Name);

                if (myInterface == null || GeneratedContracts.Contains(myInterface.Name))
                    continue;
                GeneratedContracts.Add(myInterface.Name);
                var myImplementation = MyNamespace.Types.OfType<CodeTypeDeclaration>().FirstOrDefault(a => a.IsClass && a.BaseTypes.OfType<CodeTypeReference>().Any(b => b.BaseType == myInterface.Name));
                var TypeList = MyNamespace.Types.OfType<CodeTypeDeclaration>().ToList();
                FixType(codeUnit.Namespaces, oxmGenerator, myInterface, TypeList, true, null);
                if (myImplementation != null)
                    FixType(codeUnit.Namespaces, oxmGenerator, myImplementation, TypeList, false, myInterface);

                MyNamespace.Types.Clear();
                MyNamespace.Types.Add(myInterface);
                if (myImplementation != null)
                    MyNamespace.Types.Add(myImplementation);
                CompileUnits.Add(unit);
                TypesToGenerate.AddRange(currentNormalizer.TypesToGenerate);
            }

            return TypesToGenerate;
        }

        public void Write(TextWriter iWriter)
        {
            IndentedTextWriter indented = new IndentedTextWriter(iWriter);
            CodeGeneratorOptions options = new CodeGeneratorOptions { BracingStyle = "C", BlankLinesBetweenMembers = true };
            foreach (var unit in CompileUnits)
            {
                provider.GenerateCodeFromCompileUnit(unit, indented, options);
            }
        }

        private void FixType(CodeNamespaceCollection collection, OxmGenerator3 oxmGenerator, CodeTypeDeclaration myInterface, List<CodeTypeDeclaration> typeList, bool fixAttributes, CodeTypeDeclaration fromInterface)
        {
            var methods = myInterface.Members.OfType<CodeMemberMethod>().ToList();
            List<CodeTypeDeclaration> GenerateTheTypes = new List<CodeTypeDeclaration>();
            foreach (var method in methods)
            {
                FixMethodSignature(oxmGenerator, method, typeList, collection, fixAttributes, fromInterface);
            }
        }

        private CodeTypeDeclaration SearchTypes(string type, CodeNamespaceCollection collection)
        {
            if (type.StartsWith("System"))
                return null;
            ClassDescriptor descript;
            if (oxmGenerator.TryGetClass(type, out descript))
                return null;
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

        private void FixMethodSignature(OxmGenerator3 oxmGenerator, CodeMemberMethod method, List<CodeTypeDeclaration> typeList, CodeNamespaceCollection collection, bool fixAttributes, CodeTypeDeclaration fromInterface)
        {
            if (fixAttributes)
                FixAttributes(method);
            if (!method.ReturnType.BaseType.StartsWith("System"))
            {
                var checkType = currentNormalizer.NormalizeDataTypes(method.ReturnType.BaseType, false);
                method.ReturnType.BaseType = checkType;
            }
            //((System.CodeDom.CodeMethodInvokeExpression)(((System.CodeDom.CodeMethodReturnStatement)(method.Statements[0])).Expression))
            if (fromInterface != null && method.Name != ".ctor")
            {
                CodeMethodInvokeExpression codeInvoke = null;
                foreach (var item in method.Statements.OfType<CodeMethodReturnStatement>())
                {
                    if (item.Expression is CodeMethodInvokeExpression)
                    {
                        codeInvoke = (CodeMethodInvokeExpression)item.Expression;
                        var fromMethod = fromInterface.Members.OfType<CodeMemberMethod>().FirstOrDefault(a => a.Name == codeInvoke.Method.MethodName);
                        codeInvoke.Parameters.Clear();
                        foreach (var parameter in fromMethod.Parameters.OfType<CodeParameterDeclarationExpression>())
                        {
                            codeInvoke.Parameters.Add(new CodeSnippetExpression(parameter.Name));
                        }
                    }
                }
                if (codeInvoke != null && codeInvoke.Parameters.Count == 0)
                    method.Statements[0] = new CodeExpressionStatement(codeInvoke);

            }
            List<CodeParameterDeclarationExpression> ToDelete = new List<CodeParameterDeclarationExpression>();
            List<CodeParameterDeclarationExpression> ToAdd = new List<CodeParameterDeclarationExpression>();
            foreach (CodeParameterDeclarationExpression parameter in method.Parameters)
            {
                if (parameter.Type.BaseType.StartsWith("System"))
                    continue;
                var normalized = currentNormalizer.NormalizeDataTypes(parameter.Type.BaseType, true);
                if (string.IsNullOrEmpty(normalized))
                {
                    ToDelete.Add(parameter);
                }
                else if (oxmGenerator.Config.AdditionalParameters.Get<bool>("MessageContract"))
                {
                    parameter.Type.BaseType = normalized;
                }
                else
                {
                    var parameters = currentNormalizer.GetMethodSignatureParameters(parameter);
                    ToDelete.Add(parameter);
                    ToAdd.AddRange(parameters);
                }
            }
            ToDelete.ForEach(pm => method.Parameters.Remove(pm));
            ToAdd.ForEach(pm => method.Parameters.Add(pm));
        }

        private void FixAttributes(CodeMemberMethod method)
        {
            var atts = method.CustomAttributes.OfType<CodeAttributeDeclaration>().ToList();
            foreach (var item in atts)
            {
                if (item.AttributeType.BaseType == "System.ServiceModel.XmlSerializerFormatAttribute")
                {
                    method.CustomAttributes.Remove(item);
                }
                if (item.AttributeType.BaseType == "System.ServiceModel.OperationContractAttribute")
                {
                    FixOperationContractAttribute(method.Name, item);
                }
            }

            var parameters = method.Parameters.OfType<CodeParameterDeclarationExpression>().ToList();
            if (parameters.Count > 0)
            {
                string type = null;
                type = parameters[0].Type.BaseType;
                type = type.Substring(0, type.Length - 7);
                method.CustomAttributes.Add(CodeAttribute(type + "oxm", oxmGenerator.CodeGenerator));
            }
        }

        private void FixOperationContractAttribute(string operationName, CodeAttributeDeclaration item)
        {
            var operation = importer[operationName];
            if (operation == null)
                return;
            item.Arguments[0].Value = new CodeSnippetExpression('"' + operation.Messages[0].Action + '"');
            item.Arguments[1].Value = new CodeSnippetExpression('"' + operation.Messages[1].Action + '"');
        }

        private List<CodeMemberField> AddGeneratedType(CodeTypeDeclaration codeType, CodeNamespaceCollection collection)
        {
            if (!TypesToGenerate.Contains(codeType))
            {
                //FixType(codeType, collection);
                TypesToGenerate.Add(codeType);
            }
            return codeType.Members.OfType<CodeMemberField>().ToList();
        }


    }
}
