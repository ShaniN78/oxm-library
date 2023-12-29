using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxmLibrary.CodeGeneration;
using System.Xml.Linq;

namespace OxmLibrary.GUI
{
    public class OxmXSDImporterPlugin : IOxmGeneratorPlugin
    {
        private OxmGenerator3 generator;
        private string fileName;

        #region IOxmGeneratorPlugin Members

        public IEnumerable<string> Namespaces
        {
            get { return Enumerable.Empty<string>(); }
        }

        public OxmGenerator3 OxmGenerator
        {
            get
            {
                return generator;
            }
            set
            {
                generator = value;
            }
        }

        public void ModifyConfiguration(OxmLibrary.CodeGeneration.GenerationConfiguration config)
        {
            
        }

        public OxmLibrary.CodeGeneration.PluginFileFormatSupport RequiresExternalFile
        {
            get { return OxmLibrary.CodeGeneration.PluginFileFormatSupport.FromLocanAndUrl; }
        }

        public bool RequiresExtraSaving
        {
            get { return false; }
        }

        public void AfterConfigurationAction()
        {
            generator.AddXsd(fileName);
        }


        public void WriteExtraFile(string path)
        {
            
        }

        public string FileFilter
        {
            get { return "XSD File (*.xsd)|*.xsd"; }
        }

        public string PluginName
        {
            get { return "XSD File importer"; }
        }

        public string ExtraFileFilter
        {
            get { throw new NotImplementedException(); }
        }

        public void ProcessFile(string path)
        {
            var file = XDocument.Load(path);
        }

        private string targetNameSpace;

        public void WritePlugin(System.IO.TextWriter writer)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<OxmGeneratorAddFileEventArgs> AddFile;

        public event EventHandler<OxmGeneratorAddSchemaEventArgs> AddSchema;

        public event EventHandler<OxmGeneratorAddClassEventArgs> AddClass;

        #endregion
    }
}
