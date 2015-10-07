using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using AppFramework.ConstantsEnumerators;

namespace AppFramework.Core.Classes.IE.Providers
{
    internal class XMLProvider : IDisposable
    {
        private ProviderParameters _parameters;
        private XmlReader _reader;
        private XmlReaderSettings _readerSettings;
        private XNamespace _namespace = "http://tempuri.org/AssetManagementAssets.xsd";

        private XmlReader Reader
        {
            get
            {
                if (_reader == null) InitReader();
                return _reader;
            }
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="oType">Array of classes which implements serializable interface</param>
        private XMLProvider()
        {
            _reader = null;
            Status = new StatusInfo();
        }

        public XMLProvider(ProviderParameters parameters)
            : this()
        {
            _parameters = parameters;
        }

        private XmlSerializer GetSerializer(Type objectsType)
        {
            // set the serializer engine
            XmlSerializer xmlSerializer = new XmlSerializer(objectsType);

            // errors handling
            xmlSerializer.UnknownNode += new XmlNodeEventHandler(_xmlSerializer_UnknownNode);
            xmlSerializer.UnknownAttribute += new XmlAttributeEventHandler(_xmlSerializer_UnknownAttribute);
            xmlSerializer.UnknownElement += new XmlElementEventHandler(_xmlSerializer_UnknownElement);
            xmlSerializer.UnreferencedObject += new UnreferencedObjectEventHandler(_xmlSerializer_UnreferencedObject);

            return xmlSerializer;
        }

        private void InitReader()
        {
            // Set the reader's validation settings.
            _readerSettings = new XmlReaderSettings();
            if (Parameters.ContainsKey(ProviderParameter.Schema))
            {
                _readerSettings.Schemas.Add(Parameters[ProviderParameter.Schema] as XmlSchema); // TODO: get dynamic schema
                _readerSettings.ValidationType = ValidationType.Schema;
                _readerSettings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;
                _readerSettings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
            }

            // string source has the higher priority than file source
            if (Parameters.ContainsKey(ProviderParameter.XMLString))
            {
                _reader = XmlReader.Create(new StringReader(Parameters[ProviderParameter.XMLString].ToString()), _readerSettings);
            }
            else if (Parameters.ContainsKey(ProviderParameter.ReadPath))
            {
                _reader = XmlReader.Create(new StreamReader(Parameters[ProviderParameter.ReadPath].ToString()), _readerSettings);
            }
            else
            {
                throw new ArgumentException("DataSource was not provided");
            }
        }

        # region XML errors handling
        private void _xmlSerializer_UnreferencedObject(object sender, UnreferencedObjectEventArgs e)
        {
            Status.Warnings.Add("Unreferenced Object: " + e.UnreferencedId);
        }

        private void _xmlSerializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            Status.Warnings.Add(string.Format("Unknown element '{0}' at line {1}.\nExpected: {2}.",
                e.Element, e.LineNumber, e.ExpectedElements));
        }

        private void _xmlSerializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            Status.Warnings.Add(string.Format("Unknown attribute '{0}' at line {1}.\nExpected: {2}.",
                e.Attr, e.LineNumber, e.ExpectedAttributes));
        }

        private void _xmlSerializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Status.Warnings.Add(string.Format("Unknown node '{0}' at line {1}.",
                e.Name, e.LineNumber));
        }
        #endregion

        #region IEntityDataProvider Members

        /// <summary>
        /// Pre-validation and fields retrieving
        /// </summary>
        /// <returns></returns>
        public List<string> GetFields()
        {
            List<string> fields = new List<string>();

            try
            {
                XDocument document = XDocument.Load(Reader);
                fields = (from node in document.Descendants()
                          where node.Name == _namespace + "Attribute"
                          select node.Element(_namespace + "Name").Value).Distinct().ToList();
            }
            catch (Exception ex)
            {
                Status.IsSuccess = false;
                Status.Errors.Add(ex.Message);
            }
            finally
            {
                Reader.Close();
            }

            return fields;
        }

        /// <summary>
        /// Validates XML with schema
        /// </summary>
        /// <returns></returns>
        public StatusInfo Validate()
        {
            try
            {
                while (Reader.Read()) ;
            }
            catch (Exception ex)
            {
                Status.IsSuccess = false;
                Status.Errors.Add(ex.Message);
            }
            finally
            {
                Reader.Close();
            }
            return Status;
        }

        /// <summary>
        /// Reads the datasource and returns the results
        /// </summary>
        /// <returns>XDocument</returns>
        public ActionResult<XDocument> Read()
        {
            XDocument document;
            try
            {
                document = XDocument.Load(Reader);
            }
            catch
            {
                throw;
            }
            finally
            {
                Reader.Close();
            }
            return new ActionResult<XDocument>(Status, document);
        }

        public IEnumerable<object> GetEntities()
        {
            throw new NotImplementedException();
        }

        private void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            // strict level: schema is invalid
            Status.IsSuccess = false;

            string message = string.Format("{0}\n Line: {1}, position: {2}.",
                args.Message, args.Exception.LineNumber, args.Exception.LinePosition);

            if (args.Severity == XmlSeverityType.Warning)
            {
                Status.Warnings.Add(message);
            }
            else if (args.Severity == XmlSeverityType.Error)
            {
                Status.Errors.Add(message);
            }
        }

        public IEnumerable<object> Deserialize(Type oType)
        {
            XmlSerializer xmlSerializer = GetSerializer(oType);

            object dsObj = null;

            try
            {
                dsObj = xmlSerializer.Deserialize(Reader);
            }
            catch (Exception ex)
            {
                Status.IsSuccess = false;
                Status.Errors.Add(ex.Message);
            }
            finally
            {
                Reader.Close();
            }

            if (dsObj != null)
            {
                if (dsObj as System.Array != null)
                {
                    foreach (var item in dsObj as System.Array)
                    {
                        yield return item;
                    }
                }
                else
                {
                    yield return dsObj;
                }
            }

            dsObj = null;
            yield break;
        }

        public void Write(string xmldata)
        {

            if (!Parameters.ContainsKey(ProviderParameter.WritePath))
            {
                throw new ArgumentException("Write path was not provided.");
            }

            TextWriter w = new StreamWriter(Parameters[ProviderParameter.WritePath].ToString(), false);
            try
            {
                w.Write(xmldata);
            }
            catch (Exception ex)
            {
                Status.IsSuccess = false;
                Status.Errors.Add(ex.Message);
            }
            finally
            {
                w.Flush();
                w.Close();
            }
        }

        public StatusInfo Write(XDocument xmlDocument)
        {
            if (!Parameters.ContainsKey(ProviderParameter.WritePath))
            {
                throw new ArgumentException("Write path was not provided.");
            }

            //XmlWriter w = XmlWriter.Create(Parameters[ProviderParameter.WritePath].ToString());
            try
            {
                xmlDocument.Save(Parameters[ProviderParameter.WritePath].ToString(), SaveOptions.None);
                //xmlDocument.WriteTo(w);
            }
            catch (Exception ex)
            {
                Status.IsSuccess = false;
                Status.Errors.Add(ex.Message);
            }
            finally
            {
                //w.Flush();
                //w.Close();
            }
            return Status;
        }

        /// <summary>
        /// Serializes the given entities to an XML document
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public string Serialize(IEnumerable<object> entities, Type objectsType)
        {
            XmlSerializer xmlSerializer = GetSerializer(objectsType);

            StringBuilder sb = new StringBuilder();
            XmlWriter xwr = XmlWriter.Create(sb);
            try
            {
                xmlSerializer.Serialize(xwr, entities);
            }
            catch (Exception ex)
            {
                Status.IsSuccess = false;
                Status.Errors.Add(ex.Message);
            }
            finally
            {
                xwr.Flush();
                xwr.Close();
            }
            return sb.ToString();
        }

        private ProviderParameters Parameters
        {
            get
            {
                return _parameters;
            }
            set { }
        }

        public StatusInfo Status { get; private set; }

        #endregion

        public void Dispose()
        {
        }
    }
}
