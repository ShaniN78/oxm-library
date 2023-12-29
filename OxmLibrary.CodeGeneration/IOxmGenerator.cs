using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;

namespace OxmLibrary.CodeGeneration
{
    public interface IOxmGenerator
    {
        string ClassToString(ClassDescriptor descriptor);

        string PropertyToString(PropertyDescriptor property, bool fullProperty, string propertyType, string validationRegex);

        bool TargetNamespaceAware { get; }

        CodeDomProvider Provider { get; }
    }
}
