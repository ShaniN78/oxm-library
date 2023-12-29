using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OxmLibrary.CodeGeneration
{
    public class CodeTemplateFactory 
    {
        private GenerationConfiguration config;

        private CodeTemplateFactory()
        {

        }

        public static CodeTemplateFactory Generate()
        {
            return new CodeTemplateFactory();
        }

        public void SetConfiguration(GenerationConfiguration config)
        {
            this.config = config;
        }

        private static readonly List<Type> AllTYpes;        

        static CodeTemplateFactory()
        {
            AllTYpes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.GetName().Name.StartsWith("System") && !a.GetName().Name.StartsWith("Microsoft") && a.GetName().Name != "mscorlib")
                .Select(assembly => assembly.GetTypes().Where(a => IsCodeTemplate(a)))
                .Aggregate((a, b) => a.Concat(b))
                .ToList();
        }
        private static bool IsCodeTemplate(Type a)
        {
            return (a.BaseType == typeof(CodeTemplateBase));
        }

        public static List<Type> GetTypes()
        {
            return new List<Type>(AllTYpes);
        }

        public CodeTemplateBase Create(string typename)
        {
            config = config ?? GenerationConfiguration.DefaultConfiguration;

            var type = AllTYpes.FirstOrDefault(a => a.FullName == typename);
            if (type == null)
                return null;
            var match = (CodeTemplateBase)Activator.CreateInstance(type);

            match.GenerateProvider();
            return match;
        }
    }
}
