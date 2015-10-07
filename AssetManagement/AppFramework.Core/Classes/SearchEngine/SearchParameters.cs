namespace AppFramework.Core.Classes.SearchEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using System.IO;
    using AppFramework.Core.Classes;
    using AppFramework.ConstantsEnumerators;
    using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
    using System.Xml.Serialization;
    using AppFramework.Core.Classes.SearchEngine.Enumerations;

    public class SearchParameters : ActionParameters<string, object>
    {
        public string QueryString
        {
            get { return (string)this["querystring"]; }
            set { this["querystring"] = value; }            
        }

        public string ConfigsIds
        {
            get { return (string)this["configsIds"]; }
            set { this["configsIds"] = value; }
        }

        public string TaxonomyItemsIds
        {
            get { return (string)this["taxonomyItemsIds"]; }
            set { this["taxonomyItemsIds"] = value; }
        }

        public TimePeriodForSearch Time
        {
            get { return (TimePeriodForSearch)int.Parse(this["time"].ToString()); }
            set { this["time"] = (int)value; }
        }

        public AppFramework.Entities.Enumerations.SearchOrder Order
        {
            get { return (AppFramework.Entities.Enumerations.SearchOrder)int.Parse(this["order"].ToString()); }
            set { this["order"] = (int)value; }
        }

        public SearchParameters()
            : base() { }

        /// <summary>
        /// Serialize Dictionary to xml 
        /// </summary>
        /// <returns>Xml serialized parameters dictionary</returns>
        public new string ToXml()
        {
            XElement xml = new XElement("parameters",
                from pr in this
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
            SearchParameters parameters = new SearchParameters();
            XElement xml = XElement.Load(new XmlTextReader(new StringReader(xmlString)));
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
            if (parameter is IEnumerable<AttributeElement>)
            {
                AttributeElement[] serializableParam = (parameter as List<AttributeElement>).ToArray();
                using (StringWriter sw = new StringWriter())
                {
                    XmlDocument doc = new XmlDocument();
                    XmlSerializer serializer = new XmlSerializer(typeof(AttributeElement[]));
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
                Type[] extraTypes = new Type[1];
                extraTypes[0] = typeof(AttributeElement);
                XmlSerializer serializer = new XmlSerializer(typeof(AttributeElement[]), extraTypes);
                TextReader reader = new StringReader(parameter.XPathSelectElement("ArrayOfAttributeElement").GetXmlNode().OuterXml);
                object result = serializer.Deserialize(reader);
                return (result as AttributeElement[]).ToList();
            }
            else
            {
                return parameter.Value;
            }
        }
    }
}
