using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Schema;

namespace OxmLibrary.CodeGeneration
{
    public interface IOxmGeneratorPlugin
    {
        IEnumerable<string> Namespaces { get; }

        OxmGenerator3 OxmGenerator { get; set; }

        void ModifyConfiguration(GenerationConfiguration config);

        OxmLibrary.CodeGeneration.PluginFileFormatSupport RequiresExternalFile { get; }

        bool RequiresExtraSaving { get; }

        void AfterConfigurationAction();

        void WriteExtraFile(string path);

        string FileFilter { get; }

        string PluginName { get; }

        string ExtraFileFilter { get; }

        void ProcessFile(string path);

        void WritePlugin(TextWriter writer);

        event EventHandler<OxmGeneratorAddFileEventArgs> AddFile;

        event EventHandler<OxmGeneratorAddSchemaEventArgs> AddSchema;

        event EventHandler<OxmGeneratorAddClassEventArgs> AddClass;
    }

    public class OxmGeneratorAddFileEventArgs : EventArgs
    {
        public string FileName { get; set; }

        public OxmGeneratorAddFileEventArgs(string fileName)
        {
            this.FileName = fileName;
        }
    }

    public class OxmGeneratorAddClassEventArgs : EventArgs
    {
        public ClassDescriptor Descriptor { get; set; }

        public OxmGeneratorAddClassEventArgs(ClassDescriptor descriptor)
        {
            this.Descriptor = descriptor;
        }
    }

    public class OxmGeneratorAddSchemaEventArgs : EventArgs
    {
        public XmlSchema Schema { get; set; }

        public string WsdlPath { get; set; }

        public OxmGeneratorAddSchemaEventArgs(XmlSchema schema, string wsdlPath)
        {
            this.Schema = schema;
            this.WsdlPath = wsdlPath;
        }
    }

}
