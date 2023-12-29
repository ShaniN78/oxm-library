using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.IO;

namespace OxmLibrary.WSDLRuntime
{
    public class OXMMessageInspector : IClientMessageInspector
    {
        #region IClientMessageInspector Members

        public static TextWriter Writer { get; set; }

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            Writer.WriteLine("IClientMessageInspector.AfterReceiveReply called.");
            Writer.WriteLine("Message: {0}", reply.ToString());
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
        {
            Writer.WriteLine("IClientMessageInspector.BeforeSendRequest called.");
            Writer.WriteLine("Message: {0}", request.ToString());
            return null;
        }

        #endregion
    }
}
