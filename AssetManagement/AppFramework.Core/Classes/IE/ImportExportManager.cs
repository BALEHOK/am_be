using AppFramework.Core.Calculation;
using AppFramework.Core.Classes.Barcode;
using AppFramework.DataProxy;
using Common.Logging;

namespace AppFramework.Core.Classes.IE
{
    using AppFramework.ConstantsEnumerators;
    using AppFramework.Core.AC.Authentication;
    using AppFramework.Core.Classes.Batch;
    using AppFramework.Core.Classes.IE.Adapters;
    using AppFramework.Core.Classes.IE.IntegrityValidation;
    using AppFramework.Core.Classes.IE.Providers;
    using AppFramework.Core.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Web;
    using System.Xml.Linq;

    /// <summary>
    /// Manages all operations with data import and export.
    /// Use this class for actual data retrieving.
    /// </summary>
    public class ImportExportManager : IImportExportManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILog _logger;
        private readonly IBatchJobFactory _batchJobFactory;

        public ImportExportManager(
            IUnitOfWork unitOfWork, 
            IAuthenticationService authenticationService,
            IBatchJobFactory batchJobFactory,
            ILog logger)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException();
            if (authenticationService == null)
                throw new ArgumentNullException();
            if (logger == null)
                throw new ArgumentNullException("logger");
            _logger = logger;
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _batchJobFactory = batchJobFactory;
        }

        /// <summary>
        /// TODO: refactor towards DI
        /// </summary>
        public BatchJob SynkAssets(
            string filePath, 
            long assetTypeId, 
            BindingInfo bindings, 
            string synchronizationField, 
            List<string> sheets, 
            bool deleteSourceOnSuccess)
        {
            var dbEntity = new Entities.ImportExport();

            dbEntity.GUID = Guid.NewGuid();
            dbEntity.FilePath = filePath;

            // serialize bindings to xml with ObjectToXML
            dbEntity.Bindings = bindings.ToXml();

            // set the operation properties
            dbEntity.Status = (int)ImportExportStatus.New;
            dbEntity.OperationType = (int)ImportExportOperationType.Synk;
            dbEntity.UpdateDate = DateTime.Now;
            dbEntity.UpdateUserId = _authenticationService.CurrentUserId;

            // add required extra parameters to operation
            ImportExportParameters ap = new ImportExportParameters();
            ap.Add(ImportExportParameter.AssetTypeId, assetTypeId.ToString());
            ap.Add(ImportExportParameter.DeleteOnSuccess, deleteSourceOnSuccess.ToString());
            dbEntity.Parameters = ap.ToXml();

            // save the state of importing operation
            _unitOfWork.ImportExportRepository.Insert(dbEntity);
            _unitOfWork.Commit();

            string serializedSheets = CustomSerializers.SerializeObject(sheets);

            // create batch job with parameter: GUID
            BatchJob job = null;
            if (!dbEntity.GUID.Equals(Guid.Empty))
            {
                var userId = _authenticationService.CurrentUserId;
                job = _batchJobFactory.CreateSyncAssetsJob(new SyncAssetsParameters(
                    dbEntity.GUID, 
                    userId, 
                    assetTypeId, 
                    synchronizationField, 
                    serializedSheets, 
                    filePath));
            }
            return job;
        }

        /// <summary>
        /// Performs assets export to XML string
        /// </summary>
        /// <param name="assets">Collection of assets of any types</param>
        /// <param param name="folderName">Path to folder where to write XML files</param>
        /// <returns>ActionResult with asset type ids and files paths</returns>
        public static ActionResult<List<KeyValuePair<long, string>>> ExportAssets(IEnumerable<Asset> assets, string folderName)
        {
            ActionResult<List<KeyValuePair<long, string>>> result = new ActionResult<List<KeyValuePair<long, string>>>();
            AssetsToXmlConverter adapter = new AssetsToXmlConverter(assets);

            ActionResult<List<ExportInfo>> export = adapter.GetXML();
            if (export.Status.IsSuccess)
            {
                foreach (ExportInfo item in export.Data)
                {
                    // make filename for xml
                    string filepath = Path.Combine(folderName, GetFileName(item));

                    // write xml to file 
                    ProviderParameters parameters = new ProviderParameters();
                    parameters.Add(ProviderParameter.WritePath, filepath);
                    XMLProvider provider = new XMLProvider(parameters);
                    provider.Write(item.Xml);

                    // collect filenames and AT IDs
                    result.Data.Add(new KeyValuePair<long, string>(item.AssetTypeId, filepath));
                }
            }
            return result;
        }

        /// <summary>
        /// Exports single asset to a specific file
        /// </summary>
        /// <param name="singleAsset">Asset of any type to be exported</param>
        /// <param name="fileName">Full qualified destiantion file path including file name</param>
        /// <returns></returns>
        public static void ExportSingleAsset(Asset singleAsset, string fileName)
        {
            var assets = new List<Asset> {singleAsset};
            var serializer = new AssetsToXmlConverter(assets);

            FileStream outputStream = null;

            outputStream = !File.Exists(fileName) 
                ? File.Create(fileName) 
                : File.OpenWrite(fileName);

            serializer.GetXML().Data[0].Xml.Save(outputStream);
            outputStream.Position = 0;
            outputStream.Close();
        }

        /// <summary>
        /// Exports single asset to xml and write content to memeory stream (used for asset export from view)
        /// </summary>
        /// <param name="singleAsset">Asset of any type to be exported</param>
        /// <returns></returns>
        public static MemoryStream ExportSingleAssetToMemory(Asset singleAsset)
        {
            List<Asset> forSerialization = new List<Asset>();
            forSerialization.Add(singleAsset);

            AssetsToXmlConverter serializer = new AssetsToXmlConverter(forSerialization);

            MemoryStream outputStream = new MemoryStream();

            serializer.GetXML().Data[0].Xml.Save(outputStream);
            outputStream.Position = 0;

            return outputStream;
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        internal static string GetFileName(ExportInfo item)
        {
            return ImportExportManager.GetFileName(item.AssetTypeName, item.AssetTypeId);
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="assetTypeName">Name of the asset type.</param>
        /// <param name="assetTypeId">The asset type id.</param>
        /// <returns></returns>
        public static string GetFileName(string assetTypeName, long assetTypeId)
        {
            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            string filename = DateTime.Now.ToShortDateString();
            Thread.CurrentThread.CurrentCulture = originalCulture;
            return Routines.SanitizeFileName(
                                        assetTypeName + "_" +
                                        assetTypeId + "_" +
                                        filename + ".xml");
        }

        /// <summary>
        /// Returns the list of XML fields
        /// </summary>
        /// <param name="filePath">path to DataSource file</param>
        /// <returns></returns>
        public static TransferResult<string> GetXMLDataSourceFields(string filePath, AssetType at)
        {
            SchemaGenerator sg = new SchemaGenerator(at);

            ProviderParameters parameters = new ProviderParameters();
            parameters.Add(ProviderParameter.ReadPath, filePath);
            parameters.Add(ProviderParameter.Schema, sg.GetSchema());
            XMLProvider provider = new XMLProvider(parameters);

            List<string> fields = provider.GetFields();
            return new TransferResult<string>(provider.Status, fields);
        }

        /// <summary>
        /// Returns the list of excel sheets
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static TransferResult<string> GetExcelDataSourceSheets(string filePath)
        {
            var provider = new ExcelProvider();
            var fields = provider.GetExcelSheetNames(filePath);
            return new TransferResult<string>(provider.Status, fields);
        }

        /// <summary>
        /// Returns the list of excel fields
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static TransferResult<KeyValuePair<string, string>> GetExcelDataSourceFields(string filePath, List<string> sheetNames)
        {
            var provider = new ExcelProvider();
            List<KeyValuePair<string, string>> fields = provider.GetFields(filePath, sheetNames).ToList();
            return new TransferResult<KeyValuePair<string, string>>(provider.Status, fields);
        }

        /// <summary>
        /// Returns the collection of AT attributes from given XML file
        /// </summary>
        /// <param name="pathToXML">path to XML file</param>
        /// <returns></returns>
        public static IEnumerable<AssetTypeAttribute> GetAssetTypeAttributesFromXML(string pathToXML)
        {
            ProviderParameters parameters = new ProviderParameters();
            parameters.Add(ProviderParameter.ReadPath, pathToXML);
            XMLProvider provider = new XMLProvider(parameters);

            object[] attributes = provider.Deserialize(typeof(AssetTypeAttribute[])).ToArray();

            if (attributes != null)
            {
                foreach (AssetTypeAttribute attr in attributes)
                {
                    yield return attr;
                }
            }
        }

        /// <summary>
        /// Returns the defauld template of clear and preconfigured AssetType
        /// </summary>
        /// <returns></returns>
        public static AssetType GetBasicAssetTypeConfiguration(
            Enumerators.AssetTypeClass typeClass, 
            IUnitOfWork unitOfWork,
            ILayoutRepository layoutRepository,
            string xmlFilePath = null)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            if (layoutRepository == null)
                throw new ArgumentNullException("layoutRepository");

            AssetType at = null;
            var parameters = new ProviderParameters();

            var xmlPath = string.Format(@"~/Config.Files/Predefined/{0}{1}.xml",
                                            ConfigurationManager.AppSettings["ApplicationType"],
                                            typeClass);
            var xmlFullPath = xmlFilePath ?? HttpContext.Current.Server.MapPath(xmlPath);

            parameters.Add(ProviderParameter.ReadPath, xmlFullPath);
            var provider = new XMLProvider(parameters);

            object[] assetTypes = provider.Deserialize(typeof(AssetType[])).ToArray();

            if (!provider.Status.IsSuccess)
                throw new Exception(string.Join(Environment.NewLine, provider.Status.Errors));

            at = assetTypes.FirstOrDefault() as AssetType;

            var validator = new AssetTypeValidator(unitOfWork, layoutRepository);
            if (at != null)
                validator.Heal(at);
            return at;
        }

        /// <summary>
        /// Returns the DataSourceType by the filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static DataSourceType GetDataSourceTypeByFileName(string filename)
        {
            string extension = filename.Split(new char[] { '.' }).Last().ToUpper();
            DataSourceType res = DataSourceType.UNKNOWN;

            foreach (DataSourceType type in Enum.GetValues(typeof(DataSourceType)))
            {
                if (type.ToString().ToUpper() == extension)
                {
                    res = type;
                    break;
                }
            }
            return res;
        }

        /// <summary>
        /// Checks the connection to AD service
        /// </summary>
        /// <returns></returns>
        public static ActionResult<bool> CheckADConnection(LDAPCredentials credentials)
        {
            LDAPProvider provider = new LDAPProvider(credentials);
            bool isCnn = provider.CheckConnection();
            return new ActionResult<bool>(provider.Status, isCnn);
        }

        /// <summary>
        /// Gets from XML the list of attributes
        /// which can be imported from Active Directory users entities
        /// </summary>
        /// <returns></returns>
        public static ActionResult<List<string>> GetActiveDirectoryUserFields()
        {
            ProviderParameters parameters = new ProviderParameters();
            parameters.Add(ProviderParameter.ReadPath, ApplicationSettings.ActiveDirectoryUserFieldsPath);
            XMLProvider provider = new XMLProvider(parameters);
            ActionResult<XDocument> result = provider.Read();

            List<string> fields = new List<string>();
            if (result.Status.IsSuccess)
            {
                XDocument document = result.Data;
                fields = (from node in document.Descendants()
                          where node.Name == "DisplayName"
                          select node.Value).ToList();
            }
            return new ActionResult<List<string>>(provider.Status, fields);
        }

        /// <summary>
        /// Retrieves the collection of users and convers to them to XML
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="bindings"></param>
        /// <param name="userType"></param>
        /// <returns></returns>
        public static StatusInfo SaveLDAPUsersToXML(LDAPCredentials credentials,
                                                    BindingInfo bindings,
                                                    AssetType userType,
                                                    string filepath)
        {
            throw new NotImplementedException();
            //StatusInfo status = new StatusInfo();
            //LDAPProvider readProvider = new LDAPProvider(credentials);

            //// get AD users DataSource
            //var users = readProvider.GetUsers(bindings.Bindings.Select(b => b.DataSourceFieldName).ToList());
            //status.Add(readProvider.Status);

            //if (status.IsSuccess)
            //{
            //    // get XDocument from DataSource
            //    var adapter = new LDAPToXMLAdapter(users, bindings, userType);
            //    var convRes = adapter.GetXml();

            //    /*
            //    // Optional operation: validate generated XML
            //    XMLValidator validator = new XMLValidator(convRes.Data.ToString(), userType);
            //    status.Add(validator.Validate());
            //     */

            //    if (status.IsSuccess)
            //    {
            //        // write xml to file 
            //        ProviderParameters parameters = new ProviderParameters();
            //        parameters.Add(ProviderParameter.WritePath, filepath);
            //        XMLProvider writeProvider = new XMLProvider(parameters);
            //        writeProvider.Write(convRes);
            //        status.Add(writeProvider.Status);
            //    }
            //}

            //return status;
        }

        /// <summary>
        /// Converts [id - Display name] bindings to [id - LDAP name] bindings
        /// </summary>        
        public static BindingInfo ConvertLDAPBindings(BindingInfo bindings)
        {
            ProviderParameters parameters = new ProviderParameters();
            parameters.Add(ProviderParameter.ReadPath, ApplicationSettings.ActiveDirectoryUserFieldsPath);
            XMLProvider provider = new XMLProvider(parameters);
            ActionResult<XDocument> result = provider.Read();

            BindingInfo newBindings = new BindingInfo();
           
            if (result.Status.IsSuccess)
            {
                XDocument document = result.Data;

                // make [Display name - LDAP name] pairs                
                IEnumerable<KeyValuePair<string, string>> nodes
                    = (from node in document.Descendants()
                       where node.Name == "DisplayName" && bindings.Bindings.Any(b => b.DataSourceFieldName == node.Value)
                       select new KeyValuePair<string, string>(node.Value, (from ldapNode in node.Parent.Descendants()
                                                                            where ldapNode.Name == "LDAPName"
                                                                            select ldapNode.Value).Single()));

                foreach (KeyValuePair<string, string> node in nodes)
                {
                    // from [id - Display name] get id value
                    foreach (var binding in bindings.Bindings.Where(b => b.DataSourceFieldName == node.Key))
                    {
                        // make [id - LDAP name] pair
                        newBindings.Bindings.Add(new ImportBinding
                        {
                            DestinationAttributeId = binding.DestinationAttributeId, 
                            DataSourceFieldName = node.Value
                        });
                    }
                }
            }
            return newBindings;
        }
    }
}
