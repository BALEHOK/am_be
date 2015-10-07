using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace AppFramework.Core.Classes.Tasks
{
    [DataContract]
    public class BatchTaskParametersDescriptor : ParametersDescriptor
    {
        public static BatchTaskParametersDescriptor Deserialize(string xml)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(BatchTaskParametersDescriptor));
            MemoryStream tempStream = new MemoryStream();

            byte[] buffer = Encoding.UTF8.GetBytes(xml);
            tempStream.Write(buffer, 0, buffer.Length);
            tempStream.Position = 0;

            return ser.ReadObject(tempStream) as BatchTaskParametersDescriptor;
        }
    }
}
