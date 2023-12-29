using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing.Design;

namespace OxmLibrary.CodeGeneration
{
    [Serializable]
    [Editor("OxmLibrary.GUI.AdditionalPropertiesUIEditor, OxmStylizer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
        "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    //[Editor(typeof(AdditionalPropertiesUIEditor), typeof(UITypeEditor))]
    //[TypeConverter(typeof(GenerationAdditionalParametersConverter))]
    public class GenerationAdditionalParametersCollection : IXmlSerializable
    {
        [Browsable(false)]
        [NotifyParentProperty(true)]
        public SortedList<string, GenerationAdditionalParameters> Parameters { get; private set; }

        public Dictionary<string , string> Properties 
        {
            get
            {
                return Parameters.ToDictionary(a => a.Key, b => b.Value.Value);
            }
            set
            {
                foreach (var kv in Properties)
                {
                    Parameters[kv.Key].Value = kv.Value;
                }
            }
        }

        public GenerationAdditionalParametersCollection()
        {
            Parameters = new SortedList<string, GenerationAdditionalParameters>();
        }

        public override string ToString()
        {
            return "Additional - Click to expand";
        }

        public bool Add(GenerationAdditionalParameters item)
        {
            if (!Parameters.ContainsKey(item.Key))
            {
                Parameters.Add(item.Key, item);
                return true;
            }
            return false;
        }

        public bool Add(string key, object value, string displayName)
        {
            if (!Parameters.ContainsKey(key))
            {
                var newItem = new GenerationAdditionalParameters(key, value, displayName);
                Parameters.Add(key, newItem);
                return true;
            }
            return false;
        }

        public T1 Get<T1>(string key)
        {
            return Parameters[key].GetValue<T1>();
            //return (T1)TypeHandling.Parse(typeof(T1), Parameters[key].Value);
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<GenerationAdditionalParameters>));
            var MyList = (List<GenerationAdditionalParameters>)serializer.Deserialize(reader);
            foreach (var item in MyList)
            {
                Parameters.Add(item.Key, item);
            }            
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            var myList = Parameters.Values.ToList();
            XmlSerializer serializer = new XmlSerializer(typeof(List<GenerationAdditionalParameters>));
            serializer.Serialize(writer, myList);
        }

        #endregion
    }

    [Serializable]
    public class GenerationAdditionalParameters
    {
        [Browsable(false)]
        public string Key { get; set; }

        public string Value { get;  set; }

        [Browsable(false)]
        public string DisplayName { get; set; }

        [Browsable(false)]
        public Type PropertyType { get; private set; }

        public GenerationAdditionalParameters()
        {
        }

        public GenerationAdditionalParameters(string key, object value, string displayName)
        {
            this.Key = key;
            this.Value = value.ToString();
            this.DisplayName = displayName;
            PropertyType = value.GetType();
        }

        public T1 GetValue<T1>()
        {
            return (T1)TypeHandling.Parse(typeof(T1), this.Value);
        }
    }   
}
