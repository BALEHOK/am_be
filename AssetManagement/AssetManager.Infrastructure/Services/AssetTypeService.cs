using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Calculation;
using AppFramework.Core.Classes.Extensions;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models.TypeModels;
using AssetManager.Infrastructure.Permissions;

namespace AssetManager.Infrastructure.Services
{
    public class AssetTypeService : IAssetTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScreensService _screensService;
        private readonly IAssetTypePermissionChecker _assetTypePermissionChecker;

        public AssetTypeService(IUnitOfWork unitOfWork, IScreensService screensService,
            IAssetTypePermissionChecker assetTypePermissionChecker)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (screensService == null)
                throw new ArgumentNullException("screensService");
            _screensService = screensService;
            if (assetTypePermissionChecker == null)
                throw new ArgumentNullException("assetTypePermissionChecker");
            _assetTypePermissionChecker = assetTypePermissionChecker;
        }

        public AssetTypeModel GetAssetType(long userId, long id, bool loadAttributes = false)
        {
            Expression<Func<DynEntityConfig, object>> include;

            if (loadAttributes)
            {
                include = c => c.DynEntityAttribConfigs.Select(a => a.DataType);
            }
            else
            {
                include = null;
            }

            var typeConfig = _unitOfWork.DynEntityConfigRepository
                .SingleOrDefault(c => c.DynEntityConfigId == id && c.ActiveVersion, include);

            _assetTypePermissionChecker.EnsureReadPermission(typeConfig, userId);

            return typeConfig != null
                ? CreateAssetTypeModel(typeConfig, loadAttributes, true)
                : null;
        }

        public IEnumerable<DynEntityConfig> GetAssetTypesByIds(long userId, IEnumerable<long> typesToload)
        {
            return _assetTypePermissionChecker.FilterReadPermitted(
                _unitOfWork.DynEntityConfigRepository
                    .Get(c =>
                        c.ActiveVersion && c.Active
                        && typesToload.Any(id => c.DynEntityConfigId == id)),
                userId);
        }

        public IEnumerable<AssetTypeModel> GetAssetTypes(long userId, bool loadAttributes = false, bool loadScreens = false)
        {
            return GetAssetTypes(userId, loadAttributes, loadScreens,
                (types, user) => _assetTypePermissionChecker.FilterReadPermitted(types, user));
        }

        public IEnumerable<AssetTypeModel> GetWritableAssetTypes(long userId, bool loadAttributes = false, bool loadScreens = false)
        {
            return GetAssetTypes(userId, loadAttributes, loadScreens,
                (types, user) => _assetTypePermissionChecker.FilterWritePermitted(types, user));
        }

        public IEnumerable<DynEntityAttribConfig> GetChildAttribs(long userId, long configId)
        {
            var childAttribs = _unitOfWork.DynEntityAttribConfigRepository.Get(
                    a => a.DataType.Name == "asset"
                        && a.RelatedAssetTypeID == configId
                        && a.ActiveVersion && a.Active && a.DynEntityConfig.ActiveVersion && a.DynEntityConfig.Active,
                    include: a => a.DynEntityConfig)
                .ToList();

            var permittedConfigs = _assetTypePermissionChecker
                .FilterWritePermitted(
                    childAttribs.Select(a => a.DynEntityConfig).DistinctBy(c => c.DynEntityConfigId), userId)
                .Select(c => c.DynEntityConfigId)
                .ToList();

            return childAttribs.Where(a => permittedConfigs.Any(c => c == a.DynEntityConfig.DynEntityConfigId));
        }

        private IEnumerable<AssetTypeModel> GetAssetTypes(long userId, bool loadAttributes, bool loadScreens,
            Func<IEnumerable<DynEntityConfig>, long, IEnumerable<DynEntityConfig>> accessFilter)
        {
            var activeConfigs = _unitOfWork.DynEntityConfigRepository
                .Get(c => c.ActiveVersion && c.Active, include: c => c.DynEntityAttribConfigs.Select(a => a.DataType));

            var permittedAssetTypes = accessFilter(activeConfigs, userId);

            // load types and attributes info
            var typesInfo = permittedAssetTypes.Select(typeConfig => CreateAssetTypeModel(typeConfig, loadAttributes))
                .OrderBy(t => t.DisplayName)
                .ToList();

            if (loadScreens)
                LoadScreens(typesInfo);

            return typesInfo;
        }

        private static AssetTypeModel CreateAssetTypeModel(DynEntityConfig typeConfig, bool loadAttributes,
            bool showOnPanelOnlyAttributes = false)
        {
            return new AssetTypeModel
            {
                Id = typeConfig.DynEntityConfigId,
                Uid = typeConfig.DynEntityConfigUid,
                DisplayName = typeConfig.NameLocalized(),
                DbName = typeConfig.DBTableName,
                Description = typeConfig.Comment,
                Revision = typeConfig.Revision,
                UpdateDate = typeConfig.UpdateDate,
                Attributes = loadAttributes
                    ? LoadAttributes(typeConfig, showOnPanelOnlyAttributes)
                    : null
            };
        }

        private static List<AttributeTypeModel> LoadAttributes(DynEntityConfig config, bool showOnPanelOnlyAttributes)
        {
            var dataTypeEnum = typeof(Enumerators.DataType);
            var attributes =
                config.DynEntityAttribConfigs.Where(a => !showOnPanelOnlyAttributes || a.IsShownOnPanel).Select(
                    a =>
                    {
                        var attributeInfo = new AttributeTypeModel
                        {
                            Id = a.DynEntityAttribConfigId,
                            RelationId = a.RelatedAssetTypeID ?? 0,
                            DisplayName = a.NameLocalized(),
                            DbName = a.DBTableFieldname,
                            DisplayOrder = a.DisplayOrder,
                            ValidationExpression = a.ValidationExpr,
                            CalculationFormula = a.CalculationFormula,
                            DataType = (Enumerators.DataType) Enum.Parse(dataTypeEnum, a.DataType.Name, true)
                        };
                        return attributeInfo;
                    }).OrderBy(a => a.DisplayOrder).ToList();
            return attributes;
        }

        private void LoadScreens(List<AssetTypeModel> typesInfo)
        {
            typesInfo.ForEach(type =>
            {
                var screens = _screensService.GetScreensByAssetTypeUid(type.Uid);

                var screensModel = screens.Select(screen =>
                {
                    var screenModel = new AssetTypeScreenModel
                    {
                        Id = screen.ScreenId,
                        Name = screen.Name.Localized()
                    };

                    var panels = screen.AttributePanel.Select(panel =>
                    {
                        var assetPanel = new AssetTypeScreenPanelModel
                        {
                            Id = panel.AttributePanelId,
                            Name = panel.Name.Localized(),
                            ScreenModel = screenModel,
                            Attributes =
                                panel.AttributePanelAttribute.Select(
                                    apa => new ScreenPanelAttributeModel
                                    {
                                        Id = apa.AttributePanelAttributeId,
                                        AttributeId = apa.DynEntityAttribConfig.DynEntityAttribConfigId,
                                        ScreenFormula = apa.ScreenFormula
                                    }).ToList()
                        };

                        return assetPanel;
                    }).ToList();

                    screenModel.Panels = panels;

                    return screenModel;
                }).ToList();

                type.Screens = screensModel;
            });
        }

        public IEnumerable<AssetTypeModel> GetReservableAssetTypes(long userId)
        {
            return GetAssetTypes(userId, false, false, (types, user) =>
                _assetTypePermissionChecker.FilterReadPermitted(
                    types.Where(x => x.AllowBorrow),
                    user));
        }
    }
}