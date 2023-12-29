using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.Xml.Schema;
using System.Xml;
using WsdlNS = System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Collections;
using System.Collections.ObjectModel;
using System.ServiceModel.Channels;
using System.Configuration;
using System.Xml.Linq;

namespace OxmLibrary.WSDLServiceExtensions
{
    public class WsdlImporting
    {
        WsdlImporter importer;
        private List<OperationDescription> backupOperations;

        public WsdlImporting(string path)
        {
            SchemasInsideWsdl = new List<XmlSchema>();
            Uri result;
            if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase) && Uri.TryCreate(path, UriKind.Absolute, out result))
            {
                MetadataExchangeClient client = new MetadataExchangeClient(result, MetadataExchangeClientMode.HttpGet);
                client.ResolveMetadataReferences = true;
                var meta = client.GetMetadata();
                //var wsdl = meta.MetadataSections.OfType<ServiceDescription>().ToList();

                var xmlFile = XDocument.Load(path);
                GetBackupOperationHeaders(xmlFile);
                importer = new WsdlImporter(meta);
            }
            else
            {
                var metaset = new MetadataSet(importMetaData(path, SchemasInsideWsdl));
                importer = new WsdlImporter(metaset);
            }
            contracts = importer.ImportAllContracts();
        }

        public List<XmlSchema> SchemasInsideWsdl
        {
            get;
            private set;
        }

        public ICollection Schemas
        {
            get
            {
                return importer.XmlSchemas.Schemas();
            }
        }

        public ServiceEndpointCollection EndPoints
        {
            get
            {
                return importer.ImportAllEndpoints();
            }
        }

        public IEnumerable<Binding> Bindings
        {
            get
            {
                return importer.ImportAllBindings();
            }
        }

        private Collection<ContractDescription> contracts;

        public Collection<ContractDescription> Contracts
        {
            get
            {
                return contracts;
            }
        }

        public ServiceContractGenerator ServiceGenerator(Configuration config, bool MessageContracts)
        {
            ServiceContractGenerator generator = new ServiceContractGenerator(config);
            generator.Options = ServiceContractGenerationOptions.ClientClass;
            if (!MessageContracts)
                generator.Options = generator.Options | ServiceContractGenerationOptions.TypedMessages;
            return generator;
        }

        private List<MetadataSection> importMetaData(string url, List<XmlSchema> alternativeSchemas)
        {
            System.Web.Services.Description.ServiceDescription wsdl;
            XmlSchema schema;
            XmlElement xmlDoc;

            DiscoveryClientProtocol disco = new DiscoveryClientProtocol();
            disco.AllowAutoRedirect = true;
            disco.UseDefaultCredentials = true;
            disco.DiscoverAny(url);
            disco.ResolveAll();

            var results = new List<MetadataSection>();

            foreach (object document in disco.Documents.Values)
            {
                wsdl = document as WsdlNS.ServiceDescription;
                schema = document as XmlSchema;
                xmlDoc = document as XmlElement;

                if (wsdl != null)
                {
                    results.Add(MetadataSection.CreateFromServiceDescription(wsdl));
                    var xmlFile = XDocument.Load(wsdl.RetrievalUrl);
                    GetBackupOperationHeaders(xmlFile);
                    foreach (XmlSchema xschema in wsdl.Types.Schemas)
                    {
                        alternativeSchemas.Add(xschema);
                    }
                }
                else if (schema != null)
                {
                    results.Add(MetadataSection.CreateFromSchema(schema));
                }
                else if (xmlDoc != null && xmlDoc.LocalName == "Policy")
                {
                    results.Add(MetadataSection.CreateFromPolicy(xmlDoc, null));
                }
                else
                {
                    MetadataSection mexDoc = new MetadataSection();
                    mexDoc.Metadata = document;
                    results.Add(mexDoc);
                }
            }
            return results;
        }

        private void GetBackupOperationHeaders(XDocument xmlFile)
        {
            ContractDescription contract = new ContractDescription("myname");

            var operationsElements = (from i in xmlFile.Descendants().FirstOrDefault(a => a.Name.LocalName == "binding").Descendants()
                                      let wsdl = i.GetNamespaceOfPrefix("wsdl")
                                      where i.Name.Namespace == wsdl && i.Name.LocalName == "operation"
                                      select i).ToList();
            var operations = (from i in operationsElements
                              select OperationFromElement(xmlFile, i, contract)).ToList();
            backupOperations = operations;
        }

        private OperationDescription OperationFromElement(XDocument doc, XElement i, ContractDescription contract)
        {
            var name = i.Attribute("name").Value;
            var soapOperation = i.Descendants().First(a => a.Name.LocalName == "operation");
            var input = soapOperation.Attribute("soapAction").Value;
            var output = getOutput(doc, name);
            OperationDescription result = new OperationDescription(name, contract);
            result.Messages.Add(new MessageDescription(input, MessageDirection.Input));
            result.Messages.Add(new MessageDescription(output ?? "*", MessageDirection.Output));
            return result;
        }

        private string getOutput(XDocument doc, string name)
        {
            var port = doc.Descendants().FirstOrDefault(a => a.Name.LocalName == "portType");
            if (port == null)
                return null;
            var operation = port.Descendants().FirstOrDefault(a => a.Name.LocalName == "operation" && a.Attribute("name").Value == name);
            if (operation == null)
                return null;
            var output = operation.Descendants().FirstOrDefault(a => a.Name.LocalName == "output");
            if (output == null)
                return null;
            var action = output.Attributes().FirstOrDefault(a => a.Name.LocalName == "Action");
            return action == null ? null : action.Value;
        }

        public OperationDescription this[string operationName]
        {
            get
            {
                return backupOperations.FirstOrDefault(a => a.Name == operationName);
            }
        }
    }
}
