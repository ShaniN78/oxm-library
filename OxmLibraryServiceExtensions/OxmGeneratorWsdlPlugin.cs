using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml;
using System.Xml.Schema;

using System.CodeDom.Compiler;
using OxmLibrary;
using System.Web.Services.Discovery;

using WsdlNS = System.Web.Services.Description;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.Web.Services.Description;
using System.Collections.ObjectModel;
using System.CodeDom;
using System.ComponentModel;
using OxmLibrary.CodeGeneration;

namespace OxmLibrary.WSDLServiceExtensions
{
    [DisplayName("Import XSD and contract From Wsdl")]
    public class OxmGeneratorWsdlPlugin : IOxmGeneratorPlugin
    {
        public OxmGeneratorWsdlPlugin()
        {
        }

        #region IOxmGeneratorPlugin Members

        private WsdlImporting importer;
        private string originatingPath;
        private OxmGenerator3 oxmGenerator;
        private ServiceContractGenerator generator;
        private WsdlCodeWriting writing;

        public string PluginName
        {
            get { return "Import Wsdl Definitions"; }
        }

        public PluginFileFormatSupport RequiresExternalFile
        {
            get { return PluginFileFormatSupport.FromLocanAndUrl; }
        }

        public string FileFilter
        {
            get { return "Wsdl Files (*.wsdl)|*.wsdl"; }
        }

        public void WritePlugin(TextWriter writer)
        {
            writing.Write(writer);
        }

        private List<string> GeneratedEndpoints = new List<string>();

        private Collection<ChannelEndpointElement> GenerateConfig(ServiceContractGenerator contractGenerator, ServiceEndpointCollection endpoints, IEnumerable<System.ServiceModel.Channels.Binding> bindings, ContractDescription contract)
        {
            var generatedChannelElements = new Collection<ChannelEndpointElement>();
            ChannelEndpointElement element2;
            ServiceModelSectionGroup sectionGroup = ServiceModelSectionGroup.GetSectionGroup(contractGenerator.Configuration);
            CustomBindingCollectionElement customBinding = sectionGroup.Bindings.CustomBinding;
            foreach (ServiceEndpoint endpoint in endpoints)
            {
                if (!GeneratedEndpoints.Contains(endpoint.Name))
                {
                    GeneratedEndpoints.Add(endpoint.Name);
                    try
                    {
                        contractGenerator.GenerateServiceEndpoint(endpoint, out element2);
                        generatedChannelElements.Add(element2);
                    }
                    catch (ArgumentException)
                    {
                        continue;
                    }
                }
            }
            foreach (var binding in bindings)
            {
                ServiceEndpoint endpoint2 = new ServiceEndpoint(contract, binding, null);
                if (!GeneratedEndpoints.Contains(endpoint2.Name))
                {
                    try
                    {
                        contractGenerator.GenerateServiceEndpoint(endpoint2, out element2);
                        generatedChannelElements.Add(element2);
                    }
                    catch (ArgumentException)
                    {
                        continue;
                    }
                }
            }

            return generatedChannelElements;
        }

        public void ProcessFile(string path)
        {
            originatingPath = Path.GetDirectoryName(path);
            oxmGenerator.Config.AddAdditionalParameter("MessageContract", true, "Create Message Contract");
            oxmGenerator.Config.AddAdditionalParameter("SpecifiedField", false, "Add specified field for value types");
            importer = new WsdlImporting(path);
            var messageContract = oxmGenerator.Config.AdditionalParameters.Get<bool>("MessageContract");
            generator = importer.ServiceGenerator(null, messageContract);
            writing = new WsdlCodeWriting(importer, importer.Contracts, oxmGenerator);
        }

        public event EventHandler<OxmGeneratorAddFileEventArgs> AddFile;

        public event EventHandler<OxmGeneratorAddSchemaEventArgs> AddSchema;

        public event EventHandler<OxmGeneratorAddClassEventArgs> AddClass;

        public void OnAddClass(ClassDescriptor descriptor)
        {
            if (AddClass != null)
                AddClass(this, new OxmGeneratorAddClassEventArgs(descriptor));
        }

        public void OnAddFile(string fileName)
        {
            if (AddFile != null)
                AddFile(this, new OxmGeneratorAddFileEventArgs(fileName));
        }

        public void OnAddSchema(XmlSchema schema, string wsdlPath)
        {
            if (AddSchema != null)
                AddSchema(this, new OxmGeneratorAddSchemaEventArgs(schema, wsdlPath));
        }

        public OxmGenerator3 OxmGenerator
        {
            get
            {
                return oxmGenerator;
            }
            set
            {
                oxmGenerator = value;
            }
        }

        private static readonly string[] NAMESPACES = { "System.ServiceModel" };

        public IEnumerable<string> Namespaces
        {
            get { return NAMESPACES; }
        }

        public bool RequiresExtraSaving
        {
            get { return true; }
        }

        public void WriteExtraFile(string path)
        {
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = path }, ConfigurationUserLevel.None);
            CleanUpConfig(generator, config);

            generator = importer.ServiceGenerator(config, oxmGenerator.Config.AdditionalParameters.Get<bool>("MessageContract"));
            foreach (var contract in importer.Contracts)
            {
                var result = GenerateConfig(generator, importer.EndPoints, importer.Bindings, contract);
            }
            generator.Configuration.Save();
        }

        private void CleanUpConfig(ServiceContractGenerator generator, Configuration config)
        {
            var modelSection = ServiceModelSectionGroup.GetSectionGroup(config);

            foreach (var contract in importer.Contracts)
            {
                var name = contract.Name;
                var nameSpace = contract.Namespace;

                CleanUpConfigByContract(modelSection, name, generator, config);
            }
        }

        private void CleanUpConfigByContract(ServiceModelSectionGroup modelSection, string name, ServiceContractGenerator generator, Configuration config)
        {
            var endPointsTodelete = new List<ChannelEndpointElement>();
            var behaviorsToDelete = new List<string>();
            var bindingsToDelete = new Dictionary<string, string>();
            foreach (ChannelEndpointElement endPoint in modelSection.Client.Endpoints)
            {
                if (endPoint.Contract == name)
                {
                    endPointsTodelete.Add(endPoint);
                    if (!string.IsNullOrEmpty(endPoint.BehaviorConfiguration))
                        behaviorsToDelete.Add(endPoint.BehaviorConfiguration);
                    if (!string.IsNullOrEmpty(endPoint.BindingConfiguration) && !bindingsToDelete.ContainsKey(endPoint.BindingConfiguration))
                        bindingsToDelete.Add(endPoint.BindingConfiguration, endPoint.Binding);
                }
            }
            endPointsTodelete.ForEach(a => modelSection.Client.Endpoints.Remove(a));
            foreach (var binding in bindingsToDelete)
            {
                RemoveBinding(modelSection.Bindings, binding.Value, binding.Key);
            }
            behaviorsToDelete.ForEach(a =>
                {
                    var endPoint = modelSection.Behaviors.EndpointBehaviors.OfType<EndpointBehaviorElement>().FirstOrDefault(epBe => epBe.Name == a);
                    if (endPoint != null)
                        modelSection.Behaviors.EndpointBehaviors.Remove(endPoint);
                });
        }

        private void RemoveBinding(BindingsSection bindingsSection, string binding, string bindingName)
        {
            RemoveFrom(bindingsSection.BasicHttpBinding, bindingsSection, bindingName);
            RemoveFrom(bindingsSection.CustomBinding, bindingsSection, bindingName);
            RemoveFrom(bindingsSection.NetTcpBinding, bindingsSection, bindingName);
            RemoveFrom(bindingsSection.WSHttpBinding, bindingsSection, bindingName);
            RemoveFrom(bindingsSection.WS2007HttpBinding, bindingsSection, bindingName);
        }


        private void RemoveFrom(CustomBindingCollectionElement bindings, BindingsSection bindingsSection, string bindingName)
        {
            if (bindings.ContainsKey(bindingName))
            {
                var bind = bindings.Bindings[bindingName];
                bindings.Bindings.Remove(bind);
            }
        }

        private void RemoveFrom<TBinding, TElement>(StandardBindingCollectionElement<TBinding, TElement> bindings, BindingsSection bindingsSection, string bindingName)
            where TBinding : System.ServiceModel.Channels.Binding
            where TElement : StandardBindingElement, new()
        {
            if (bindings.ContainsKey(bindingName))
            {
                var bind = bindings.Bindings[bindingName];
                bindings.Bindings.Remove(bind);
            }
        }

        public string ExtraFileFilter
        {
            get { return "Config file(*.config)|*.config"; }
        }

        public void ModifyConfiguration(OxmLibrary.CodeGeneration.GenerationConfiguration config)
        {
            config.UseTargetNamespace = true;
            config.ProjectName = importer.Contracts[0].Name;
            var tempNamspace = writing.PreProcess(oxmGenerator, generator);
            if (string.IsNullOrEmpty(tempNamspace))
            {
                tempNamspace = string.IsNullOrEmpty(importer.Contracts[0].Namespace) ? config.CurrentNamespace : removeScheme(importer.Contracts[0].Namespace);
            }

            config.CurrentNamespace = tempNamspace.Replace("/", "");
        }

        private string removeScheme(string currentNamespace)
        {
            int index = currentNamespace.IndexOf("://");
            return index == -1 ? currentNamespace : currentNamespace.Substring(index + 3);
        }

        public void AfterConfigurationAction()
        {
            var messageContract = oxmGenerator.Config.AdditionalParameters.Get<bool>("MessageContract");

            foreach (XmlSchema schema in importer.Schemas)
            {
                OnAddSchema(schema, originatingPath);
            }

            var types = writing.PrepareTypes(oxmGenerator, generator);
            foreach (var type in types)
            {
                var descriptor = ClassDescriptorHelper.GenerateFromCodeType(type, oxmGenerator, true, messageContract);
                if (descriptor != null)
                    OnAddClass(descriptor);
            }
            oxmGenerator.TypographicalOrderOfClasses();
        }

        #endregion
    }
}
