using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace AppFramework.Core.Classes.Tasks
{
    [DataContract]
    public abstract class ParametersDescriptor : IExtensibleDataObject
    {
        [DataMember(Name = "Parameters")]
        public List<KeyValuePair<string, string>> Data
        {
            get;
            set;
        }

        public ParametersDescriptor()
        {
            this.Data = new List<KeyValuePair<string, string>>();
        }

        public string Serialize()
        {
            MemoryStream memoryStream = new MemoryStream();
            DataContractSerializer ser = new DataContractSerializer(this.GetType());
            ser.WriteObject(memoryStream, this);

            memoryStream.Position = 0;
            byte[] buffer = new byte[memoryStream.Length];
            memoryStream.Read(buffer, 0, buffer.Length);

            return Encoding.UTF8.GetString(buffer); ;
        }

        #region IExtensibleDataObject implementation
        private ExtensionDataObject _extenstionData;
        public ExtensionDataObject ExtensionData
        {
            get
            {
                return _extenstionData;
            }
            set
            {
                _extenstionData = value;
            }
        }
        #endregion
    }
}
