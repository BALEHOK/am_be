using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using AppFramework.Core.Classes.IE.Providers;
using AppFramework.ConstantsEnumerators;

namespace AppFramework.Core.Classes.IE.IntegrityValidation
{
    /// <summary>
    /// Performs validation of internal XML format
    /// </summary>
    internal class XMLValidator
    {
        private string _xml;
        private AssetType _at;

        public XMLValidator(string xml, AssetType at)
        {
            _xml = xml;
            _at = at;
        }

        /// <summary>
        /// XML validation
        /// </summary>
        /// <returns></returns>
        public StatusInfo Validate()
        {
            // get schema for validation
            SchemaGenerator sg = new SchemaGenerator(_at);
            XmlSchema schema = sg.GetSchema();

            // set provider parameters
            ProviderParameters parameters = new ProviderParameters();
            parameters.Add(ProviderParameter.XMLString, _xml);
            parameters.Add(ProviderParameter.Schema, schema);
            XMLProvider xmlProvider = new XMLProvider(parameters);

            // perform validation
            return xmlProvider.Validate();
        }
    }
}
