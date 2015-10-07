using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using AppFramework.Core.Classes.IE;
using AppFramework.Core.Classes.IE.Adapters;
using AppFramework.Core.Exceptions;
using Common.Logging;

namespace AppFramework.Core.Classes
{
    public class AssetTemplateService : IAssetTemplateService
    {
        private static readonly string TemplatesPath = ApplicationSettings.AssetTemplatesPath;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IXMLToAssetsAdapter _xmlToAssetsAdapter;
        private readonly ILog _logger;

        public AssetTemplateService(
            IAssetTypeRepository assetTypeRepository, 
            IXMLToAssetsAdapter xmlToAssetsAdapter,
            ILog logger)
        {
            if (assetTypeRepository == null)
                throw new ArgumentNullException("IAssetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (xmlToAssetsAdapter == null)
                throw new ArgumentNullException("IXMLToAssetsAdapter");
            _xmlToAssetsAdapter = xmlToAssetsAdapter;
            if (logger == null)
                throw new ArgumentNullException("logger");
            _logger = logger;
        }

        public List<AssetTemplate> GetTemplatesByAssetTypeUid(long assetTypeUid)
        {
            var currentType = _assetTypeRepository.GetByUid(assetTypeUid);
            if (currentType == null)
                throw new ArgumentException("Cannot find AssetType with given id");

            var templates = new List<AssetTemplate>();
            var pathToTemplates = Path.Combine(TemplatesPath, assetTypeUid.ToString());
            if (!Directory.Exists(pathToTemplates)) 
                return templates;

            var dInfo = new DirectoryInfo(pathToTemplates);
            var files = dInfo.GetFiles();

            foreach (var file in files)
            {
                var templateId = long.Parse(file.Name.Split(new []{ '.' }, 
                    StringSplitOptions.RemoveEmptyEntries).First());

                try
                {
                    var entity = _xmlToAssetsAdapter.GetEntities(file.FullName, currentType).FirstOrDefault();
                    if (entity == null)
                        continue;
                    var template = new AssetTemplate
                    {
                        ID = templateId,
                        Template = entity
                    };
                    templates.Add(template);
                }
                catch (ImportException)
                {
                    _logger.WarnFormat("Cannot add template: XML is improperly configured. Path: {0}",
                        file.FullName);
                }
                catch (XmlException ex)
                {
                    _logger.Warn(ex);
                }
            }
            return templates;
        }

        public AssetTemplate GetTemplateByID(long assetTypeUID, long templateId)
        {
            string pathToTemplates = Path.Combine(TemplatesPath, assetTypeUID.ToString());
            if (!Directory.Exists(pathToTemplates)) return null;

            string filePath = Path.Combine(pathToTemplates, templateId.ToString() + ".xml");
            if (!File.Exists(filePath)) return null;

            var currentType = _assetTypeRepository.GetByUid(assetTypeUID);
            var template = new AssetTemplate()
            {
                ID = templateId,
                Template = _xmlToAssetsAdapter.GetEntities(filePath, currentType).FirstOrDefault()
            };
            return template;
        }

        public static bool DeleteById(long assetTypeUID, long templateId)
        {
            string templatePath = Path.Combine(TemplatesPath, assetTypeUID.ToString());
            string filePath = Path.Combine(templatePath, templateId.ToString() + ".xml");

            if (!Directory.Exists(templatePath)) return false;
            if (!File.Exists(filePath)) return false;

            try
            {
                File.Delete(filePath);
            }
            catch
            {
                try { File.Delete(filePath); }
                catch { return false; }
            }
            return true;
        }

        public void Save(AssetTemplate template)
        {
            if (template.Template == null) 
                throw new NullReferenceException();

            long assetTypeUID = template.Template.GetConfiguration().UID;
            string templatePath = Path.Combine(TemplatesPath, assetTypeUID.ToString());

            if (!Directory.Exists(templatePath))
                Directory.CreateDirectory(templatePath);

            var dInfo = new DirectoryInfo(templatePath);
            FileInfo[] files = dInfo.GetFiles();

            string lstId = (from f in files
                            orderby f.Name descending
                            select f.Name).FirstOrDefault();

            long nTemplateID = 1;
            if (!string.IsNullOrEmpty(lstId))
                nTemplateID = long.Parse(lstId.Split(new char[1] { '.' }, 
                    StringSplitOptions.RemoveEmptyEntries)[0]) + 1;

            ImportExportManager.ExportSingleAsset(template.Template, 
                Path.Combine(templatePath, nTemplateID.ToString() + ".xml"));
        }

        public bool Update(AssetTemplate template)
        {
            if (template.Template == null) 
                throw new NullReferenceException("Asset wasn't initalized");

            long assetTypeUID = template.Template.GetConfiguration().UID;
            string templatePath = Path.Combine(TemplatesPath, assetTypeUID.ToString());
            string filePath = Path.Combine(templatePath, template.ID.ToString() + ".xml");

            if (!Directory.Exists(templatePath)) 
                return false;
            if (!File.Exists(filePath)) 
                return false;

            using (FileStream fs = File.Open(filePath, FileMode.Open))
            {
                template.Template.Serialize(fs);
            }

            return true;
        }

        public bool Delete(AssetTemplate template)
        {
            long assetTypeUID = template.Template.GetConfiguration().UID;
            string templatePath = Path.Combine(TemplatesPath, assetTypeUID.ToString());
            string filePath = Path.Combine(templatePath, template.ID.ToString() + ".xml");

            if (!Directory.Exists(templatePath)) 
                return false;
            if (!File.Exists(filePath)) 
                return false;

            File.Delete(filePath);

            return true;
        }
    }
}
