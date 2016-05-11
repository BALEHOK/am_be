using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;

namespace AppFramework.Core.Classes.SearchEngine
{
    public class SearchParameters : ActionParameters<string, object>
    {
        public string QueryString
        {
            get { return (string) this["querystring"]; }
            set { this["querystring"] = value; }
        }

        public string ConfigsIds
        {
            get { return (string) this["configsIds"]; }
            set { this["configsIds"] = value; }
        }

        public string TaxonomyItemsIds
        {
            get { return (string) this["taxonomyItemsIds"]; }
            set { this["taxonomyItemsIds"] = value; }
        }

        public List<AttributeElement> Elements
        {
            get { return this["elements"] as List<AttributeElement>; }
            set { this["elements"] = value; }
        }

        public TimePeriodForSearch Time
        {
            get { return (TimePeriodForSearch) int.Parse(this["time"].ToString()); }
            set { this["time"] = (int) value; }
        }

        public Entities.Enumerations.SearchOrder Order
        {
            get { return (Entities.Enumerations.SearchOrder) int.Parse(this["order"].ToString()); }
            set { this["order"] = (int) value; }
        }

        /// <summary>
        /// Serialize Dictionary to xml 
        /// </summary>
        /// <returns>Xml serialized parameters dictionary</returns>
        public new string ToXml()
        {
            var xml = new XElement("parameters",
                from pr in this
                where pr.Value != null
                select new XElement("param",
                    new XAttribute("name", pr.Key),
                    new XAttribute("type", pr.Value.GetType()),
                    getParameterSerializedValue(pr.Value)));

            return xml.ToString();
        }

        /// <summary>
        /// Gets parameters dictionary from XML.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        /// <returns></returns>
        public static new SearchParameters GetFromXml(string xmlString)
        {
            var parameters = new SearchParameters();
            var xml = XElement.Load(new XmlTextReader(new StringReader(xmlString)));
            xml.XPathSelectElements("//param")
                .ToList()
                .ForEach(e => parameters.Add(e.Attribute("name").Value, getParameterDeserializedValue(e)));
            return parameters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>string or XElement</returns>
        private object getParameterSerializedValue(object parameter)
        {
            object result;
            var parameterEnumerable = parameter as List<AttributeElement>;
            if (parameterEnumerable != null)
            {
                var serializableParam = parameterEnumerable.ToArray();
                using (var sw = new StringWriter())
                {
                    var doc = new XmlDocument();
                    var serializer = new XmlSerializer(typeof (AttributeElement[]));
                    serializer.Serialize(sw, serializableParam);
                    doc.LoadXml(sw.ToString());
                    sw.Close();
                    result = doc.SelectSingleNode("/ArrayOfAttributeElement").GetXElement();
                }
            }
            else
            {
                result = parameter.ToString();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>string or List of AttributeElement</returns>
        private static object getParameterDeserializedValue(XElement parameter)
        {
            if (parameter.Attribute("type").Value.Contains("AttributeElement"))
            {
                var extraTypes = new Type[1];
                extraTypes[0] = typeof (AttributeElement);
                var serializer = new XmlSerializer(typeof (AttributeElement[]), extraTypes);
                TextReader reader =
                    new StringReader(parameter.XPathSelectElement("ArrayOfAttributeElement").GetXmlNode().OuterXml);
                var result = serializer.Deserialize(reader);
                return (result as AttributeElement[]).ToList();
            }
            return parameter.Value;
        }
    }
}