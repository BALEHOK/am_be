using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models.TypeModels;

namespace AssetManager.Infrastructure.Services
{
    public interface IAssetTypeService
    {
        TypesInfoModel GetAssetTypes(bool loadAttributes = false, bool loadScreens = false);
    }

    public class AssetTypeService : IAssetTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScreensService _screensService;
        private readonly IDataFactory _dataFactory;

        public AssetTypeService(IUnitOfWork unitOfWork, IScreensService screensService, IDataFactory dataFactory)
        {
            if (unitOfWork == null)
                throw new System.ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (screensService == null)
                throw new System.ArgumentNullException("screensService");
            _screensService = screensService;
            if (dataFactory == null)
                throw new System.ArgumentNullException("dataFactory");
            _dataFactory = dataFactory;
        }

        public TypesInfoModel GetAssetTypes(bool loadAttributes = false, bool loadScreens = false)
        {
            var activeConfigs = _unitOfWork.DynEntityConfigRepository
                .Get(c => c.ActiveVersion, include: c => c.DynEntityAttribConfigs.Select(a => a.DataType))
                .ToList();

            // load types and attributes info
            var typesInfo = activeConfigs.Select(config =>
            {
                var typeInfo = new AssetTypeModel
                {
                    Id = config.DynEntityConfigId,
                    DisplayName = config.Name,
                    DbName = config.DBTableName,
                    Description = config.Comment,
                    Revision = config.Revision,
                    UpdateDate = config.UpdateDate,
                    Attributes = loadAttributes
                        ? LoadAttributes(config)
                        : null,
                };
                return typeInfo;
            })
            .OrderBy(t => t.DisplayName)
            .ToList();
            
            if (loadScreens)
                LoadScreens(typesInfo);

            return new TypesInfoModel
            {
                ActiveTypes = typesInfo
            };
        }

        private static List<AttributeTypeModel> LoadAttributes(DynEntityConfig config)
        {
            var attributes =
                config.DynEntityAttribConfigs.Select(
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
                            DataType = a.DataType.Name,
                        };
                        return attributeInfo;
                    }).OrderBy(a => a.DisplayOrder).ToList();
            return attributes;
        }

        private void LoadScreens(List<AssetTypeModel> typesInfo)
        {
            typesInfo.ForEach(type =>
            {
                var assetType = _dataFactory.Get<AssetType>(type.Id);
                var screens = _screensService.GetScreensByAssetTypeUid(assetType.UID);

                var screensModel = screens.Select(screen =>
                {
                    var screenModel = new AssetTypeScreenModel
                    {
                        Id = screen.ScreenId,
                        Name = screen.Name,
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
