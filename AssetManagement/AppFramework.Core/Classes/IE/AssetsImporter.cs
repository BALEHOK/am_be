using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.IE.Adapters;
using AppFramework.Core.ConstantsEnumerators;
using Common.Logging;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppFramework.Core.Classes.IE
{
    public class AssetsImporter
    {
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IExcelToXmlConverter _excelToXmlAdapter;
        private readonly IXMLToAssetsAdapter _xmlToAssetsAdapter;
        private readonly ILog _logger;

        public AssetsImporter(
            IAssetTypeRepository assetTypeRepository,
            IExcelToXmlConverter excelToXmlAdapter,
            IXMLToAssetsAdapter xmlToAssetsAdapter,
            ILog logger)
        {
            if (assetTypeRepository == null)
                throw new ArgumentNullException("IAssetTypeRepository");
            if (excelToXmlAdapter == null)
                throw new ArgumentNullException("IExcelToXmlAdapter");
            if (xmlToAssetsAdapter == null)
                throw new ArgumentNullException("IXMLToAssetsAdapter");
            if (logger == null)
                throw new ArgumentNullException("IUnitOfWork");

            _logger = logger;
            _assetTypeRepository = assetTypeRepository;
            _excelToXmlAdapter = excelToXmlAdapter;
            _xmlToAssetsAdapter = xmlToAssetsAdapter;
        }

        public IEnumerable<Asset> Import(
            long assetTypeId,
            long userId,
            string filepath,
            List<string> sheets, 
            BindingInfo bindings)
        {
            var assetType = _assetTypeRepository.GetById(assetTypeId);
            var xmlExport = _excelToXmlAdapter.ConvertToXml(filepath, bindings, assetType, sheets);

            _logger.DebugFormat("Bindings:\r\n{0}", 
                bindings.ToXml());
            _logger.DebugFormat("Intermediate xml result:\r\n{0}", 
                xmlExport.ToString());

            var result = _xmlToAssetsAdapter.GetEntities(xmlExport, assetType).ToList();

            foreach (var asset in result)
            {
                asset.Attributes
                     .Where(a => a.Configuration.DataTypeEnum == Enumerators.DataType.Role)
                     .ForEach(_fixRoleAttribute);
                asset[AttributeNames.UpdateUserId].Value = userId.ToString();
                asset[AttributeNames.UpdateUserId].ValueAsId = userId;
                asset[AttributeNames.UpdateDate].Value = DateTime.Now.ToString(
                    ApplicationSettings.DisplayCultureInfo.DateTimeFormat);
                yield return asset;
            }
        }

        private static void _fixRoleAttribute(AssetAttribute attribute)
        {
            int parsedInt;
            if (!int.TryParse(attribute.Value, out parsedInt))
            {
                PredefinedRoles role;
                attribute.Value = Enum.TryParse<PredefinedRoles>(attribute.Value, out role)
                    ? ((int) role).ToString()
                    : ((int) PredefinedRoles.OnlyPerson).ToString();
            }
        }
    }
}
