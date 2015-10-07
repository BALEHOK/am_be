using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.IE.Providers;

namespace AppFramework.Core.Classes.IE
{
    /// <summary>
    /// Generates the specific XSD schema,
    /// which describes XML format to antities of given type.
    /// </summary>
    public class SchemaGenerator
    {

        public static string DefaultNamespace = "http://tempuri.org/AssetManagementAssets.xsd";

        private AssetType _at;

        /// <summary>
        /// Annotation for current schema document
        /// </summary>
        private string GeneralAnnotation
        {
            get
            {
                string annotation
                    = string.Format("Schema definition for assets of type {0}", _at.NameInvariant);
                return annotation;
            }
        }

        /// <summary>
        /// Annotation for document attributes
        /// </summary>
        private string AttributesAnnotation
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Empty);
                sb.AppendLine("List of attributes can contain: ID and elements with following names: ");
                foreach (AssetTypeAttribute attr in _at.Attributes.Where(a => a.Editable))
                {
                    string attrDef = attr.Name;
                    if (attr.IsRequired)
                    {
                        attrDef += " (required)";
                    }
                    sb.AppendLine(attrDef);
                }
                return sb.ToString();
            }
        }

        public SchemaGenerator(AssetType at)
        {
            _at = at;
        }

        /// <summary>
        /// Returns the XSD schema document for Importing assets  of given AssetType
        /// </summary>
        /// <param name="at"></param>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            XmlSchema schema = null;
            XmlReader reader = null;
            ValidationEventHandler handler = new ValidationEventHandler(ValidationCallbackOne);

            try
            {
                reader = XmlReader.Create(new StreamReader(ApplicationSettings.AssetsSchemaPath));
                schema = XmlSchema.Read(reader, handler);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
            }

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.ValidationEventHandler += handler;
            schemaSet.Add(schema);
            schemaSet.Compile();

            XmlSchema compiledSchema = null;

            foreach (XmlSchema schema1 in schemaSet.Schemas())
            {
                compiledSchema = schema1;
            }

            XmlSchemaElement foundElement = null;

            // traversing over all elements of schema till required will be found
            foreach (XmlSchemaElement element in compiledSchema.Elements.Values)
            {
                if (element.Name == "Assets")
                {
                    element.Annotation = GetAnnotation(GeneralAnnotation);
                }

                foundElement = element;
                while (foundElement != null)
                {
                    if (foundElement.Name == "Attributes")
                    {
                        foundElement.Annotation = GetAnnotation(AttributesAnnotation);
                    }
                    else if (foundElement.Name == "Attribute")
                    {
                        break;
                    }
                    foundElement = TraverseDescendant(foundElement);
                }
            }

            if (foundElement != null)
            {
                // min - required attributes except internal
                foundElement.MinOccurs = _at.Attributes.Where(a => a.IsRequired).Count();
                // max - all attributes
                foundElement.MaxOccurs = _at.Attributes.Count();
            }

            return compiledSchema;
        }

        public Stream GetXlsxSchemaAsStream(string appRootFolder, out string filePath)
        {
            string tempPath = appRootFolder + "\\Temp\\";

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            filePath = tempPath + "export" + DateTime.Now.ToLongTimeString().Replace(".", "_").Replace(":", "_") + ".xlsx";

            File.Copy(String.Format("{0}{1}", appRootFolder, @"Config.Files\Predefined\xlsxTemplate.xlsx"), filePath);

            DataTable dt = new DataTable("Sheet1$");
            var dr = dt.NewRow();
            int i = 1;
            foreach (var assetTypeAttribute in _at.Attributes.Where(at => at.Editable))
            {
                var columnName = assetTypeAttribute.Name;
                dt.Columns.Add(columnName);
                dr[columnName] = assetTypeAttribute.Name;
                i++;
            }

            dt.Rows.Add(dr);

            var xlsProvider = new ExcelProvider();
            xlsProvider.Write(dt, filePath);

            Stream result = File.OpenRead(filePath);

            return result;
        }

        private static XmlSchemaAnnotation GetAnnotation(string annotationText)
        {
            XmlSchemaAnnotation annotationElement = new XmlSchemaAnnotation();
            XmlSchemaDocumentation doc = new XmlSchemaDocumentation();
            doc.Markup = TextToNodeArray(annotationText);
            annotationElement.Items.Add(doc);
            return annotationElement;
        }

        private static XmlNode[] TextToNodeArray(string text)
        {
            XmlDocument doc = new XmlDocument();
            return new XmlNode[1] {
                  doc.CreateTextNode(text)};
        }

        private static XmlSchemaElement TraverseDescendant(XmlSchemaElement element)
        {
            XmlSchemaElement returnElement = null;
            XmlSchemaComplexType ct = element.SchemaType as XmlSchemaComplexType;
            if (ct != null)
            {
                XmlSchemaSequence seq = (XmlSchemaSequence)ct.ContentTypeParticle;
                foreach (XmlSchemaParticle p in seq.Items)
                {
                    XmlSchemaElement elem = p as XmlSchemaElement;
                    if (elem != null)
                    {
                        returnElement = elem;
                    }
                }
            }
            return returnElement;
        }

        private void ValidationCallbackOne(object sender, ValidationEventArgs args)
        {
            throw new System.Exception(args.Message);
        }

        /// <summary>
        /// Generates the XSD schema documets and returns it as string
        /// </summary>
        /// <param name="at"></param>
        /// <returns></returns>
        public string GetSchemaAsString()
        {
            XmlSchema schema = GetSchema();
            return ConvertToString(schema);
        }

        /// <summary>
        /// Writes schema to file
        /// </summary>
        /// <param name="schema">XmlSchema to write</param>
        /// <param name="filepath">Full filepath</param>
        public static void WriteSchema(XmlSchema schema, string filepath)
        {
            TextWriter twr = new StreamWriter(filepath, false, Encoding.Default);
            try
            {
                schema.Write(twr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                twr.Flush();
                twr.Close();
            }
        }

        /// <summary>
        /// Converts schema document to string
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        private static string ConvertToString(XmlSchema schema)
        {
            StringBuilder sb = new StringBuilder();
            TextWriter twr = null;
            try
            {
                twr = new StringWriter(sb);
                schema.Write(twr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                twr.Close();
            }

            return sb.ToString(); ;
        }
    }
}
