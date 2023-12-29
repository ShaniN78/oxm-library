using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxmLibrary;
using System.CodeDom;
using OxmLibrary.CodeGeneration;

namespace OxmLibrary.WSDLServiceExtensions
{
    public static class ClassDescriptorHelper
    {
        public static ClassDescriptor GenerateFromCodeType(CodeTypeDeclaration codeType, IOxmGenerator generator, bool GenerateAllFieldConstructor, bool MessageContract)
        {
            ClassDescriptor descriptor = new ClassDescriptor(generator);
            descriptor.OxmName = codeType.Name + "oxm";
            var commentsArray = new CodeCommentStatement[codeType.Comments.Count];
            codeType.Comments.CopyTo(commentsArray, 0);
            var comments = commentsArray.Where(a => a.Comment.DocComment).Select(a => a.Comment.Text).ToArray();
            descriptor.Comments = string.Join(";", comments);
            descriptor.Depth = 1;

            foreach (var member in codeType.CustomAttributes.OfType<CodeAttributeDeclaration>())
            {
                switch (member.Name)
                {
                    case "System.ServiceModel.MessageContractAttribute":
                        if (!MessageContract)
                            return null;
                        descriptor.CustomAttributes.Add(member);
                        //var attribute = new CodeAttributeDeclaration("OxmLibraryServiceExtensions.OxmContractMessageBodyAttribute");
                        //var properties = codeType.Members.OfType<CodeMemberField>().ToList();
                        //var body = properties.FirstOrDefault(a => a.CustomAttributes.OfType<CodeAttributeDeclaration>().Any(att => att.AttributeType.BaseType == "System.ServiceModel.MessageBodyMemberAttribute"));
                        //if (body != null)
                        //{
                        //    CodeAttributeArgument arg = new CodeAttributeArgument("BodyName", new CodeSnippetExpression('"' + body.Name + '"'));
                        //    attribute.Arguments.Add(arg);
                        //}
                        //descriptor.CustomAttributes.Add(attribute);
                        descriptor.InheritsFrom = "Nothing";
                        break;
                }
            }

            foreach (var member in codeType.Members.OfType<CodeMemberProperty>())
            {
                if (member.HasGet && member.HasSet)
                {
                    var property = PropertyDescriptor.FromCodeMemberProperty(member, generator);
                    descriptor.Add(property.PName, property);
                }
            }

            foreach (var member in codeType.Members.OfType<CodeMemberField>())
            {
                var property = PropertyDescriptor.FromCodeFieldProperty(member, generator);
                foreach (CodeAttributeDeclaration attribute in member.CustomAttributes)
                {
                    switch (attribute.Name)
                    {
                        case "System.ServiceModel.MessageBodyMemberAttribute":
                        case "System.ServiceModel.MessageHeaderAttribute":
                            property.CustomAttributes.Add(attribute);
                            break;
                    }
                }
                descriptor.Add(property.PName, property);
            }

            foreach (var member in codeType.Members.OfType<CodeConstructor>())
            {
                if (member.Parameters.Count != 0)
                    descriptor.AddConstructor(member);
            }


            return descriptor;
        }
    }
}
