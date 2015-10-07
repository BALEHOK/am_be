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

namespace AppFramework.Core.Classes.IE.Providers
{
    public class ProviderParameters : ActionParameters<ProviderParameter, object>
    {
        public ProviderParameters() 
            : base() { }

        /// <summary>
        /// Gets parameters dictionary from XML.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        /// <returns></returns>
        public static ProviderParameters GetFromXml(string xmlString)
        {
            ProviderParameters parameters = new ProviderParameters();
            XElement xml = XElement.Load(new XmlTextReader(new StringReader(xmlString)));
            xml.XPathSelectElements("//param").ToList()
                .ForEach(e => parameters.Add(StringToParameter(e.Attribute("Key").Value), e.Attribute("Value").Value));

            return parameters;
        }  
    }
}
