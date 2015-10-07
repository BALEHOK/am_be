using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AppFramework.Core.Helpers
{
    public static class CustomSerializers
    {
        public static string SerializeObject(this List<string> list)
        {
            var serializer = new XmlSerializer(typeof(List<string>));

            using (var outStream = new StringWriter())
            {
                serializer.Serialize(outStream, list);
                return outStream.ToString();
            }
        }

        public static void Deserialize(this List<string> list, string source)
        {
            var serializer = new XmlSerializer(typeof(List<string>));

            using (var inStream = new StringReader(source))
            {
                var other = (List<string>)(serializer.Deserialize(inStream));
                list.Clear();
                list.AddRange(other);
            }
        }
    }
}
