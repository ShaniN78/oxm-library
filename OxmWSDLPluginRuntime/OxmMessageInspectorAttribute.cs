using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace OxmLibrary.WSDLRuntime
{
    public class OxmMessageInspectorAttribute : Attribute, IContractBehavior
    {
        #region IContractBehavior Members

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new OXMMessageInspector());
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            throw new NotImplementedException();
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
            return;
        }

        #endregion
    }
}
