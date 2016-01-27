using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models.TypeModels;

namespace AssetManager.Infrastructure.Services
{
    public class AssetTypeService : IAssetTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScreensService _screensService;

        public AssetTypeService(IUnitOfWork unitOfWork, IScreensService screensService)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (screensService == null)
                throw new ArgumentNullException("screensService");
            _screensService = screensService;
        }

        public AssetTypeModel GetAssetType(long id, bool loadAttributes = false)
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

            return typeConfig != null
                ? CreateAssetTypeModel(typeConfig, loadAttributes, true)
                : null;
        }

        public IEnumerable<DynEntityConfig> GetAssetTypesByIds(IEnumerable<long> typesToload)
        {
            return _unitOfWork.DynEntityConfigRepository
                .Get(c =>
                    c.ActiveVersion
                    && typesToload.Any(id => c.DynEntityConfigId == id));
        }

        public TypesInfoModel GetAssetTypes(bool loadAttributes = false, bool loadScreens = false)
        {
            var activeConfigs = _unitOfWork.DynEntityConfigRepository
                .Get(c => c.ActiveVersion && c.Active, include: c => c.DynEntityAttribConfigs.Select(a => a.DataType));

            // load types and attributes info
            var typesInfo = activeConfigs.Select(typeConfig => CreateAssetTypeModel(typeConfig, loadAttributes))
                .OrderBy(t => t.DisplayName)
                .ToList();

            if (loadScreens)
                LoadScreens(typesInfo);

            return new TypesInfoModel
            {
                ActiveTypes = typesInfo
            };
        }

        private static AssetTypeModel CreateAssetTypeModel(DynEntityConfig typeConfig, bool loadAttributes,
            bool showOnPanelOnlyAttributes = false)
        {
            return new AssetTypeModel
            {
                Id = typeConfig.DynEntityConfigId,
                Uid = typeConfig.DynEntityConfigUid,
                DisplayName = typeConfig.Name,
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
            var dataTypeEnum = typeof (Enumerators.DataType);
            var attributes =
                config.DynEntityAttribConfigs.Where(a => !showOnPanelOnlyAttributes || a.IsShownOnPanel).Select(
                    a =>
                    {
                        var attributeInfo = new AttributeTypeModel
                        {
                            Id = a.DynEntityAttribConfigId,
                            RelationId = a.RelatedAssetTypeID ?? 0,
                            DisplayName = a.Name,
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
                        Name = screen.Name
                    };

                    var panels = screen.AttributePanel.Select(panel =>
                    {
                        var assetPanel = new AssetTypeScreenPanelModel
                        {
                            Id = panel.AttributePanelId,
                            Name = panel.Name,
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
    }
}