using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.Runtime.Serialization;
using System.Xml;
using System.Reflection;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;

namespace OxmLibrary.WSDLRuntime
{
    public class OXMContractSerializerAttribute : Attribute, IOperationBehavior
    {
        #region IOperationBehavior Membe

        public Type WrapperType { get; set; }

        public OXMContractSerializerAttribute()
        {
        }

        public OXMContractSerializerAttribute(Type WrapperType)
        {
            this.WrapperType = WrapperType;
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
            var bind = bindingParameters;
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {                 
            ReplaceDataContractSerializerOperationBehavior(operationDescription, WrapperType);
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            ReplaceDataContractSerializerOperationBehavior(operationDescription, WrapperType);
        }

        public void Validate(OperationDescription operationDescription)
        {
            
        }

        private void ReplaceDataContractSerializerOperationBehavior(OperationDescription description, Type wrapperType)
        {
            DataContractSerializerOperationBehavior dcs = description.Behaviors.Find<DataContractSerializerOperationBehavior>();

            if (dcs != null)
                description.Behaviors.Remove(dcs);

            var serializer = new OxmContractSerializerOperationBehavior(description, wrapperType, description.SyncMethod);
            description.Behaviors.Add(serializer);
        } 
        #endregion
    }

    public class OxmContractSerializerOperationBehavior : DataContractSerializerOperationBehavior
    {
        protected Type wrapperType;
        protected MethodInfo info;

        public Type WrapperType { get { return wrapperType; } }

        public MethodInfo Info { get { return info; } }
        
        public OxmContractSerializerOperationBehavior(OperationDescription operationDescription, Type WrapperType, MethodInfo info) : base(operationDescription) 
        {
            this.info = info;
            this.wrapperType = WrapperType;
        }

        public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
        {
            return new OXMContractSerializer(type, wrapperType, info);
        }
        
        public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
        {
            return new OXMContractSerializer(type, wrapperType, info); 
        }
    }
}