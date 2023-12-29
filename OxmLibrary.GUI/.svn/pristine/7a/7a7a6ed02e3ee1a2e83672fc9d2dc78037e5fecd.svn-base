using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxmLibrary.CodeGeneration;

namespace OxmLibrary
{
    public static class CodeTemplateHandling
    {
        public static CodeTemplateBase GetTemplate(string language)
        {
            CodeTemplateFactory factory = CodeTemplateFactory.Generate();
            return factory.Create(language);
        }

        public static List<Type> GetAllAvailableTemplates()
        {
            return CodeTemplateFactory.GetTypes();
        }        
    }
}
