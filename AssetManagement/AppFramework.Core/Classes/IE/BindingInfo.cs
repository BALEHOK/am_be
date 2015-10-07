using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Xml.XPath;

namespace AppFramework.Core.Classes.IE
{
    public class BindingInfo
    {
        public List<ImportBinding> Bindings { get; set; }

        public BindingInfo() 
        {
            Bindings = new List<ImportBinding>();
        }

        /// <summary>
        /// Gets parameters dictionary from XML.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        /// <returns></returns>
        public static BindingInfo GetFromXml(string xmlString)
        {
            var parameters = new BindingInfo();
            var xml = XElement.Load(new XmlTextReader(new StringReader(xmlString)));
            xml.XPathSelectElements("/bindings/param").ToList()
                .ForEach(e =>
                {
                    var binding = new ImportBinding
                    {
                        DestinationAttributeId = long.Parse(e.Attribute("Key").Value),
                        DataSourceFieldName = e.Attribute("Value").Value,
                        DefaultValue = e.Attribute("DefaultValue").Value,
                    };
                    if (e.Attribute("RelatedAttributeId") != null &&
                        !string.IsNullOrEmpty(e.Attribute("RelatedAttributeId").Value))
                        binding.DestinationRelatedAttributeId = long.Parse(e.Attribute("RelatedAttributeId").Value);
                    parameters.Bindings.Add(binding);
                });
            return parameters;
        }

        public string ToXml()
        {
            var xml = new XElement("parameters",
                new XElement("bindings",
                    from binding in Bindings
                    select new XElement("param",
                        new XAttribute("Key", binding.DestinationAttributeId),
                        new XAttribute("Value", binding.DataSourceFieldName ?? ""),
                        new XAttribute("DefaultValue", binding.DefaultValue ?? ""),
                        new XAttribute("RelatedAttributeId",
                            binding.DestinationRelatedAttributeId.HasValue
                                ? binding.DestinationRelatedAttributeId.ToString()
                                : ""))));
            return xml.ToString();
        }
    }
}
