namespace AppFramework.Core.Classes.Batch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml;
    using System.IO;
    using System.Xml.XPath;
    using AppFramework.Core.Classes;
    using AppFramework.ConstantsEnumerators;

    public class BatchActionParameters : ActionParameters<string, string>
    {
        public BatchActionParameters() 
            : base() { }

        /// <summary>
        /// Gets parameters dictionary from XML.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        /// <returns></returns>
        public static BatchActionParameters GetFromXml(string xmlString)
        {
            BatchActionParameters parameters = new BatchActionParameters();
            XElement xml = XElement.Load(new XmlTextReader(new StringReader(xmlString)));
            xml.XPathSelectElements("//param").ToList()
                .ForEach(e => parameters.Add(e.Attribute("Key").Value, e.Attribute("Value").Value));
            return parameters;
        }
    }
}
