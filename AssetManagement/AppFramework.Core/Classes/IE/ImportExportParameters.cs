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
using AppFramework.Core.Classes.Batch;

namespace AppFramework.Core.Classes.IE
{
    public class ImportExportParameters : ActionParameters<ImportExportParameter, string>
    {
         public ImportExportParameters() 
            : base() { }

        /// <summary>
        /// Gets parameters dictionary from XML.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        /// <returns></returns>
         public static new ImportExportParameters GetFromXml(string xmlString)
        {
            var parameters = new ImportExportParameters();
            XElement xml = XElement.Load(new XmlTextReader(new StringReader(xmlString)));
            xml.XPathSelectElements("//param").ToList()
                .ForEach(e => parameters.Add(ImportExportParameters.StringToParameter(e.Attribute("Key").Value), e.Attribute("Value").Value));
            return parameters;
        } 
    }
}
