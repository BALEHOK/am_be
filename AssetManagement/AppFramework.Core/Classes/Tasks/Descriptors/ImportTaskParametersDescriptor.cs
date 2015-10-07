using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using AppFramework.ConstantsEnumerators;
using System.IO;

namespace AppFramework.Core.Classes.Tasks
{
    [DataContract]
    public class ImportTaskParametersDescriptor : ParametersDescriptor
    {
        [DataMember(Order = 2)]
        public TaskImportFileType FileType { get; set; }

        public static ImportTaskParametersDescriptor Deserialize(string xml)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(ImportTaskParametersDescriptor));
            MemoryStream tempStream = new MemoryStream();

            byte[] buffer = Encoding.UTF8.GetBytes(xml);
            tempStream.Write(buffer, 0, buffer.Length);
            tempStream.Position = 0;

            return ser.ReadObject(tempStream) as ImportTaskParametersDescriptor;
        }
    }
}
